namespace AsyncDataLibrary.Models;

[StorageFile("books.json")]
public class Book : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;

    public override string ToString() =>
        $"Book [{Id:D}] | \"{Title}\" by {Author} | Genre: {Genre} | Price: {Price:C} | Available: {IsAvailable}";
}
