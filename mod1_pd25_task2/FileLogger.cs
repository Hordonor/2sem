using System.Text;

namespace mod1_pd25_task2;

public class FileLogger
{
    private readonly string _logFilePath;

    public FileLogger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public void Subscribe(MessagePublisher publisher)
    {
        publisher.MessageSent += OnMessageSent;
    }

    private void OnMessageSent(string message)
    {
        string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        File.AppendAllText(_logFilePath, entry + Environment.NewLine, Encoding.UTF8);
        Console.WriteLine($"Записано у logPD25.txt: {entry}");
    }
}
