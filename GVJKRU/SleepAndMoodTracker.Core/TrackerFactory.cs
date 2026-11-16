using System;
using System.Collections.Generic;
using System.Text;

namespace SleepAndMoodTracker.Core;

public static class TrackerFactory
{
    public const string DefaultFilePath = "dailyLogs.json";

    public static IDailyLogManager Create(string filePath = DefaultFilePath)
    {
        IDailyLogStorage storage = new JsonDailyLogStorage(filePath);
        return new DailyLogManager(storage);
    }
}
