namespace AsyncDataLibrary.Models;

[StorageFile("users.json")]
public class User : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public override string ToString() =>
        $"User [{Id:D}] | {Name} | {Email} | Registered: {CreatedAt:yyyy-MM-dd}";
}
