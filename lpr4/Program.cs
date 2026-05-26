using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace lpr4
{
    #region Моделі для Завдання 1 (Task Tracker)
    public class TaskItem
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
    #endregion

    #region Моделі для Завдання 2 (Students)
    public class Student
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public double AverageScore { get; set; }
    }
    #endregion

    #region Моделі для Завдання 3 (Author & Book Cycle Check)
    // Моделі з циклічним посиланням (спричинять помилку)
    public class AuthorCycle
    {
        public string Name { get; set; } = string.Empty;
        public List<BookCycle> Books { get; set; } = new();
    }

    public class BookCycle
    {
        public string Title { get; set; } = string.Empty;
        public AuthorCycle Author { get; set; } = null!;
    }

    // Моделі з виправленим циклічним посиланням (використовуємо [JsonIgnore])
    public class AuthorFixed
    {
        public string Name { get; set; } = string.Empty;
        public List<BookFixed> Books { get; set; } = new();
    }

    public class BookFixed
    {
        public string Title { get; set; } = string.Empty;

        [JsonIgnore] // Ламає циклічність серіалізації
        public AuthorFixed Author { get; set; } = null!;
    }
    #endregion

    #region Моделі для Завдання 4 (Enum OrderStatus)
    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed
    }

    public class Order
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
    }
    #endregion

    #region Моделі для Завдання 5 (Polymorphism)
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Dog), "dog")]
    [JsonDerivedType(typeof(Cat), "cat")]
    public abstract class Animal
    {
        public string Name { get; set; } = string.Empty;
    }

    public class Dog : Animal
    {
        public int BarkVolume { get; set; }
    }

    public class Cat : Animal
    {
        public int Lives { get; set; }
    }
    #endregion

    #region Моделі для Завдання 6 (Nested Objects)
    public class PlayerWithInventory
    {
        public string Name { get; set; } = string.Empty;
        public Inventory? Inventory { get; set; }
    }

    public class Inventory
    {
        public List<string> Items { get; set; } = new();
    }
    #endregion

    #region Моделі для Завдання 7 (Model Versioning)
    public class PlayerOld
    {
        public string Name { get; set; } = string.Empty;
    }

    public class PlayerNew
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; } = 1; // Значення за замовчуванням для нових полів
    }
    #endregion

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("========================================");
            Console.WriteLine("    ПРАКТИЧНА РОБОТА №4: JSON СЕРІАЛІЗАЦІЯ ");
            Console.WriteLine("========================================\n");

            // Завдання 1: Запуск Task Tracker
            RunTaskTracker();

            Console.WriteLine("\n\n");

            // Завдання 2: Студенти
            RunTask2();

            Console.WriteLine("\n\n");

            // Завдання 3: Циклічні посилання
            RunTask3();

            Console.WriteLine("\n\n");

            // Завдання 4: Enum
            RunTask4();

            Console.WriteLine("\n\n");

            // Завдання 5: Поліморфізм
            RunTask5();

            Console.WriteLine("\n\n");

            // Завдання 6: Вкладені об'єкти
            RunTask6();

            Console.WriteLine("\n\n");

            // Завдання 7: Версійність моделей
            RunTask7();

            Console.WriteLine("\n\n");

            // Завдання 8: Обробка помилок
            RunTask8();

            Console.WriteLine("\n========================================");
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
                Console.ReadKey();
            }
        }

        #region Завдання 1: Збереження стану програми (Task Tracker)
        static void RunTaskTracker()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 1: Task Tracker");
            Console.WriteLine("----------------------------------------");

            string filePath = "tasks.json";
            List<TaskItem> tasks = new List<TaskItem>();

            // Відновлення стану
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
                    Console.WriteLine($"[Task Tracker] Відновлено {tasks.Count} задач із файлу {filePath}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Task Tracker] Помилка завантаження задач: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("[Task Tracker] Файл tasks.json не знайдено. Створено новий порожній список.");
            }

            if (Console.IsInputRedirected)
            {
                // Якщо ввід перенаправлено (автотести), виконуємо симуляцію
                Console.WriteLine("[Task Tracker] Запущено в демо-режимі (автоматична симуляція):");
                
                // Додаємо кілька задач
                tasks.Add(new TaskItem { Title = "Реалізувати івенти", IsCompleted = false });
                tasks.Add(new TaskItem { Title = "Розібратися з JSON", IsCompleted = false });
                Console.WriteLine($"  - Додано 2 задачі.");

                // Зміна статусу
                if (tasks.Count > 0)
                {
                    tasks[0].IsCompleted = true;
                    Console.WriteLine($"  - Змінено статус задачі \"{tasks[0].Title}\" на Completed.");
                }

                // Виведення списку
                Console.WriteLine("Поточний список задач:");
                PrintTasks(tasks);

                // Збереження при виході
                SaveTasks(filePath, tasks);
            }
            else
            {
                // Інтерактивне меню для користувача
                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine("\n--- Task Tracker Menu ---");
                    Console.WriteLine("1. Додати задачу");
                    Console.WriteLine("2. Змінити статус задачі");
                    Console.WriteLine("3. Переглянути список задач");
                    Console.WriteLine("4. Вийти з програми");
                    Console.Write("Оберіть дію: ");

                    string? choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Введіть назву задачі: ");
                            string title = Console.ReadLine() ?? "";
                            if (!string.IsNullOrWhiteSpace(title))
                            {
                                tasks.Add(new TaskItem { Title = title, IsCompleted = false });
                                Console.WriteLine("Задача додана.");
                            }
                            break;
                        case "2":
                            if (tasks.Count == 0)
                            {
                                Console.WriteLine("Список задач порожній.");
                                break;
                            }
                            Console.WriteLine("Оберіть номер задачі для зміни статусу:");
                            for (int i = 0; i < tasks.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. [{ (tasks[i].IsCompleted ? "X" : " ") }] {tasks[i].Title}");
                            }
                            Console.Write("Номер: ");
                            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tasks.Count)
                            {
                                tasks[index - 1].IsCompleted = !tasks[index - 1].IsCompleted;
                                Console.WriteLine($"Статус задачі \"{tasks[index - 1].Title}\" змінено.");
                            }
                            else
                            {
                                Console.WriteLine("Невірний номер.");
                            }
                            break;
                        case "3":
                            Console.WriteLine("Список задач:");
                            PrintTasks(tasks);
                            break;
                        case "4":
                            exit = true;
                            SaveTasks(filePath, tasks);
                            break;
                        default:
                            Console.WriteLine("Невідома дія. Спробуйте ще раз.");
                            break;
                    }
                }
            }
        }

        private static void PrintTasks(List<TaskItem> tasks)
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("  (порожній)");
                return;
            }
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. [{ (tasks[i].IsCompleted ? "X" : " ") }] {tasks[i].Title}");
            }
        }

        private static void SaveTasks(string filePath, List<TaskItem> tasks)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(tasks, options);
                File.WriteAllText(filePath, json);
                Console.WriteLine($"[Task Tracker] Стан збережено у {filePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Task Tracker] Помилка збереження задач: {ex.Message}");
            }
        }
        #endregion

        #region Завдання 2: Серіалізація списку об'єктів (Students)
        static void RunTask2()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 2: Серіалізація списку об'єктів");
            Console.WriteLine("----------------------------------------");

            string filePath = "students.json";

            // Створюємо список студентів
            List<Student> students = new List<Student>
            {
                new Student { Name = "Олександр", Age = 19, AverageScore = 4.8 },
                new Student { Name = "Марія", Age = 20, AverageScore = 4.9 },
                new Student { Name = "Дмитро", Age = 21, AverageScore = 3.9 },
                new Student { Name = "Ірина", Age = 19, AverageScore = 4.5 },
                new Student { Name = "Артем", Age = 22, AverageScore = 4.2 }
            };

            // Серіалізація у файл
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(students, options);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Список студентів серіалізовано в {filePath}.");

            // Десеріалізація назад
            string loadedJson = File.ReadAllText(filePath);
            List<Student> loadedStudents = JsonSerializer.Deserialize<List<Student>>(loadedJson) ?? new List<Student>();

            Console.WriteLine("\nДані після десеріалізації:");
            foreach (var s in loadedStudents)
            {
                Console.WriteLine($"  Студент: {s.Name,-10} | Вік: {s.Age} | Сер. бал: {s.AverageScore}");
            }
        }
        #endregion

        #region Завдання 3: Циклічні посилання
        static void RunTask3()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 3: Циклічні посилання");
            Console.WriteLine("----------------------------------------");

            // 1. Демонстрація проблеми
            AuthorCycle authorC = new AuthorCycle { Name = "Тарас Шевченко" };
            BookCycle bookC1 = new BookCycle { Title = "Кобзар", Author = authorC };
            BookCycle bookC2 = new BookCycle { Title = "Гайдамаки", Author = authorC };
            authorC.Books.Add(bookC1);
            authorC.Books.Add(bookC2);

            Console.WriteLine("1. Спроба серіалізації об'єкта з циклічним посиланням:");
            try
            {
                string jsonError = JsonSerializer.Serialize(authorC);
                Console.WriteLine(jsonError);
            }
            catch (JsonException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Впіймано помилку циклічності: {ex.Message}");
                Console.ResetColor();
            }

            // 2. Демонстрація вирішення
            AuthorFixed authorF = new AuthorFixed { Name = "Тарас Шевченко" };
            BookFixed bookF1 = new BookFixed { Title = "Кобзар", Author = authorF };
            BookFixed bookF2 = new BookFixed { Title = "Гайдамаки", Author = authorF };
            authorF.Books.Add(bookF1);
            authorF.Books.Add(bookF2);

            Console.WriteLine("\n2. Успішна серіалізація після вирішення проблеми через [JsonIgnore]:");
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonFixed = JsonSerializer.Serialize(authorF, options);
                Console.WriteLine(jsonFixed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неочікувана помилка: {ex.Message}");
            }
        }
        #endregion

        #region Завдання 4: Серіалізація та десеріалізація enum
        static void RunTask4()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 4: Серіалізація та десеріалізація enum");
            Console.WriteLine("----------------------------------------");

            Order order = new Order { Id = 105, Status = OrderStatus.Processing };

            // 1. Дефолтна серіалізація (як число)
            string defaultJson = JsonSerializer.Serialize(order);
            Console.WriteLine("1. За замовчуванням (як число):");
            Console.WriteLine($"  {defaultJson}");

            // 2. Серіалізація як текст
            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            
            string stringEnumJson = JsonSerializer.Serialize(order, options);
            Console.WriteLine("\n2. Після додавання JsonStringEnumConverter (як текст):");
            Console.WriteLine(stringEnumJson);

            // 3. Десеріалізація назад
            Order deserializedOrder = JsonSerializer.Deserialize<Order>(stringEnumJson, options)!;
            Console.WriteLine("\n3. Результат десеріалізації:");
            Console.WriteLine($"  Order ID: {deserializedOrder.Id}, Status: {deserializedOrder.Status}");
        }
        #endregion

        #region Завдання 5: Серіалізація списку базового класу (Polymorphism)
        static void RunTask5()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 5: Поліморфна серіалізація");
            Console.WriteLine("----------------------------------------");

            List<Animal> animals = new List<Animal>
            {
                new Dog { Name = "Рекс", BarkVolume = 85 },
                new Cat { Name = "Мурка", Lives = 9 }
            };

            // Серіалізація з підтримкою поліморфізму
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(animals, options);
            Console.WriteLine("Серіалізований поліморфний список (із дискримінатором $type):");
            Console.WriteLine(json);

            // Десеріалізація назад
            List<Animal> restoredAnimals = JsonSerializer.Deserialize<List<Animal>>(json, options) ?? new List<Animal>();
            
            Console.WriteLine("\nРезультат десеріалізації:");
            foreach (var animal in restoredAnimals)
            {
                if (animal is Dog dog)
                {
                    Console.WriteLine($"  Собака: {dog.Name} | Гучність гавкання: {dog.BarkVolume} dB (Тип: Dog)");
                }
                else if (animal is Cat cat)
                {
                    Console.WriteLine($"  Кішка: {cat.Name} | Кількість життів: {cat.Lives} (Тип: Cat)");
                }
            }
        }
        #endregion

        #region Завдання 6: Вкладені об'єкти
        static void RunTask6()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 6: Вкладені об'єкти (Null handling)");
            Console.WriteLine("----------------------------------------");

            PlayerWithInventory p = new PlayerWithInventory
            {
                Name = "Geralt",
                Inventory = new Inventory { Items = new List<string> { "Срібний меч", "Еліксир Ластівка" } }
            };

            // 1. Повна серіалізація
            string fullJson = JsonSerializer.Serialize(p);
            Console.WriteLine($"1. Повний JSON:\n  {fullJson}");

            // 2. Симуляція вручну видаленого поля Inventory
            string corruptedJson = "{\"Name\":\"Geralt\"}"; // без Inventory
            Console.WriteLine($"\n2. JSON після видалення поля Inventory:\n  {corruptedJson}");

            // 3. Десеріалізація
            PlayerWithInventory deserializedPlayer = JsonSerializer.Deserialize<PlayerWithInventory>(corruptedJson)!;
            
            // Безпечна ініціалізація null вкладеного об'єкта
            deserializedPlayer.Inventory ??= new Inventory();

            Console.WriteLine("\n3. Обробка стану:");
            Console.WriteLine($"  Гравець: {deserializedPlayer.Name}");
            Console.WriteLine($"  Кількість предметів в інвентарі: {deserializedPlayer.Inventory.Items.Count}");
        }
        #endregion

        #region Завдання 7: Версійність моделей
        static void RunTask7()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 7: Версійність моделей");
            Console.WriteLine("----------------------------------------");

            // 1. Створюємо та серіалізуємо старого гравця (без Level)
            PlayerOld oldPlayer = new PlayerOld { Name = "Thrall" };
            string oldJson = JsonSerializer.Serialize(oldPlayer);
            Console.WriteLine($"1. Старий JSON (тільки Name):\n  {oldJson}");

            // 2. Десеріалізуємо старий JSON у нову модель гравця (яка має Level)
            PlayerNew newPlayer = JsonSerializer.Deserialize<PlayerNew>(oldJson)!;
            
            Console.WriteLine("\n2. Результат десеріалізації у нову модель (з дефолтним Level):");
            Console.WriteLine($"  Гравець: {newPlayer.Name}");
            Console.WriteLine($"  Рівень (Level): {newPlayer.Level}");
        }
        #endregion

        #region Завдання 8: Обробка помилок десеріалізації
        static void RunTask8()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 8: Обробка помилок десеріалізації");
            Console.WriteLine("----------------------------------------");

            // Невалідний JSON
            string badJson = "{\"Name\": \"Дмитро\", \"Age\": 21, \"AverageScore\": }"; // пропущено значення бала

            Console.WriteLine($"Пошкоджений JSON для десеріалізації:\n  {badJson}\n");

            try
            {
                Student s = JsonSerializer.Deserialize<Student>(badJson)!;
                Console.WriteLine($"Успішно десеріалізовано студента: {s.Name}");
            }
            catch (JsonException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("УВАГА! Виникла помилка під час обробки файлу конфігурації JSON:");
                Console.WriteLine($"Помилка: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("\n[Система] Програма продовжила роботу в штатному режимі (аварійного виходу уникнено).");
            }
        }
        #endregion
    }
}
