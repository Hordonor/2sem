using AsyncDataLibrary.Interfaces;
using AsyncDataLibrary.Models;

namespace AsyncDataLibrary.Services;

/// <summary>
/// Сервіс для роботи з книгами.
/// Надає зручний API поверх IRepository<Book>.
/// </summary>
public class BookService
{
    private readonly IRepository<Book> _repository;

    public BookService(IRepository<Book> repository)
    {
        _repository = repository;
    }

    // ── Синхронні ──────────────────────────────────────────────────

    public IEnumerable<Book> GetAll() => _repository.GetAll();
    public Book? GetById(Guid id) => _repository.GetById(id);
    public void Add(Book book) => _repository.Add(book);
    public void Update(Book book) => _repository.Update(book);
    public void Delete(Guid id) => _repository.Delete(id);

    // ── Асинхронні ─────────────────────────────────────────────────

    public Task<IEnumerable<Book>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Book?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);
    public Task AddAsync(Book book) => _repository.AddAsync(book);
    public Task UpdateAsync(Book book) => _repository.UpdateAsync(book);
    public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);

    // ── Бізнес-логіка ──────────────────────────────────────────────

    /// <summary>Синхронний пошук доступних книг за жанром.</summary>
    public IEnumerable<Book> GetAvailableByGenre(string genre)
        => GetAll().Where(b =>
            b.IsAvailable &&
            b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));

    /// <summary>Асинхронний пошук доступних книг за жанром.</summary>
    public async Task<IEnumerable<Book>> GetAvailableByGenreAsync(string genre)
    {
        var all = await GetAllAsync();
        return all.Where(b =>
            b.IsAvailable &&
            b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
    }
}
