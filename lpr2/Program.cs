using System;

namespace lpr2
{
    #region Частина 1: Клімат-контроль (Патерн Observer)

    // Датчик температури (Суб'єкт/Джерело події)
    public class TemperatureSensor
    {
        private double _temperature;

        public double Temperature
        {
            get => _temperature;
            set
            {
                // Подія спрацьовує тільки якщо температура змінилася
                if (Math.Abs(_temperature - value) > 0.001)
                {
                    _temperature = value;
                    OnTemperatureChanged(_temperature);
                }
            }
        }

        // Подія зміни температури
        public event Action<double>? TemperatureChanged;

        protected virtual void OnTemperatureChanged(double temperature)
        {
            TemperatureChanged?.Invoke(temperature);
        }
    }

    // Спостерігач 1: Дисплей
    public class Display
    {
        public void OnTemperatureChanged(double temperature)
        {
            Console.WriteLine($"[Display] Поточна температура: {temperature:F1}°C");
        }
    }

    // Спостерігач 2: Кондиціонер
    public class AirConditioner
    {
        public void OnTemperatureChanged(double temperature)
        {
            if (temperature < 17)
            {
                Console.WriteLine("[Air Conditioner] Режим: УВІМКНЕНО ОБІГРІВ");
            }
            else if (temperature >= 17 && temperature <= 25)
            {
                Console.WriteLine("[Air Conditioner] Режим: КОНДИЦІОНЕР ВИМКНЕНО");
            }
            else // temperature > 25
            {
                Console.WriteLine("[Air Conditioner] Режим: УВІМКНЕНО ОХОЛОДЖЕННЯ");
            }
        }
    }

