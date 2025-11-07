using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"Working directory: {Directory.GetCurrentDirectory()}");

        var defaultPath = Path.Combine("app_data", "converted_file.json");

        if (args.Length > 0 && string.Equals(args[0], "beautify", StringComparison.OrdinalIgnoreCase))
        {
            string input = defaultPath;
            if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
            {
                input = args[1];
            }

            BeautifyJson(input);
            return;
        }

        var path = defaultPath;

        // If default file doesn't exist, try to find any .json under app_data and use the first one.
        if (!File.Exists(path))
        {
            var appDataDir = Path.Combine(Directory.GetCurrentDirectory(), "app_data");
            if (Directory.Exists(appDataDir))
            {
                var jsonFiles = Directory.GetFiles(appDataDir, "*.json");
                if (jsonFiles.Length > 0)
                {
                    path = jsonFiles[0];
                    Console.WriteLine($"Default file not found. Using first JSON in app_data: {path}");
                }
                else
                {
                    Console.WriteLine($"No JSON files found in: {appDataDir}");
                    return;
                }
            }
            else
            {
                Console.WriteLine($"File not found: {path}");
                return;
            }
        }

        string json = File.ReadAllText(path);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        List<Record>? records;
        try
        {
            records = JsonSerializer.Deserialize<List<Record>>(json, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserialize error: {ex.Message}");
            return;
        }

        if (records == null)
        {
            Console.WriteLine("No records parsed.");
            return;
        }

        Console.WriteLine($"Parsed records: {records.Count}");

        // 計算不同公司的數量（以 公司代號 判別）
        var uniqueCompanies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var rec in records)
        {
            if (!string.IsNullOrWhiteSpace(rec.公司代號))
                uniqueCompanies.Add(rec.公司代號.Trim());
        }
        Console.WriteLine($"Distinct companies (by 公司代號): {uniqueCompanies.Count}");

        for (int i = 0; i < records.Count; i++)
        {
            var r = records[i];
            Console.WriteLine($"[{i + 1}/{records.Count}] {r.公司代號} - {r.公司名稱} ({r.資料年月})");
            Console.WriteLine($"  產業別: {r.產業別}");
            Console.WriteLine($"  當月營收: {r.營業收入_當月營收}  上月營收: {r.營業收入_上月營收}  去年當月: {r.營業收入_去年當月營收}");
            Console.WriteLine($"  累計當月累計營收: {r.累計營業收入_當月累計營收}  去年累計: {r.累計營業收入_去年累計營收}");
            Console.WriteLine($"  備註: {r.備註}");
        }
    }

    private static void BeautifyJson(string inputPath)
    {
        try
        {
            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"Input file not found: {inputPath}");
                return;
            }

            var json = File.ReadAllText(inputPath);
            using var doc = JsonDocument.Parse(json);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var pretty = JsonSerializer.Serialize(doc.RootElement, options);

            var dir = Path.GetDirectoryName(inputPath) ?? "app_data";
            var name = Path.GetFileNameWithoutExtension(inputPath);
            var ext = Path.GetExtension(inputPath);
            var outName = name + "_pretty" + (string.IsNullOrEmpty(ext) ? ".json" : ext);
            var outPath = Path.Combine(dir, outName);

            File.WriteAllText(outPath, pretty);
            Console.WriteLine($"Beautified JSON written to: {outPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Beautify error: {ex.Message}");
        }
    }
}

public class Record
{
    [JsonPropertyName("出表日期")]
    public string? 出表日期 { get; set; }

    [JsonPropertyName("資料年月")]
    public string? 資料年月 { get; set; }

    [JsonPropertyName("公司代號")]
    public string? 公司代號 { get; set; }

    [JsonPropertyName("公司名稱")]
    public string? 公司名稱 { get; set; }

    [JsonPropertyName("產業別")]
    public string? 產業別 { get; set; }

    [JsonPropertyName("營業收入-當月營收")]
    public string? 營業收入_當月營收 { get; set; }

    [JsonPropertyName("營業收入-上月營收")]
    public string? 營業收入_上月營收 { get; set; }

    [JsonPropertyName("營業收入-去年當月營收")]
    public string? 營業收入_去年當月營收 { get; set; }

    [JsonPropertyName("營業收入-上月比較增減(%)")]
    public string? 營業收入_上月比較增減_pct { get; set; }

    [JsonPropertyName("營業收入-去年同月增減(%)")]
    public string? 營業收入_去年同月增減_pct { get; set; }

    [JsonPropertyName("累計營業收入-當月累計營收")]
    public string? 累計營業收入_當月累計營收 { get; set; }

    [JsonPropertyName("累計營業收入-去年累計營收")]
    public string? 累計營業收入_去年累計營收 { get; set; }

    [JsonPropertyName("累計營業收入-前期比較增減(%)")]
    public string? 累計營業收入_前期比較增減_pct { get; set; }

    [JsonPropertyName("備註")]
    public string? 備註 { get; set; }
}
