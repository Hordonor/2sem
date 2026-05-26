using System.Text;

namespace mod1_pd25_task1;

public delegate string TextOperation(string text);

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string inputPath = Path.Combine(baseDir, "textPD25.txt");
        string outputPath = Path.Combine(baseDir, "resultPD25.txt");

        Console.WriteLine("========================================");
        Console.WriteLine("  МОДУЛЬНА РОБОТА №1 — ЗАВДАННЯ 1");
        Console.WriteLine("  ПД-25 Борушевський Роман");
        Console.WriteLine("========================================\n");

        if (File.Exists(outputPath))
            File.WriteAllText(outputPath, string.Empty, Encoding.UTF8);

        ProcessFile(inputPath, outputPath, ToUpperCase);
        ProcessFile(inputPath, outputPath, CountCharacters);
        ProcessFile(inputPath, outputPath, CountWords);

        Console.WriteLine("Обробку завершено. Результати записано у resultPD25.txt:\n");
        Console.WriteLine(File.ReadAllText(outputPath, Encoding.UTF8));

        if (!Console.IsInputRedirected)
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }

    static void ProcessFile(string inputPath, string outputPath, TextOperation operation)
    {
        foreach (string line in File.ReadAllLines(inputPath, Encoding.UTF8))
        {
            string result = operation(line);
            File.AppendAllText(outputPath, result + Environment.NewLine, Encoding.UTF8);
        }

        File.AppendAllText(outputPath, Environment.NewLine, Encoding.UTF8);
    }

    static string ToUpperCase(string text) => text.ToUpperInvariant();

    static string CountCharacters(string text) => text.Length.ToString();

    static string CountWords(string text) =>
        string.IsNullOrWhiteSpace(text)
            ? "0"
            : text.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries).Length.ToString();
}
