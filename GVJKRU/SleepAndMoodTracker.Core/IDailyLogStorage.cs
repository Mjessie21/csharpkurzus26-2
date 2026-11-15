using System;
using System.Collections.Generic;
using System.Text;

namespace SleepAndMoodTracker.Core;

internal interface IDailyLogStorage
{
    List<DailyLog> Load();

    void Save(IEnumerable<DailyLog> dailyLogs);
}
