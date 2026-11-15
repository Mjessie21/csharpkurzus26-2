using System;
using System.Collections.Generic;
using System.Text;

namespace SleepAndMoodTracker.Core;

public enum SleepCategory
{
    Short,
    Normal,
    Long
}


public static class SleepCategoryClassifier
{
    public const double ShortSleepLimitHours = 6;

    public const double LongSleepLimitHours = 9;

    public static SleepCategory Classify(double sleepHours)
    {
        if (sleepHours < ShortSleepLimitHours)
        {
            return SleepCategory.Short;
        }

        if (sleepHours > LongSleepLimitHours)
        {
            return SleepCategory.Long;
        }

        return SleepCategory.Normal;
    }
}
