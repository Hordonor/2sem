using AsyncDataLibrary.Models;

namespace AsyncDataLibrary.Interfaces;

/// <summary>
/// Узагальнений (generic) репозиторій для CRUD-операцій над сутностями.
/// Підтримує як синхронні, так і асинхронні варіанти кожної операції.
/// Обмеження: T — клас, що реалізує IEntity.
/// </summary>
public interface IRepository<T> where T : class, IEntity
{
    // ── Синхронні операції ──────────────────────────────────────────

    /// <summary>Повертає всі сутності.</summary>
    IEnumerable<T> GetAll();

    /// <summary>Повертає сутність за ідентифікатором або null.</summary>
    T? GetById(Guid id);

    /// <summary>Додає нову сутність.</summary>
    void Add(T entity);

    /// <summary>Оновлює існуючу сутність за Id.</summary>
    void Update(T entity);

    /// <summary>Видаляє сутність за ідентифікатором.</summary>
    void Delete(Guid id);

    // ── Асинхронні операції ─────────────────────────────────────────

    /// <summary>Асинхронно повертає всі сутності.</summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Асинхронно повертає сутність за ідентифікатором або null.</summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>Асинхронно додає нову сутність.</summary>
    Task AddAsync(T entity);

    /// <summary>Асинхронно оновлює існуючу сутність за Id.</summary>
    Task UpdateAsync(T entity);

    /// <summary>Асинхронно видаляє сутність за ідентифікатором.</summary>
    Task DeleteAsync(Guid id);
}
