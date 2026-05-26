using AsyncDataLibrary.Interfaces;
using AsyncDataLibrary.Models;

namespace AsyncDataLibrary.Services;

/// <summary>
/// Сервіс для роботи з користувачами.
/// Надає зручний API поверх IRepository<User>.
/// UI викликає методи цього сервісу, не знаючи нічого про JSON.
/// </summary>
public class UserService
{
    private readonly IRepository<User> _repository;

    public UserService(IRepository<User> repository)
    {
        _repository = repository;
    }

    // ── Синхронні ──────────────────────────────────────────────────

    public IEnumerable<User> GetAll() => _repository.GetAll();
    public User? GetById(Guid id) => _repository.GetById(id);
    public void Add(User user) => _repository.Add(user);
    public void Update(User user) => _repository.Update(user);
    public void Delete(Guid id) => _repository.Delete(id);

    // ── Асинхронні ─────────────────────────────────────────────────

    public Task<IEnumerable<User>> GetAllAsync() => _repository.GetAllAsync();
    public Task<User?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);
    public Task AddAsync(User user) => _repository.AddAsync(user);
    public Task UpdateAsync(User user) => _repository.UpdateAsync(user);
    public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);

    // ── Бізнес-логіка ──────────────────────────────────────────────

    /// <summary>Синхронний пошук за email (нечутливий до регістру).</summary>
    public User? FindByEmail(string email)
        => GetAll().FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    /// <summary>Асинхронний пошук за email (нечутливий до регістру).</summary>
    public async Task<User?> FindByEmailAsync(string email)
    {
        var all = await GetAllAsync();
        return all.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
