using Cleverence;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Введите путь к входному лог-файлу (по умолчанию: logs.txt):");
        string inputFile = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(inputFile))
            inputFile = "logs.txt";

        Console.WriteLine("Введите путь к выходному файлу (по умолчанию: output.txt):");
        string outputFile = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(outputFile))
            outputFile = "output.txt";

        Console.WriteLine("Введите путь к файлу с проблемами (по умолчанию: problems.txt):");
        string problemsFile = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(problemsFile))
            problemsFile = "problems.txt";

       
        if (!File.Exists(inputFile))
        {
            File.WriteAllText(inputFile, "");
            Console.WriteLine($"Файл {inputFile} не найден. Создан пустой файл.");
            return;
        }

        var standardizer = new LogStandardizer(inputFile, outputFile, problemsFile);
        await standardizer.ProcessLogsAsync();

        Console.WriteLine("Обработка завершена. Отформатированные логи:");
        Console.WriteLine(File.ReadAllText(outputFile));
        Console.WriteLine("Введите строку для сжатия:");
        string input = Console.ReadLine();
        
        string compressed = Compression.Compress(input);
        Console.WriteLine($"Сжатая строка: {compressed}");



        Console.WriteLine("Запуск демонстрации параллельного доступа...");

        var tasks = new Task[8];

        // Читатели
        for (int i = 0; i < 5; i++)
        {
            int readerId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.WriteLine($"[{Time()}] Reader {readerId} хочет читать...");
                    Console.WriteLine($"[{Time()}] Reader {readerId} {Server.GetCount}");
                    Thread.Sleep(200); 
                }
            });
        }

        // Писатели
        for (int i = 5; i < 8; i++)
        {
            int writerId = i - 5;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.WriteLine($"[{Time()}] Writer {writerId} хочет писать...");
                    Server.AddToCount(1);
                    Thread.Sleep(1000); 
                }
            });
        }

        await Task.WhenAll(tasks);
        Console.WriteLine($"[{Time()}] Готово. Финальный count: {Server.GetCount()}");
    }

    private static string Time() => DateTime.Now.ToString("HH:mm:ss.fff");

}

