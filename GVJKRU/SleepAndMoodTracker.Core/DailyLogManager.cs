using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace SleepAndMoodTracker.Core;

internal sealed class DailyLogManager : IDailyLogManager
{
    private const string DateFormat = "yyyy-MM-dd";
    private const double MinimumSleepHours = 0;
    private const double MaximumSleepHours = 24;
    private const int MinimumMoodScore = 1;
    private const int MaximumMoodScore = 10;

    private readonly IDailyLogStorage _storage;
    private readonly List<DailyLog> _dailyLogs;

    public DailyLogManager(IDailyLogStorage storage)
    {
        _storage = storage;
        _dailyLogs = _storage.Load();
    }


    public Either<DailyLog, Exception> AddDailyLog(string dateInput, string sleepInput, string moodInput, string note)
    {
        try
        {
            DateOnly date = ParseDate(dateInput);
            double sleepHours = ParseSleepHours(sleepInput);
            int moodScore = ParseMoodScore(moodInput);

            if (_dailyLogs.Any(dailyLog => dailyLog.Date == date))
            {
                return new InvalidOperationException($"Erre a napra ({date.ToString(DateFormat)}) már van bejegyzés.");
            }
            DailyLog newDailyLog = new DailyLog(date, sleepHours, moodScore, note.Trim());

            _storage.Save(_dailyLogs.Append(newDailyLog));
            _dailyLogs.Add(newDailyLog);

            return newDailyLog;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    public IReadOnlyList<DailyLog> GetAllLogs()
    {
        return _dailyLogs.OrderBy(dailyLog => dailyLog.Date).ToList();
    }


    public IEnumerable<DailyLog> GetBadSleepDays()
    {
        return _dailyLogs
            .Where(dailyLog => SleepCategoryClassifier.Classify(dailyLog.SleepHours) == SleepCategory.Short)
            .OrderBy(dailyLog => dailyLog.Date);
    }

    public double GetAvarageMood()
    {
        return _dailyLogs.Average(dailyLog => dailyLog.MoodScore);
    }

    public DailyLog? GetBestDay()
    {
        return _dailyLogs
            .OrderByDescending(dailyLog => dailyLog.MoodScore)
            .ThenByDescending(dailyLog => dailyLog.SleepHours)
            .FirstOrDefault();
    }

    public Dictionary<SleepCategory, double> GetAvarageMoodBySleepCategory()
    {
        return _dailyLogs
            .GroupBy(dailyLog => SleepCategoryClassifier.Classify(dailyLog.SleepHours))
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key,
                group => group.Average(dailyLog => dailyLog.MoodScore)
            );
    }










    private static DateOnly ParseDate(string dateInput)
    {
        if (string.IsNullOrWhiteSpace(dateInput))
        {
            return DateOnly.FromDateTime(DateTime.Now);
        }

        if (!DateOnly.TryParseExact(dateInput.Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
        {
            throw new FormatException("A dátum formátuma nem megfelelő. Add meg ilyen formátumban: ÉÉÉÉ - HH - NN!");
        }

        if (date > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ArgumentException("Jövőbeli dátumra nem lehet bejegyzést írni!");
        }

        return date;
    }


    private static double ParseSleepHours(string sleepInput)
    {
        string cleanedInput = sleepInput.Trim().Replace(",", ".");

        if(!double.TryParse(cleanedInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double sleepHours))
        {
            throw new FormatException("Az alvásidő formátuma nem megfelelő. Add meg óraszámként!");
        }

        if(sleepHours < MinimumSleepHours || sleepHours > MaximumSleepHours)
        {
            throw new ArgumentException($"Az alvás óraszáma {MinimumSleepHours} és {MaximumSleepHours} között legyen!");
        }

        return sleepHours;
    }


    private static int ParseMoodScore(string moodInput)
    {
        if(!int.TryParse(moodInput.Trim(), out int moodScore))
        {
            throw new FormatException("Az hangulat formátuma nem megfelelő. Add meg egész számként!");
        }

        if(moodScore < MinimumMoodScore || moodScore > MaximumMoodScore)
        {
            throw new ArgumentException($"A hangulat {MinimumMoodScore} és {MaximumMoodScore} között legyen!");
        }

        return moodScore;
    }

    
}


