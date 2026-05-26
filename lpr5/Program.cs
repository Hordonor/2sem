using AsyncDataLibrary.Infrastructure;
using AsyncDataLibrary.Models;
using AsyncDataLibrary.Repositories;
using AsyncDataLibrary.Services;
using System.Diagnostics;

// ╔══════════════════════════════════════════════════════════════════╗
// ║         ПРАКТИЧНА РОБОТА №5: Async / Service / Repository        ║
// ║         UI знає тільки про сервіси — нічого про JSON/файли       ║
// ╚══════════════════════════════════════════════════════════════════╝

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("========================================");
Console.WriteLine("  ПРАКТИЧНА РОБОТА №5: Async Data Library");
Console.WriteLine("========================================\n");

// ── Composition Root ────────────────────────────────────────────────
// Тут і тільки тут створюються всі залежності та передаються сервісам.
// UI (цей файл) — "пульт керування", що лише викликає сервіси.

string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

// Очищення даних від попередніх запусків (для чистої демонстрації)
if (Directory.Exists(dataDir))
    Directory.Delete(dataDir, recursive: true);

var serializer   = new JsonDataSerializer();
var storage      = new FileStorageProvider(dataDir);

var userRepo     = new JsonRepository<User>(storage, serializer);
var bookRepo     = new JsonRepository<Book>(storage, serializer);
var orderRepo    = new JsonRepository<Order>(storage, serializer);

var userService  = new UserService(userRepo);
var bookService  = new BookService(bookRepo);
var orderService = new OrderService(orderRepo);

Console.WriteLine($"Директорія зберігання даних: {dataDir}\n");

// ════════════════════════════════════════════════════════════════════
// ЧАСТИНА 1: СИНХРОННІ ОПЕРАЦІЇ
// ════════════════════════════════════════════════════════════════════
Console.WriteLine("════════════════════════════════════════");
Console.WriteLine("ЧАСТИНА 1: СИНХРОННІ ОПЕРАЦІЇ");
Console.WriteLine("════════════════════════════════════════\n");

// ── Додавання користувачів (sync) ──
Console.WriteLine("--- Додавання користувачів (sync) ---");
var user1 = new User { Name = "Олена Ковальчук", Email = "olena@example.com" };
var user2 = new User { Name = "Микола Бондаренко", Email = "mykola@example.com" };
userService.Add(user1);
userService.Add(user2);
Console.WriteLine($"  Додано: {user1.Name}");
Console.WriteLine($"  Додано: {user2.Name}");

// ── Додавання книг (sync) ──
Console.WriteLine("\n--- Додавання книг (sync) ---");
var book1 = new Book { Title = "Kobzar", Author = "Тарас Шевченко", Genre = "Poetry",   Price = 150.00m };
var book2 = new Book { Title = "Тіні забутих предків", Author = "М. Коцюбинський", Genre = "Drama",    Price = 120.00m };
var book3 = new Book { Title = "Захар Беркут", Author = "Іван Франко", Genre = "Historical", Price = 200.00m };
bookService.Add(book1);
bookService.Add(book2);
bookService.Add(book3);
Console.WriteLine($"  Додано: {book1.Title}");
Console.WriteLine($"  Додано: {book2.Title}");
Console.WriteLine($"  Додано: {book3.Title}");

// ── Пошук (sync) ──
Console.WriteLine("\n--- Пошук користувача за email (sync) ---");
var found = userService.FindByEmail("mykola@example.com");
Console.WriteLine(found != null ? $"  Знайдено: {found}" : "  Не знайдено");

// ── Перегляд всіх книг (sync) ──
Console.WriteLine("\n--- Всі книги (sync) ---");
foreach (var b in bookService.GetAll())
    Console.WriteLine($"  {b}");

// ── Замовлення (sync) ──
Console.WriteLine("\n--- Створення замовлення (sync) ---");
var order1 = new Order
{
    UserId     = user1.Id,
    BookId     = book1.Id,
    Quantity   = 2,
    TotalPrice = book1.Price * 2
};
orderService.Add(order1);
Console.WriteLine($"  Замовлення створено: {order1}");

// ── Оновлення (sync) ──
Console.WriteLine("\n--- Оновлення книги (sync) ---");
book2.Price = 99.00m;
bookService.Update(book2);
Console.WriteLine($"  Оновлено ціну книги \"{book2.Title}\" → {book2.Price:C}");

// ── Видалення (sync) ──
Console.WriteLine("\n--- Видалення замовлення (sync) ---");
orderService.Delete(order1.Id);
Console.WriteLine($"  Замовлення {order1.Id:D} видалено");

