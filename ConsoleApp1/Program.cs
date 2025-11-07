using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

// Read JSON from app_data and deserialize into C# records with property mappings
var filename = "gp_p_01.json";
var file = Path.Combine(AppContext.BaseDirectory, "app_data", filename);

Console.WriteLine($"Looking for: {Path.GetFullPath(file)}");
if (!File.Exists(file))
{
    Console.WriteLine($"File not found: {Path.GetFullPath(file)}");
    return;
}

var json = File.ReadAllText(file);
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

RootData? root;
try
{
    root = JsonSerializer.Deserialize<RootData>(json, options);
}
catch (Exception ex)
{
    Console.WriteLine("Failed to parse JSON: " + ex.Message);
    return;
}

if (root == null)
{
    Console.WriteLine("No data parsed.");
    return;
}

Console.WriteLine($"Resource: {root.ResourceId}");
Console.WriteLine($"Total (metadata): {root.Total}");
Console.WriteLine($"Records parsed: {root.Records?.Count ?? 0}");
Console.WriteLine();

// Print first 20 records in a simple table
var records = root.Records ?? new List<Record>();
int show = Math.Min(20, records.Count);
if (show == 0)
{
    Console.WriteLine("No records to display.");
    return;
}

var headers = new[] { "storeno", "storename", "storeaddr", "contacttel", "taxno" };
var widths = new int[headers.Length];
for (int i = 0; i < headers.Length; i++) widths[i] = Math.Max(8, headers[i].Length + 1);
for (int i = 0; i < show; i++)
{
    var r = records[i];
    widths[0] = Math.Max(widths[0], (r.Storeno ?? "").Length + 1);
    widths[1] = Math.Max(widths[1], (r.Storename ?? "").Length + 1);
    widths[2] = Math.Max(widths[2], (r.Storeaddr ?? "").Length + 1);
    widths[3] = Math.Max(widths[3], (r.Contacttel ?? "").Length + 1);
    widths[4] = Math.Max(widths[4], (r.Taxno ?? "").Length + 1);
}

// header
for (int i = 0; i < headers.Length; i++) Console.Write(headers[i].PadRight(widths[i]));
Console.WriteLine();
Console.WriteLine(new string('-', widths.Sum()));

for (int i = 0; i < show; i++)
{
    var r = records[i];
    Console.Write((r.Storeno ?? "").PadRight(widths[0]));
    Console.Write((r.Storename ?? "").PadRight(widths[1]));
    Console.Write((r.Storeaddr ?? "").PadRight(widths[2]));
    Console.Write((r.Contacttel ?? "").PadRight(widths[3]));
    Console.Write((r.Taxno ?? "").PadRight(widths[4]));
    Console.WriteLine();
}

// Record definition with JsonPropertyName attributes matching the Chinese keys
public record MarketRecord(
    [property: JsonPropertyName("月別")] string 月別,
    [property: JsonPropertyName("台灣-加權指數")] string 台灣_加權指數,
    [property: JsonPropertyName("台灣-上櫃指數")] string 台灣_上櫃指數,
    [property: JsonPropertyName("美國-那斯達克指數")] string 美國_那斯達克指數,
    [property: JsonPropertyName("美國-道瓊工業指數")] string 美國_道瓊工業指數,
    [property: JsonPropertyName("日本-日經225指數")] string 日本_日經225指數,
    [property: JsonPropertyName("新加坡-海峽時報指數")] string 新加坡_海峽時報指數,
    [property: JsonPropertyName("南韓-綜合指數")] string 南韓_綜合指數,
    [property: JsonPropertyName("倫敦-金融時報指數")] string 倫敦_金融時報指數,
    [property: JsonPropertyName("中國-上海綜合指數")] string 中國_上海綜合指數,
    [property: JsonPropertyName("中國-香港恆生指數")] string 中國_香港恆生指數
)
{
    // helper to get values as array for printing
    public string[] ToArray() => new[] { 月別, 台灣_加權指數, 台灣_上櫃指數, 美國_那斯達克指數, 美國_道瓊工業指數, 日本_日經225指數, 新加坡_海峽時報指數, 南韓_綜合指數, 倫敦_金融時報指數, 中國_上海綜合指數, 中國_香港恆生指數 };
};
