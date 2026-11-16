using System;
using System.Collections.Generic;
using System.Text;

namespace SleepAndMoodTracker.Core;

public interface IDailyLogManager
{
    Either<DailyLog, Exception> AddDailyLog(string dateInput, string sleepInput, string moodInput, string note);
    IReadOnlyList<DailyLog> GetAllLogs();
}
