// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;

namespace SquidEyes.FxData.Models;

public static class DateTimeExtenders
{
    public static DateTime ToDateTime(this TradeDate value) =>
        value.AsFunc(v => new DateTime(v.Year, v.Month, v.Day));

    internal static string ToDateTimeText(this DateTime value) =>
        value.ToString("MM/dd/yyyy HH:mm:ss.fff");

    internal static string ToDateText(this DateTime value) =>
        value.ToString("MM/dd/yyyy");

    internal static string ToDateText(this DateOnly value) =>
        value.ToString("MM/dd/yyyy");

    internal static string ToTimeText(this DateTime value, bool includeMilliseconds)
    {
        if (includeMilliseconds)
            return value.ToString("HH:mm:ss.fff");
        else
            return value.ToString("HH:mm:ss");
    }

    internal static string ToTimeSpanText(
        this TimeSpan value, bool daysOptional = true)
    {
        if (daysOptional && value < TimeSpan.FromDays(1))
            return value.ToString("hh\\:mm\\:ss\\.fff");
        else
            return value.ToString("d\\.hh\\:mm\\:ss\\.fff");
    }

    internal static DateTime AddHours(
        this DateTime value, int hours, bool minusOneMillisecond)
    {
        if (minusOneMillisecond)
            return value.AddHours(hours).AddMilliseconds(-1);
        else
            return value.AddHours(hours);
    }

    internal static bool IsWeekday(this DateOnly date) =>
        date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday;
}