// ════════════════════════════════════════════════════════════════════
// ЧАСТИНА 2: АСИНХРОННІ ОПЕРАЦІЇ
// ════════════════════════════════════════════════════════════════════
Console.WriteLine("\n════════════════════════════════════════");
Console.WriteLine("ЧАСТИНА 2: АСИНХРОННІ ОПЕРАЦІЇ (async/await)");
Console.WriteLine("════════════════════════════════════════\n");

// ── Додавання (async) ──
Console.WriteLine("--- Додавання нових записів (async) ---");
var user3 = new User { Name = "Соломія Павленко", Email = "solomiia@example.com" };
var book4 = new Book { Title = "Лісова пісня", Author = "Леся Українка", Genre = "Drama", Price = 180.00m };

await userService.AddAsync(user3);
await bookService.AddAsync(book4);
Console.WriteLine($"  Async додано: {user3.Name}");
Console.WriteLine($"  Async додано: {book4.Title}");

// ── Отримання всіх (async) ──
Console.WriteLine("\n--- Всі користувачі (async) ---");
var allUsers = await userService.GetAllAsync();
foreach (var u in allUsers)
    Console.WriteLine($"  {u}");

// ── Пошук за жанром (async) ──
Console.WriteLine("\n--- Книги жанру Drama (async) ---");
var dramaBooks = await bookService.GetAvailableByGenreAsync("Drama");
foreach (var b in dramaBooks)
    Console.WriteLine($"  {b}");

// ── Замовлення та зміна статусу (async) ──
Console.WriteLine("\n--- Створення та зміна статусу замовлення (async) ---");
var order2 = new Order
{
    UserId     = user3.Id,
    BookId     = book4.Id,
    Quantity   = 1,
    TotalPrice = book4.Price
};
await orderService.AddAsync(order2);
Console.WriteLine($"  Async замовлення створено: {order2}");

await orderService.ChangeStatusAsync(order2.Id, OrderStatus.Processing);
var updatedOrder = await orderService.GetByIdAsync(order2.Id);
Console.WriteLine($"  Статус змінено → {updatedOrder?.Status}");

await orderService.ChangeStatusAsync(order2.Id, OrderStatus.Completed);
updatedOrder = await orderService.GetByIdAsync(order2.Id);
Console.WriteLine($"  Статус змінено → {updatedOrder?.Status}");

// ── Замовлення за userId (async) ──
Console.WriteLine($"\n--- Замовлення для {user3.Name} (async) ---");
var userOrders = await orderService.GetByUserIdAsync(user3.Id);
foreach (var o in userOrders)
    Console.WriteLine($"  {o}");

// ════════════════════════════════════════════════════════════════════
// ЧАСТИНА 3: ПОРІВНЯННЯ ПРОДУКТИВНОСТІ sync vs async
// ════════════════════════════════════════════════════════════════════
Console.WriteLine("\n════════════════════════════════════════");
Console.WriteLine("ЧАСТИНА 3: ПОРІВНЯННЯ sync vs async (5 паралельних записів)");
Console.WriteLine("════════════════════════════════════════\n");

// Sync — послідовне виконання
var sw = Stopwatch.StartNew();
for (int i = 1; i <= 5; i++)
{
    bookService.Add(new Book
    {
        Title  = $"Sync Book {i}",
        Author = "Test Author",
        Genre  = "Test",
        Price  = i * 10m
    });
}
sw.Stop();
Console.WriteLine($"  Sync   (5 записів послідовно):   {sw.ElapsedMilliseconds} мс");

// Async — паралельне виконання через Task.WhenAll
sw.Restart();
var asyncTasks = Enumerable.Range(1, 5).Select(i =>
    bookService.AddAsync(new Book
    {
        Title  = $"Async Book {i}",
        Author = "Test Author",
        Genre  = "Test",
        Price  = i * 10m
    }));
await Task.WhenAll(asyncTasks);
sw.Stop();
Console.WriteLine($"  Async  (5 записів Task.WhenAll): {sw.ElapsedMilliseconds} мс");

// ── Фінальний рахунок ──
Console.WriteLine("\n--- Фінальний стан сховища ---");
var finalUsers  = (await userService.GetAllAsync()).Count();
var finalBooks  = (await bookService.GetAllAsync()).Count();
var finalOrders = (await orderService.GetAllAsync()).Count();
Console.WriteLine($"  Users:  {finalUsers}");
Console.WriteLine($"  Books:  {finalBooks}");
Console.WriteLine($"  Orders: {finalOrders}");
Console.WriteLine($"\n  JSON файли збережено у: {dataDir}");
Console.WriteLine("    ├── users.json");
Console.WriteLine("    ├── books.json");
Console.WriteLine("    └── orders.json");

Console.WriteLine("\n========================================");
if (!Console.IsInputRedirected)
{
    Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
    Console.ReadKey();
}
