using System.Globalization;
using System.Text.RegularExpressions;

public class LogStandardizer
{
    private readonly string _inputPath;
    private readonly string _outputPath;
    private readonly string _problemsPath;

    
    public LogStandardizer(string inputPath, string outputPath, string problemsPath)
    {
        _inputPath = inputPath;
        _outputPath = outputPath;
        _problemsPath = problemsPath;
    }

    public async Task ProcessLogsAsync()
    {
        if (!File.Exists(_inputPath))
        {
            Console.WriteLine("Входной файл не найден.");
            return;
        }

        var lines = await File.ReadAllLinesAsync(_inputPath);
        var outputLines = new List<string>();
        var problemLines = new List<string>();

        foreach (var line in lines)
        {
            string? parsed = ParseLine(line);

            if (parsed is null)
                problemLines.Add(line);
            else
                outputLines.Add(parsed);
        }

        await File.WriteAllLinesAsync(_outputPath, outputLines);
        await File.WriteAllLinesAsync(_problemsPath, problemLines);
    }

    private string? ParseLine(string line)
    {
        if (Format1.IsMatch(line))
        {
            var match = Format1.Match(line);
            string date = ConvertDate(match.Groups["date"].Value);
            string time = match.Groups["time"].Value;
            string level = NormalizeLevel(match.Groups["level"].Value);

            string method = match.Groups["method"].Success && !string.IsNullOrWhiteSpace(match.Groups["method"].Value)
                ? match.Groups["method"].Value
                : "DEFAULT";

            string message = match.Groups["message"].Value;

            return $"{date}\t{time}\t{level}\t{method}\t{message}";
        }
        else if (Format2.IsMatch(line))
        {
            var match = Format2.Match(line);
            string date = ConvertDate(match.Groups["date"].Value);
            string time = match.Groups["time"].Value;
            string level = NormalizeLevel(match.Groups["level"].Value);

            string method = match.Groups["method"].Success && !string.IsNullOrWhiteSpace(match.Groups["method"].Value)
                ? match.Groups["method"].Value
                : "DEFAULT";

            string message = match.Groups["message"].Value;

            return $"{date}\t{time}\t{level}\t{method}\t{message}";
        }


        return null;
    }


    private static readonly Regex Format1 = new Regex(
    @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{3})\s+(?<level>[A-Z]+)(?:\s+(?<method>[A-Za-z0-9_.]+))?\s+(?<message>.+)$",
    RegexOptions.Compiled);

    private static readonly Regex Format2 = new Regex(
    @"^(?<date>\d{4}-\d{2}-\d{2}) (?<time>\d{2}:\d{2}:\d{2}\.\d+)\|\s*(?<level>[A-Z]+)\|\d+\|\s*(?:(?<method>[\w\.]+)\|\s*)?(?<message>.+)$",
    RegexOptions.Compiled);



    private string ConvertDate(string input)
    {
        if (DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1))
            return dt1.ToString("yyyy-MM-dd");

        if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt2))
            return dt2.ToString("yyyy-MM-dd");

        return input;
    }

    private string NormalizeLevel(string level)
    {
        return level switch
        {
            "INFORMATION" => "INFO",
            "INFO" => "INFO",
            "WARNING" => "WARN",
            "WARN" => "WARN",
            "ERROR" => "ERROR",
            "DEBUG" => "DEBUG",
            _ => level
        };
    }
}
