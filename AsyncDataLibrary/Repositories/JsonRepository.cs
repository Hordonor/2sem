using AsyncDataLibrary.Infrastructure;
using AsyncDataLibrary.Interfaces;
using AsyncDataLibrary.Models;

namespace AsyncDataLibrary.Repositories;

/// <summary>
/// Узагальнена реалізація IRepository<T> на базі JSON файлів.
/// Автоматично визначає назву файлу через атрибут [StorageFile] та рефлексію.
/// UI і сервіси не знають нічого про JSON або файлову систему.
/// SemaphoreSlim захищає весь цикл «читання → зміна → запис» від гонок даних.
/// </summary>
public class JsonRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly FileStorageProvider _storage;
    private readonly IDataSerializer _serializer;
    private readonly SemaphoreSlim _asyncLock = new(1, 1); // 1 async op at a time
    private readonly string _fileName;

    public JsonRepository(FileStorageProvider storage, IDataSerializer serializer)
    {
        _storage = storage;
        _serializer = serializer;

        // Знаходимо назву файлу через атрибут [StorageFile] на типі T
        var attr = typeof(T)
            .GetCustomAttributes(typeof(StorageFileAttribute), false)
            .FirstOrDefault() as StorageFileAttribute;

        _fileName = attr?.FileName
            ?? throw new InvalidOperationException(
                $"Тип {typeof(T).Name} не має атрибуту [StorageFile]. " +
                "Вкажіть ім'я JSON-файлу, наприклад: [StorageFile(\"items.json\")]");
    }

    // ── Синхронні операції ──────────────────────────────────────────

    private List<T> LoadAll()
    {
        string json = _storage.Read(_fileName);
        if (string.IsNullOrWhiteSpace(json)) return new List<T>();
        return _serializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    private void SaveAll(List<T> items)
    {
        string json = _serializer.Serialize(items);
        _storage.Write(_fileName, json);
    }

    public IEnumerable<T> GetAll() => LoadAll();

    public T? GetById(Guid id) =>
        LoadAll().FirstOrDefault(x => x.Id == id);

    public void Add(T entity)
    {
        var list = LoadAll();
        list.Add(entity);
        SaveAll(list);
    }

    public void Update(T entity)
    {
        var list = LoadAll();
        int idx = list.FindIndex(x => x.Id == entity.Id);
        if (idx == -1)
            throw new KeyNotFoundException($"{typeof(T).Name} з Id={entity.Id} не знайдено.");
        list[idx] = entity;
        SaveAll(list);
    }

    public void Delete(Guid id)
    {
        var list = LoadAll();
        int removed = list.RemoveAll(x => x.Id == id);
        if (removed == 0)
            throw new KeyNotFoundException($"{typeof(T).Name} з Id={id} не знайдено.");
        SaveAll(list);
    }

    // ── Асинхронні операції ─────────────────────────────────────────

    private async Task<List<T>> LoadAllAsync()
    {
        string json = await _storage.ReadAsync(_fileName);
        if (string.IsNullOrWhiteSpace(json)) return new List<T>();
        return _serializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    private async Task SaveAllAsync(List<T> items)
    {
        string json = _serializer.Serialize(items);
        await _storage.WriteAsync(_fileName, json);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        await _asyncLock.WaitAsync();
        try { return await LoadAllAsync(); }
        finally { _asyncLock.Release(); }
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        await _asyncLock.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            return list.FirstOrDefault(x => x.Id == id);
        }
        finally { _asyncLock.Release(); }
    }

    public async Task AddAsync(T entity)
    {
        await _asyncLock.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            list.Add(entity);
            await SaveAllAsync(list);
        }
        finally { _asyncLock.Release(); }
    }

    public async Task UpdateAsync(T entity)
    {
        await _asyncLock.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            int idx = list.FindIndex(x => x.Id == entity.Id);
            if (idx == -1)
                throw new KeyNotFoundException($"{typeof(T).Name} з Id={entity.Id} не знайдено.");
            list[idx] = entity;
            await SaveAllAsync(list);
        }
        finally { _asyncLock.Release(); }
    }

    public async Task DeleteAsync(Guid id)
    {
        await _asyncLock.WaitAsync();
        try
        {
            var list = await LoadAllAsync();
            int removed = list.RemoveAll(x => x.Id == id);
            if (removed == 0)
                throw new KeyNotFoundException($"{typeof(T).Name} з Id={id} не знайдено.");
            await SaveAllAsync(list);
        }
        finally { _asyncLock.Release(); }
    }
}
