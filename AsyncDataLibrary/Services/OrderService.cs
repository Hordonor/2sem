using AsyncDataLibrary.Interfaces;
using AsyncDataLibrary.Models;

namespace AsyncDataLibrary.Services;

/// <summary>
/// Сервіс для роботи із замовленнями.
/// Надає зручний API поверх IRepository<Order>.
/// </summary>
public class OrderService
{
    private readonly IRepository<Order> _repository;

    public OrderService(IRepository<Order> repository)
    {
        _repository = repository;
    }

    // ── Синхронні ──────────────────────────────────────────────────

    public IEnumerable<Order> GetAll() => _repository.GetAll();
    public Order? GetById(Guid id) => _repository.GetById(id);
    public void Add(Order order) => _repository.Add(order);
    public void Update(Order order) => _repository.Update(order);
    public void Delete(Guid id) => _repository.Delete(id);

    // ── Асинхронні ─────────────────────────────────────────────────

    public Task<IEnumerable<Order>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Order?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);
    public Task AddAsync(Order order) => _repository.AddAsync(order);
    public Task UpdateAsync(Order order) => _repository.UpdateAsync(order);
    public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);

    // ── Бізнес-логіка ──────────────────────────────────────────────

    /// <summary>Синхронне отримання замовлень певного користувача.</summary>
    public IEnumerable<Order> GetByUserId(Guid userId)
        => GetAll().Where(o => o.UserId == userId);

    /// <summary>Асинхронне отримання замовлень певного користувача.</summary>
    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId)
    {
        var all = await GetAllAsync();
        return all.Where(o => o.UserId == userId);
    }

    /// <summary>Асинхронна зміна статусу замовлення.</summary>
    public async Task ChangeStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException($"Замовлення з Id={orderId} не знайдено.");
        order.Status = newStatus;
        await UpdateAsync(order);
    }
}
