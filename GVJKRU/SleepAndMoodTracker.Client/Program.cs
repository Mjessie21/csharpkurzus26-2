using SleepAndMoodTracker.Core;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

IDailyLogManager tracker;
try
{
    tracker = TrackerFactory.Create();

}
catch (InvalidDataException exception)
{
    Console.WriteLine($"HIBA: {exception.Message}");
    return;
}

Console.WriteLine("--- Alvás- és hangulatnapló ---");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Válassz: [1] Új bejegyzés  [2] Összes bejegyzés  [3] Statisztika");
    Console.Write("input > ");
    string? menuChoice = Console.ReadLine();

    if(menuChoice is null)
    {
        break;
    }
    switch (menuChoice)
    {
        case "1":
            AddNewDailyLog(tracker);
            break;
        case "2":
            PrintAllLogs(tracker);
            break;
        case "3":
            PrintStatistics(tracker);
            break;
        default:
            Console.WriteLine("Ismeretlen menüpont, 1 és 3 közötti számot adj meg.");
            break;
    }
}

static void AddNewDailyLog(IDailyLogManager tracker)
{
    Console.Write("\nDátum (ÉÉÉÉ-HH-NN, üresen hagyva a mai nap) > ");
    string dateInput = Console.ReadLine() ?? "";

    Console.Write("Hány órát aludtál? > ");
    string sleepInput = Console.ReadLine() ?? "";

    Console.Write("Milyen a hangulatod (1-10)? > ");
    string moodInput = Console.ReadLine() ?? "";

    Console.Write("Jegyzet (opcionális) > ");
    string note = Console.ReadLine() ?? "";

    Either<DailyLog, Exception> result = tracker.AddDailyLog(dateInput, sleepInput, moodInput, note);

    if(result.TryGetSuccess(out DailyLog? savedDailyLog))
    {
        Console.WriteLine($"Sikeresen rögzítve: {FormatDailyLog(savedDailyLog)}");
    }
    else if(result.TryGetError(out Exception? exception))
    {
        Console.WriteLine($"HIBA: {exception.Message}");
    }
}

static void PrintAllLogs(IDailyLogManager tracker)
{
    IReadOnlyList<DailyLog> allLogs = tracker.GetAllLogs();

    if(allLogs.Count == 0)
    {
        Console.WriteLine("Még nincs egy bejegyzés sem.");
        return;
    }
    Console.WriteLine("\n-------------------------------------------------------------------");
    Console.WriteLine();
    foreach (DailyLog dailyLog in allLogs)
    {
        Console.WriteLine($"- {FormatDailyLog(dailyLog)}");
    }
    Console.WriteLine("\n-------------------------------------------------------------------");
}

static void PrintStatistics(IDailyLogManager tracker)
{
    DailyLog? bestDay = tracker.GetBestDay();

    if(bestDay is null)
    {
        Console.WriteLine("Még nincs adat a statisztikához");
        return;
    }

    Console.WriteLine("\n-------------------------------------------------------------------");

    Console.WriteLine($"\nÁtlagos hangulat: {tracker.GetAverageMood():F1}");

    Console.WriteLine($"\nLegjobb nap: {FormatDailyLog(bestDay)}");

    Console.WriteLine("\nÁtlagos hangulat alváshossz szerint:");
    
    foreach(KeyValuePair<SleepCategory,double> categoryAverage in tracker.GetAvarageMoodBySleepCategory())
    {
        Console.WriteLine($" {GetCategoryDisplayName(categoryAverage.Key)}: {categoryAverage.Value:F1}");
    }

    Console.WriteLine($"\nKialvatlan napok ({SleepCategoryClassifier.ShortSleepLimitHours} óránál kevesebb alvás):");
    foreach(DailyLog dailyLog in tracker.GetBadSleepDays())
    {
        Console.WriteLine($"  - {FormatDailyLog(dailyLog)}");
    }
    Console.WriteLine("\n-------------------------------------------------------------------");

}

static string FormatDailyLog(DailyLog dailyLog)
{
    string noteText = string.IsNullOrEmpty(dailyLog.Note) ? "" : $" ({dailyLog.Note})";
    return $"{dailyLog.Date:yyyy-MM-dd}: {dailyLog.SleepHours} óra alvás, hangulat {dailyLog.MoodScore}/10{noteText}";
}
static string GetCategoryDisplayName(SleepCategory sleepCategory)
{
    return sleepCategory switch
    {
        SleepCategory.Short => $"Rövid (<{SleepCategoryClassifier.ShortSleepLimitHours} óra)",
        SleepCategory.Normal => $"Normál ({SleepCategoryClassifier.ShortSleepLimitHours}-{SleepCategoryClassifier.LongSleepLimitHours} óra)",
        SleepCategory.Long => $"Hosszú (> {SleepCategoryClassifier.LongSleepLimitHours} óra)",
        _ => sleepCategory.ToString()
    };
}
