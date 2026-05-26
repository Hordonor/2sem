using System;
using System.Collections.Generic;

namespace lpr1
{
    // Завдання 1: Делегат MathOperation
    public delegate double MathOperation(double x, double y);

    // Завдання 2: Делегат NotificationHandler
    public delegate void NotificationHandler(string message);

    // Завдання 3: Делегат FilterPredicate
    public delegate bool FilterPredicate(int number);

    // Завдання 6: Делегат Validator
    public delegate bool Validator(string text);

    // Завдання 5: Клас Logger
    public class Logger
    {
        public Action<string>? LogHandler { get; set; }

        public void Log(string message)
        {
            LogHandler?.Invoke(message);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Встановлюємо UTF-8 кодування для коректного виведення кирилиці в консолі
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("========================================");
            Console.WriteLine("       ПРАКТИЧНА РОБОТА №1: ДЕЛЕГАТИ    ");
            Console.WriteLine("========================================\n");

            RunTask1();
            RunTask2();
            RunTask3();
            RunTask4();
            RunTask5();
            RunTask6();

            Console.WriteLine("========================================");
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
                Console.ReadKey();
            }
        }

        #region Завдання 1: Калькулятор (Базове розуміння)
        static void RunTask1()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 1: Калькулятор (Базове розуміння)");
            Console.WriteLine("----------------------------------------");

            MathOperation op;

            // Додавання
            op = Add;
            Console.WriteLine($"Add(10, 5)      = {op(10, 5)}");

            // Віднімання
            op = Subtract;
            Console.WriteLine($"Subtract(10, 5) = {op(10, 5)}");

            // Множення
            op = Multiply;
            Console.WriteLine($"Multiply(10, 5) = {op(10, 5)}");