    // Спостерігач 3: Система безпеки
    public class SecuritySystem
    {
        public void OnTemperatureChanged(double temperature)
        {
            if (temperature > 40)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Security System] УВАГА! Повідомлення про перегрів! Температура: {temperature}°C");
                Console.ResetColor();
            }
            else if (temperature < 5)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[Security System] УВАГА! Попередження про ризик замерзання систем! Температура: {temperature}°C");
                Console.ResetColor();
            }
        }
    }

    #endregion

    #region Частина 2: Observer у GameDev (Player Damage)

    // Аргументи події отримання урону
    public class PlayerDamageEventArgs : EventArgs
    {
        public int DamageAmount { get; }
        public int CurrentHealth { get; }

        public PlayerDamageEventArgs(int damageAmount, int currentHealth)
        {
            DamageAmount = damageAmount;
            CurrentHealth = currentHealth;
        }
    }

    // Гравець (Суб'єкт/Джерело події)
    public class Player
    {
        public string Name { get; }
        public int Health { get; private set; } = 100;

        // Подія отримання урону
        public event EventHandler<PlayerDamageEventArgs>? DamageReceived;

        public Player(string name)
        {
            Name = name;
        }

        public void TakeDamage(int damage)
        {
            if (Health <= 0)
            {
                Console.WriteLine($"{Name} вже мертвий!");
                return;
            }

            Health -= damage;
            if (Health < 0) Health = 0;

            Console.WriteLine($"\n[Player] {Name} отримав урон: -{damage}. Поточне HP: {Health}");
            
            // Виклик події
            OnDamageReceived(new PlayerDamageEventArgs(damage, Health));
        }

        protected virtual void OnDamageReceived(PlayerDamageEventArgs e)
        {
            DamageReceived?.Invoke(this, e);
        }
    }

    // Спостерігач 1: Смуга здоров'я в інтерфейсі (UIHealthBar)
    public class UIHealthBar
    {
        public void OnDamageReceived(object? sender, PlayerDamageEventArgs e)
        {
            Console.Write("[UI Health Bar] Здоров'я: [");
            int barCount = e.CurrentHealth / 10;
            for (int i = 0; i < 10; i++)
            {
                if (i < barCount)
                    Console.Write("■");
                else
                    Console.Write(" ");
            }
            Console.WriteLine($"] {e.CurrentHealth}%");
        }
    }

    // Спостерігач 2: Звукова система (SoundSystem)
    public class SoundSystem
    {
        public void OnDamageReceived(object? sender, PlayerDamageEventArgs e)
        {
            Console.WriteLine("[Sound System] Грає звук: *Удар / Стогін болю*");
            
            if (e.CurrentHealth > 0 && e.CurrentHealth <= 20)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("[Sound System] Грає критичний звук: *Гучне серцебиття (Критичний стан)*");
                Console.ResetColor();
            }
        }
    }

    // Спостерігач 3: Система досягнень (AchievementSystem)
    public class AchievementSystem
    {
        private bool _unlockedHalfHealth = false;
        private bool _unlockedFirstDeath = false;

        public void OnDamageReceived(object? sender, PlayerDamageEventArgs e)
        {
            if (e.CurrentHealth <= 50 && !_unlockedHalfHealth)
            {
                _unlockedHalfHealth = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Achievement System] ДОСЯГНЕННЯ РОЗБЛОКОВАНО: \"Half Health\" (Здоров'я <= 50%)");
                Console.ResetColor();
            }

            if (e.CurrentHealth <= 0 && !_unlockedFirstDeath)
            {
                _unlockedFirstDeath = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Achievement System] ДОСЯГНЕННЯ РОЗБЛОКОВАНО: \"First Death\" (Перша смерть)");
                Console.ResetColor();
            }
        }
    }

    // Спостерігач 4: Логер гри (GameLogger)
    public class GameLogger
    {
        public void OnDamageReceived(object? sender, PlayerDamageEventArgs e)
        {
            Console.WriteLine($"[Game Logger] Запис: нанесено урон {e.DamageAmount}, HP гравця після удару: {e.CurrentHealth}");
        }
    }

    #endregion

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("========================================");
            Console.WriteLine("       ПРАКТИЧНА РОБОТА №2: OBSERVER    ");
            Console.WriteLine("========================================\n");

            // --- Тестування Частини 1: Клімат-контроль ---
            RunClimateControlSimulation();

            Console.WriteLine("\n\n");

            // --- Тестування Частини 2: GameDev ---
            RunGameSimulation();

            Console.WriteLine("\n========================================");
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
                Console.ReadKey();
            }
        }

        private static void RunClimateControlSimulation()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("ЧАСТИНА 1: СИСТЕМА КЛІМАТ-КОНТРОЛЮ");
            Console.WriteLine("========================================");

            // 1. Створення датчика
            TemperatureSensor sensor = new TemperatureSensor();

            // 2. Створення систем (Спостерігачів)
            Display display = new Display();
            AirConditioner airConditioner = new AirConditioner();
            SecuritySystem security = new SecuritySystem();

            // 3. Підписка на події
            sensor.TemperatureChanged += display.OnTemperatureChanged;
            sensor.TemperatureChanged += airConditioner.OnTemperatureChanged;
            sensor.TemperatureChanged += security.OnTemperatureChanged;

            // 4. Послідовно встановлюємо кілька температур для тестування
            Console.WriteLine("\n--- Встановлюємо 22°C (Комфортна температура) ---");
            sensor.Temperature = 22;

            Console.WriteLine("\n--- Встановлюємо 12°C (Холодно, кондиціонер має ввімкнути обігрів) ---");
            sensor.Temperature = 12;

            Console.WriteLine("\n--- Встановлюємо 28°C (Спекотно, має бути охолодження) ---");
            sensor.Temperature = 28;

            Console.WriteLine("\n--- Встановлюємо 45°C (Небезпека перегріву, має спрацювати безпека) ---");
            sensor.Temperature = 45;

            Console.WriteLine("\n--- Встановлюємо 3°C (Ризик замерзання та обігрів) ---");
            sensor.Temperature = 3;

            Console.WriteLine("\n--- Встановлюємо повторно 3°C (Подія не має спрацювати, бо температура не змінилась) ---");
            sensor.Temperature = 3;
        }

        private static void RunGameSimulation()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("ЧАСТИНА 2: ОБРОБКА УРОНУ ГРАВЦЕМ (GameDev)");
            Console.WriteLine("========================================");

            // 1. Створення гравця
            Player player = new Player("Arthur");

            // 2. Створення ігрових систем (Спостерігачів)
            UIHealthBar healthBar = new UIHealthBar();
            SoundSystem sound = new SoundSystem();
            AchievementSystem achievements = new AchievementSystem();
            GameLogger logger = new GameLogger();

            // 3. Підписка систем на подію отримання урону
            player.DamageReceived += healthBar.OnDamageReceived;
            player.DamageReceived += sound.OnDamageReceived;
            player.DamageReceived += achievements.OnDamageReceived;
            player.DamageReceived += logger.OnDamageReceived;

            // 4. Послідовно наносимо урон
            Console.WriteLine("\n--- Наносимо 15 урону ---");
            player.TakeDamage(15);

            Console.WriteLine("\n--- Наносимо 40 урону (HP має впасти до 45, досягнення Half Health) ---");
            player.TakeDamage(40);

            Console.WriteLine("\n--- Наносимо 30 урону (HP стає 15, критичний звук серцебиття) ---");
            player.TakeDamage(30);

            Console.WriteLine("\n--- Наносимо 20 урону (HP стає 0, смерть гравця) ---");
            player.TakeDamage(20);

            Console.WriteLine("\n--- Спроба нанести урон мертвому гравцю ---");
            player.TakeDamage(10);
        }
    }
}
