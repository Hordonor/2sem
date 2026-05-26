namespace AsyncDataLibrary.Infrastructure;

/// <summary>
/// Низькорівневий провайдер для читання/запису файлів з даними.
/// Надає як синхронний, так і асинхронний доступ.
/// Використовує SemaphoreSlim (по одному на файл) для захисту від
/// паралельних записів при виклику Task.WhenAll.
/// UI та сервіси не знають про файлову систему — вся відповідальність тут.
/// </summary>
public class FileStorageProvider
{
    private readonly string _baseDirectory;

    // Словник семафорів: по одному на кожен файл (thread-safe ліниве створення)
    private readonly Dictionary<string, SemaphoreSlim> _fileLocks = new();
    private readonly object _lockDictLock = new();

    /// <param name="baseDirectory">
    /// Базова директорія для зберігання JSON файлів.
    /// Якщо не вказано — використовується поточна робоча директорія.
    /// </param>
    public FileStorageProvider(string? baseDirectory = null)
    {
        _baseDirectory = baseDirectory ?? Directory.GetCurrentDirectory();
        Directory.CreateDirectory(_baseDirectory);
    }

    private string GetFilePath(string fileName) =>
        Path.Combine(_baseDirectory, fileName);

    private SemaphoreSlim GetLock(string fileName)
    {
        lock (_lockDictLock)
        {
            if (!_fileLocks.TryGetValue(fileName, out var sem))
            {
                sem = new SemaphoreSlim(1, 1);
                _fileLocks[fileName] = sem;
            }
            return sem;
        }
    }

    // ── Синхронні операції ──────────────────────────────────────────

    /// <summary>Читає весь вміст файлу (синхронно). Повертає порожній рядок, якщо файл не існує.</summary>
    public string Read(string fileName)
    {
        string path = GetFilePath(fileName);
        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    /// <summary>Записує текст у файл (синхронно), перезаписуючи вміст.</summary>
    public void Write(string fileName, string content) =>
        File.WriteAllText(GetFilePath(fileName), content);

    // ── Асинхронні операції ─────────────────────────────────────────

    /// <summary>Читає весь вміст файлу (асинхронно). Повертає порожній рядок, якщо файл не існує.</summary>
    public async Task<string> ReadAsync(string fileName)
    {
        string path = GetFilePath(fileName);
        return File.Exists(path)
            ? await File.ReadAllTextAsync(path)
            : string.Empty;
    }

    /// <summary>
    /// Записує текст у файл (асинхронно), перезаписуючи вміст.
    /// Захищено SemaphoreSlim від паралельних записів в один файл.
    /// </summary>
    public async Task WriteAsync(string fileName, string content)
    {
        var sem = GetLock(fileName);
        await sem.WaitAsync();
        try
        {
            await File.WriteAllTextAsync(GetFilePath(fileName), content);
        }
        finally
        {
            sem.Release();
        }
    }
}
