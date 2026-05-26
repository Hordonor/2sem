using System.Threading;

// ╔══════════════════════════════════════════════════════════════════╗
// ║         ПРАКТИЧНА РОБОТА №6: Потоки та обробка клавіш             ║
// ║         Основний потік — лічильник; фоновий — бінди клавіш       ║
// ╚══════════════════════════════════════════════════════════════════╝

Console.OutputEncoding = System.Text.Encoding.UTF8;

var state = new CounterState();

Console.WriteLine("========================================");
Console.WriteLine("  ПРАКТИЧНА РОБОТА №6: Потоки та бінди");
Console.WriteLine("========================================");
Console.WriteLine();
Console.WriteLine("Керування:");
Console.WriteLine("  P — пауза / продовження лічильника");
Console.WriteLine("  R — скинути лічильник до 0");
Console.WriteLine("  C — змінити колір тексту");
Console.WriteLine("  Q — завершити програму");
Console.WriteLine();

var keyThread = new Thread(() => KeyInputLoop(state))
{
    IsBackground = true,
    Name = "KeyInputThread"
};

keyThread.Start();

while (!state.ShouldExit)
{
    if (!state.IsPaused)
    {
        int value = state.IncrementCounter();
        state.WriteCounter(value);
    }

    Thread.Sleep(1000);
}

keyThread.Join(TimeSpan.FromSeconds(1));

Console.ResetColor();
Console.WriteLine("\nПрограму завершено.");

void KeyInputLoop(CounterState counterState)
{
    while (!counterState.ShouldExit)
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.P:
                    counterState.TogglePause();
                    break;
                case ConsoleKey.R:
                    counterState.ResetCounter();
                    break;
                case ConsoleKey.C:
                    counterState.ChangeColor();
                    break;
                case ConsoleKey.Q:
                    counterState.RequestExit();
                    return;
            }
        }

        Thread.Sleep(50);
    }
}

sealed class CounterState
{
    private readonly object _lock = new();

    private int _counter;
    private bool _isPaused;
    private bool _shouldExit;
    private int _colorIndex;

    private static readonly ConsoleColor[] Colors =
    [
        ConsoleColor.White,
        ConsoleColor.Green,
        ConsoleColor.Yellow,
        ConsoleColor.Cyan,
        ConsoleColor.Magenta,
        ConsoleColor.Red
    ];

    public bool IsPaused
    {
        get { lock (_lock) return _isPaused; }
    }

    public bool ShouldExit
    {
        get { lock (_lock) return _shouldExit; }
    }

    public int IncrementCounter()
    {
        lock (_lock)
        {
            return ++_counter;
        }
    }

    public void WriteCounter(int value)
    {
        lock (_lock)
        {
            Console.ForegroundColor = Colors[_colorIndex];
            Console.WriteLine($"Counter: {value}");
            Console.ResetColor();
        }
    }

    public void TogglePause()
    {
        lock (_lock)
        {
            _isPaused = !_isPaused;
            Console.WriteLine(_isPaused
                ? "[P] Лічильник призупинено."
                : "[P] Лічильник продовжено.");
        }
    }

    public void ResetCounter()
    {
        lock (_lock)
        {
            _counter = 0;
            Console.WriteLine("[R] Лічильник скинуто до 0.");
        }
    }

    public void ChangeColor()
    {
        lock (_lock)
        {
            _colorIndex = (_colorIndex + 1) % Colors.Length;
            Console.WriteLine($"[C] Колір змінено на {Colors[_colorIndex]}.");
        }
    }

    public void RequestExit()
    {
        lock (_lock)
        {
            _shouldExit = true;
            Console.WriteLine("[Q] Завершення програми...");
        }
    }
}
