using System;
using System.IO;
using System.Collections.Generic;

namespace lpr3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Налаштування UTF-8 для консолі
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length > 0)
            {
                // Якщо передано аргументи — запускаємо як CLI-аналізатор (Завдання 5)
                string targetPath = args[0];
                RunFileAnalyzerCLI(targetPath);
            }
            else
            {
                // Якщо запущено без аргументів — запускаємо повну демонстрацію всіх завдань
                Console.WriteLine("========================================");
                Console.WriteLine("    ПРАКТИЧНА РОБОТА №3: ФАЙЛОВА СИСТЕМА ");
                Console.WriteLine("========================================\n");

                SetupAndRunDemo();

                Console.WriteLine("========================================");
                if (!Console.IsInputRedirected)
                {
                    Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
                    Console.ReadKey();
                }
            }
        }

        #region Завдання 1: Аналізатор текстового файлу
        public static void RunTextAnalyzer(string inputPath, string outputPath)
        {
            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"Помилка: Файл {inputPath} не знайдено.");
                return;
            }

            int lineCount = 0;
            int wordCount = 0;
            int charCount = 0;

            // Читання за допомогою StreamReader
            using (StreamReader sr = new StreamReader(inputPath))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    lineCount++;
                    // Довжина самого рядка
                    charCount += line.Length;
                    // Оскільки ReadLine() відкидає символ переходу рядка, додаємо 1 для \n
                    charCount += 1;

                    // Поділ на слова
                    string[] words = line.Split(new char[] { ' ', '\t', '.', ',', ';', ':', '!', '?', '-', '(', ')', '"', '\'', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    wordCount += words.Length;
                }
            }

            // Запис результату у report.txt
            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                sw.WriteLine($"кількість рядків: {lineCount}");
                sw.WriteLine($"кількість слів: {wordCount}");
                sw.WriteLine($"кількість символів: {charCount}");
            }
        }
        #endregion

        #region Завдання 2: Інспектор папки
        public static void InspectFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Помилка: Папка {folderPath} не існує.");
                return;
            }

            Console.WriteLine($"Вміст папки: {folderPath}");

            // Виводимо всі підпапки
            string[] directories = Directory.GetDirectories(folderPath);
            Console.WriteLine($"Підпапки ({directories.Length}):");
            foreach (var dir in directories)
            {
                Console.WriteLine($"  [Folder] {Path.GetFileName(dir)}");
            }

            // Виводимо всі файли
            string[] files = Directory.GetFiles(folderPath);
            Console.WriteLine($"\nФайли ({files.Length}):");
            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                Console.WriteLine($"  [File] {fi.Name,-25} | Розмір: {FormatSize(fi.Length),-10} | Створено: {fi.CreationTime}");
            }
        }
        #endregion

        #region Завдання 3: Пошук найбільшого файлу
        public static void FindLargestFile(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Помилка: Папка {folderPath} не існує.");
                return;
            }

            try
            {
                // Отримуємо всі файли включно з підпапками
                string[] allFiles = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                FileInfo? largestFile = null;

                foreach (var file in allFiles)
                {
                    FileInfo fi = new FileInfo(file);
                    if (largestFile == null || fi.Length > largestFile.Length)
                    {
                        largestFile = fi;
                    }
                }

                if (largestFile != null)
                {
                    Console.WriteLine($"Name: {largestFile.Name}");
                    Console.WriteLine($"Size: {FormatSize(largestFile.Length)}");
                    Console.WriteLine($"Path: {largestFile.FullName}");
                }
                else
                {
                    Console.WriteLine("У папці немає файлів.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при пошуку найбільшого файлу: {ex.Message}");
            }
        }
        #endregion

        #region Завдання 4: Очищення кешу
        // 1. Варіант з рекурсією
        public static void CleanCacheRecursive(string currentDir, ref int deletedCount, ref long totalSize)
        {
            // Видаляємо всі файли в поточній папці
            foreach (var file in Directory.GetFiles(currentDir))
            {
                try
                {
                    FileInfo fi = new FileInfo(file);
                    long size = fi.Length;
                    File.Delete(file);
                    deletedCount++;
                    totalSize += size;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Рекурсія] Помилка видалення файлу {file}: {ex.Message}");
                }
            }

            // Рекурсивний обхід підпапок
            foreach (var dir in Directory.GetDirectories(currentDir))
            {
                CleanCacheRecursive(dir, ref deletedCount, ref totalSize);
            }
        }

        // 2. Варіант БЕЗ рекурсії (використовуємо чергу)
        public static void CleanCacheIterative(string startDir, out int deletedCount, out long totalSize)
        {
            deletedCount = 0;
            totalSize = 0;

            Queue<string> dirQueue = new Queue<string>();
            dirQueue.Enqueue(startDir);

            while (dirQueue.Count > 0)
            {
                string currentDir = dirQueue.Dequeue();

                // Видаляємо файли в поточній папці
                foreach (var file in Directory.GetFiles(currentDir))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        long size = fi.Length;
                        File.Delete(file);
                        deletedCount++;
                        totalSize += size;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Ітеративно] Помилка видалення файлу {file}: {ex.Message}");
                    }
                }

                // Додаємо підпапки в чергу
                foreach (var dir in Directory.GetDirectories(currentDir))
                {
                    dirQueue.Enqueue(dir);
                }
            }
        }
        #endregion

        #region Завдання 5: File Analyzer CLI
        public static void RunFileAnalyzerCLI(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Помилка: Папка {folderPath} не існує.");
                return;
            }

            int foldersCount = 0;
            int filesCount = 0;
            long totalSize = 0;
            string largestFileName = "Немає";
            long largestFileSize = -1;

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(folderPath);

            while (queue.Count > 0)
            {
                string currentDir = queue.Dequeue();

                try
                {
                    // Підрахунок файлів
                    foreach (var file in Directory.GetFiles(currentDir))
                    {
                        filesCount++;
                        FileInfo fi = new FileInfo(file);
                        totalSize += fi.Length;

                        if (fi.Length > largestFileSize)
                        {
                            largestFileSize = fi.Length;
                            largestFileName = fi.Name;
                        }
                    }

                    // Підрахунок папок та додавання в чергу
                    foreach (var dir in Directory.GetDirectories(currentDir))
                    {
                        foldersCount++;
                        queue.Enqueue(dir);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка доступу до {currentDir}: {ex.Message}");
                }
            }

            // Виведення результату
            Console.WriteLine($"Folders: {foldersCount}");
            Console.WriteLine($"Files: {filesCount}");
            Console.WriteLine($"Total size: {FormatSize(totalSize)}");
            Console.WriteLine($"Largest file: {largestFileName}");
        }
        #endregion

        #region Допоміжні методи та Демо-режим

        // Форматування розміру файлу
        public static string FormatSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
        }

        // Налаштування та запуск автоматичної демонстрації завдань
        private static void SetupAndRunDemo()
        {
            string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_test_dir");

            // Очищення перед створенням
            if (Directory.Exists(tempDir))
            {
                try
                {
                    Directory.Delete(tempDir, true);
                }
                catch { }
            }

            Directory.CreateDirectory(tempDir);

            // Створення тестової структури папок
            string docDir = Path.Combine(tempDir, "Documents");
            string mediaDir = Path.Combine(tempDir, "Media");
            string cacheDir = Path.Combine(tempDir, "Cache");
            string subCacheDir = Path.Combine(cacheDir, "SubCache");

            Directory.CreateDirectory(docDir);
            Directory.CreateDirectory(mediaDir);
            Directory.CreateDirectory(cacheDir);
            Directory.CreateDirectory(subCacheDir);

            // 1. Створення story.txt для Завдання 1
            string storyPath = Path.Combine(tempDir, "story.txt");
            string storyContent =
                "Once upon a time in a far away land,\n" +
                "there was a C# programmer who loved streams.\n" +
                "Reading files was his favourite thing to do!\n" +
                "This is the end of the short story.";
            File.WriteAllText(storyPath, storyContent);

            // 2. Файли в Documents
            File.WriteAllText(Path.Combine(docDir, "notes.txt"), "Important notes about the project.");
            File.WriteAllText(Path.Combine(docDir, "todo.md"), "1. Learn C#\n2. Make a Git repo\n3. Complete labs");

            // 3. Файли в Media
            File.WriteAllBytes(Path.Combine(mediaDir, "photo.jpg"), new byte[45000]); // 45 KB
            File.WriteAllBytes(Path.Combine(mediaDir, "video.mp4"), new byte[2500000]); // ~2.5 MB (Найбільший)

            // 4. Тимчасові файли в Cache
            File.WriteAllBytes(Path.Combine(cacheDir, "session_1.tmp"), new byte[150000]); // 150 KB
            File.WriteAllBytes(Path.Combine(cacheDir, "session_2.tmp"), new byte[250000]); // 250 KB
            File.WriteAllBytes(Path.Combine(subCacheDir, "old_cache.bin"), new byte[400000]); // 400 KB

            Console.WriteLine("========================================");
            Console.WriteLine("   СТВОРЕНО ТИМЧАСОВУ ТЕСТОВУ СТРУКТУРУ  ");
            Console.WriteLine("========================================\n");

            // --- Завдання 1 ---
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 1: Аналізатор текстового файлу");
            Console.WriteLine("----------------------------------------");
            string reportPath = Path.Combine(tempDir, "report.txt");
            RunTextAnalyzer(storyPath, reportPath);
            Console.WriteLine($"Аналіз виконано для {Path.GetFileName(storyPath)}");
            Console.WriteLine($"Результати записано у {Path.GetFileName(reportPath)}:");
            Console.WriteLine(File.ReadAllText(reportPath));

            // --- Завдання 2 ---
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 2: Інспектор папки");
            Console.WriteLine("----------------------------------------");
            InspectFolder(tempDir);
            Console.WriteLine();

            // --- Завдання 3 ---
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 3: Пошук найбільшого файлу");
            Console.WriteLine("----------------------------------------");
            FindLargestFile(tempDir);
            Console.WriteLine();

            // --- Завдання 4 ---
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 4: Очищення кешу");
            Console.WriteLine("----------------------------------------");
            
            // Рекурсивна чистка підпапки SubCache
            Console.WriteLine("1. Рекурсивне очищення папки SubCache:");
            int recCount = 0;
            long recSize = 0;
            CleanCacheRecursive(subCacheDir, ref recCount, ref recSize);
            Console.WriteLine($"[Рекурсія] Видалено файлів: {recCount}, сумарний розмір: {FormatSize(recSize)}");

            // Ітеративна чистка папки Cache
            Console.WriteLine("\n2. Ітеративне (без рекурсії) очищення головної папки Cache:");
            int iterCount = 0;
            long iterSize = 0;
            CleanCacheIterative(cacheDir, out iterCount, out iterSize);
            Console.WriteLine($"[Ітеративно] Видалено файлів: {iterCount}, сумарний розмір: {FormatSize(iterSize)}");
            Console.WriteLine();

            // --- Завдання 5 ---
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Завдання 5: File Analyzer CLI (Демо)");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Запуск аналізу для папки: {tempDir}");
            RunFileAnalyzerCLI(tempDir);
            Console.WriteLine();

            // Очищення за собою
            try
            {
                Directory.Delete(tempDir, true);
                Console.WriteLine("Тестову папку успішно видалено після демонстрації.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не вдалося видалити тестову папку: {ex.Message}");
            }
        }

        #endregion
    }
}
