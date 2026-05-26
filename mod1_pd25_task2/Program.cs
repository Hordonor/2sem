using System.Text;

namespace mod1_pd25_task2;

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logPD25.txt");

        Console.WriteLine("========================================");
        Console.WriteLine("  МОДУЛЬНА РОБОТА №1 — ЗАВДАННЯ 2");
        Console.WriteLine("  ПД-25 Борушевський Роман");
        Console.WriteLine("========================================\n");
        Console.WriteLine("Введіть 4 рядки тексту:\n");

        File.WriteAllText(logPath, string.Empty, Encoding.UTF8);

        var publisher = new MessagePublisher();
        var logger = new FileLogger(logPath);
        logger.Subscribe(publisher);

        for (int i = 1; i <= 4; i++)
        {
            Console.Write($"Рядок {i}: ");
            string? message = Console.ReadLine() ?? string.Empty;
            publisher.Send(message);
        }

        Console.WriteLine("\nУсі повідомлення збережено у logPD25.txt:\n");
        Console.WriteLine(File.ReadAllText(logPath, Encoding.UTF8));

        if (!Console.IsInputRedirected)
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
