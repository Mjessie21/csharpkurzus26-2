using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SleepAndMoodTracker.Core;

internal sealed class JsonDailyLogStorage(string filePath) : IDailyLogStorage
{

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };



    public List<DailyLog> Load()
    {
        if(!File.Exists(filePath))
        {
            return new List<DailyLog>();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<DailyLog>>(json, SerializerOptions) ?? new List<DailyLog>();
        }
        catch(JsonException exception)
        {
            throw new InvalidDataException($"A fájl nem olvasható: {filePath}", exception);
        }
    }



    public void Save(IEnumerable<DailyLog> dailyLogs)
    {
        string json = JsonSerializer.Serialize(dailyLogs, SerializerOptions);
        File.WriteAllText(filePath, json);
    }
}
