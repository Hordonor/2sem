namespace AsyncDataLibrary.Models;

/// <summary>
/// Базовий інтерфейс для всіх сутностей, що зберігаються у JSON.
/// </summary>
public interface IEntity
{
    Guid Id { get; set; }
}
