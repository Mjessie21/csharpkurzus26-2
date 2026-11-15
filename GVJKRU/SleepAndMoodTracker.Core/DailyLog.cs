using System;
using System.Collections.Generic;
using System.Text;

namespace SleepAndMoodTracker.Core;

public record class DailyLog(

   DateOnly Date,
   double SleepHours,
   int MoodScore,
   string Note

);

   