            // Ділення
            op = Divide;
            Console.WriteLine($"Divide(10, 5)   = {op(10, 5)}");
            Console.WriteLine($"Divide(10, 0)   = {op(10, 0)} (перевірка ділення на нуль)");
            Console.WriteLine();
        }

        public static double Add(double a, double b) => a + b;
        public static double Subtract(double a, double b) => a - b;
        public static double Multiply(double a, double b) => a * b;
        public static double Divide(double a, double b) => b != 0 ? a / b : double.NaN;
        #endregion

        #region Завдання 2: Мультикастинг (Ланцюжок викликів)
        static void RunTask2()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 2: Мультикастинг (Ланцюжок викликів)");
            Console.WriteLine("----------------------------------------");

            // Створюємо екземпляр з першим методом
            NotificationHandler handler = SendEmail;
            
            // Додаємо другий метод
            handler += SendSMS;

            Console.WriteLine("Виклик комбінованого делегата:");
            handler("Лабораторну роботу виконано успішно!");
            Console.WriteLine();
        }

        public static void SendEmail(string message) => Console.WriteLine($"Email sent: [{message}]");
        public static void SendSMS(string message) => Console.WriteLine($"SMS sent: [{message}]");
        #endregion

        #region Завдання 3: Фільтрація списку (Делегат як параметр)
        static void RunTask3()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 3: Фільтрація списку (Делегат як параметр)");
            Console.WriteLine("----------------------------------------");

            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // 1. Пошук парних чисел
            Console.Write("Пошук парних чисел: ");
            FilterArray(numbers, IsEven);

            // 2. Пошук чисел більше 5
            Console.Write("Пошук чисел більше 5: ");
            FilterArray(numbers, IsGreaterThanFive);

            // 3. Додатково: Лямбда-вираз для пошуку непарних чисел
            Console.Write("Пошук непарних чисел (лямбда): ");
            FilterArray(numbers, n => n % 2 != 0);
            
            Console.WriteLine();
        }

        public static bool IsEven(int n) => n % 2 == 0;
        public static bool IsGreaterThanFive(int n) => n > 5;

        public static void FilterArray(int[] numbers, FilterPredicate predicate)
        {
            foreach (var number in numbers)
            {
                if (predicate(number))
                {
                    Console.Write($"{number} ");
                }
            }
            Console.WriteLine();
        }
        #endregion

        #region Завдання 4: Використання стандартних делегатів (Func та Action)
        static void RunTask4()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 4: Використання стандартних делегатів (Func та Action)");
            Console.WriteLine("----------------------------------------");

            Console.WriteLine("1. Калькулятор за допомогою Func<double, double, double>:");
            Func<double, double, double> mathFunc;

            mathFunc = Add;
            Console.WriteLine($"  Func Add(15, 3)      = {mathFunc(15, 3)}");
            mathFunc = Subtract;
            Console.WriteLine($"  Func Subtract(15, 3) = {mathFunc(15, 3)}");
            mathFunc = Multiply;
            Console.WriteLine($"  Func Multiply(15, 3) = {mathFunc(15, 3)}");
            mathFunc = Divide;
            Console.WriteLine($"  Func Divide(15, 3)   = {mathFunc(15, 3)}");

            Console.WriteLine("\n2. Фільтрація імен за допомогою Predicate<string> (FindAll):");
            List<string> students = new List<string> { "Андрій", "Богдан", "Аліна", "Василь", "Анна", "Дмитро", "Артем" };
            char letter = 'А';
            
            // Використання Predicate<string> через лямбда-вираз
            List<string> filteredStudents = students.FindAll(name => name.StartsWith(letter.ToString(), StringComparison.OrdinalIgnoreCase));
            
            Console.WriteLine($"Студенти, чиї імена починаються на літеру '{letter}':");
            foreach (var name in filteredStudents)
            {
                Console.WriteLine($"  - {name}");
            }
            Console.WriteLine();
        }
        #endregion

        #region Завдання 5: Логування (Практичний кейс)
        static void RunTask5()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 5: Логування (Практичний кейс)");
            Console.WriteLine("----------------------------------------");

            Logger logger = new Logger();

            // 1. Спочатку налаштовуємо виведення в консоль
            logger.LogHandler = message => Console.WriteLine($"[Console Logger]: {message}");
            logger.Log("Повідомлення успішно відправлено в обробку.");

            // 2. Змінюємо логування на льоту, щоб переводити текст у верхній регістр
            logger.LogHandler = message => Console.WriteLine($"[CONSOLE LOGGER UPPER]: {message.ToUpper()}");
            logger.Log("це повідомлення має бути перетворене на верхній регістр.");
            
            Console.WriteLine();
        }
        #endregion

        #region Завдання 6: Динамічний валідатор тексту
        static void RunTask6()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 6: Динамічний валідатор тексту");
            Console.WriteLine("----------------------------------------");

            // Створюємо два валідатори з різною конфігурацією довжини через замикання
            Validator passwordValidator = GetValidator(8);
            Validator loginValidator = GetValidator(3);

            string[] testLogins = { "admin", "lo", "usr" };
            string[] testPasswords = { "12345", "supersecure123", "qwerty" };

            Console.WriteLine("Перевірка логінів (мінімальна довжина 3):");
            foreach (var login in testLogins)
            {
                bool isValid = loginValidator(login);
                Console.WriteLine($"  Логін: \"{login,-10}\" -> {(isValid ? "VALID" : "INVALID (too short)")}");
            }

            Console.WriteLine("\nПеревірка паролів (мінімальна довжина 8):");
            foreach (var pwd in testPasswords)
            {
                bool isValid = passwordValidator(pwd);
                Console.WriteLine($"  Пароль: \"{pwd,-15}\" -> {(isValid ? "VALID" : "INVALID (too short)")}");
            }
            Console.WriteLine();
        }

        public static Validator GetValidator(int minLength)
        {
            // Повертаємо лямбда-вираз, який захоплює локальну змінну minLength (замикання)
            return text => text != null && text.Length >= minLength;
        }
        #endregion
    }
}
