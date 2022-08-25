// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.FxData.Helpers;
using System.Diagnostics.CodeAnalysis;
using static System.DayOfWeek;

namespace SquidEyes.FxData.Models;

public struct TradeDate : IEquatable<TradeDate>, IComparable<TradeDate>
{
    static TradeDate()
    {
        MinDateOnly = new(2016, 1, 4);
        MaxDateOnly = new(2031, 12, 26);

        MinValue = From(MinDateOnly);
        MaxValue = From(MaxDateOnly);
    }

    public static TradeDate MinValue { get; }
    public static TradeDate MaxValue { get; }

    internal static DateOnly MinDateOnly { get; }
    internal static DateOnly MaxDateOnly { get; }

    internal DateOnly Value { get; }

    internal TradeDate(DateOnly value) => Value = value;

    public TradeDate ToNextTradeDate() =>
        GetThisOrNextTradeDate(Value.AddDays(1));

    public TradeDate ToThisOrNextTradeDate() =>
        GetThisOrNextTradeDate(Value);

    public TradeDate ToPrevTradeDate() =>
        GetThisOrPrevTradeDate(Value.AddDays(-1));

    public TradeDate ToThisOrPrevTradeDate() =>
        GetThisOrPrevTradeDate(Value);

    public int Month => Value.Month;

    public int Day => Value.Day;

    public int Year => Value.Year;

    public DayOfWeek DayOfWeek => Value.DayOfWeek;

    public DateOnly AsDateOnly() => Value;

    public DateTime ToDateTime(TimeOnly timeOnly, Market market)
    {
        return Value.ToDateTime(timeOnly);
    }

    public override string ToString() => Value.ToString("MM/dd/yyyy");

    public bool Equals(TradeDate other) => Value.Equals(other.Value);

    public override bool Equals([NotNullWhen(true)] object? other) =>
        other is TradeDate tradeDate && Equals(tradeDate);

    public override int GetHashCode() => Value.GetHashCode();

    public int CompareTo(TradeDate other) => Value.CompareTo(other.Value);

    public static TradeDate From(int year, int month, int day) =>
        From(new DateOnly(year, month, day));

    public static TradeDate From(DateOnly value)
    {
        if (!IsTradeDate(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        return new TradeDate(value);
    }

    public static TradeDate Parse(string value) =>
        From(DateOnly.Parse(value));

    public static bool TryParse(string value, out TradeDate tradeDate) =>
        Try.GetValue(() => Parse(value), out tradeDate);

    private static TradeDate GetThisOrNextTradeDate(DateOnly value)
    {
        while (value <= MaxDateOnly && !IsTradeDate(value))
            value = value.AddDays(1);

        if (value > MaxDateOnly)
            throw new ArgumentOutOfRangeException(nameof(value));

        return From(value);
    }

    private static TradeDate GetThisOrPrevTradeDate(DateOnly value)
    {
        while (value >= MinDateOnly && !IsTradeDate(value))
            value = value.AddDays(-1);

        if (value > MinDateOnly)
            throw new ArgumentOutOfRangeException(nameof(value));

        return From(value);
    }

    public static bool IsTradeDate(DateOnly value)
    {
        bool IsHoliday()
        {
            return (value.Month, value.Day, value.DayOfWeek) switch
            {
                (1, 1, Monday) => true,
                (1, 1, Tuesday) => true,
                (1, 1, Wednesday) => true,
                (1, 1, Thursday) => true,
                (1, 1, Friday) => true,
                (12, 25, Monday) => true,
                (12, 25, Tuesday) => true,
                (12, 25, Wednesday) => true,
                (12, 25, Thursday) => true,
                (12, 25, Friday) => true,
                _ => false,
            };
        }

        if (value < MinDateOnly || value > MaxDateOnly)
            return false;

        if (!value.IsWeekday())
            return false;

        if (IsHoliday())
            return false;

        return true;
    }

    public static bool operator ==(TradeDate lhs, TradeDate rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(TradeDate lhs, TradeDate rhs) =>
        !(lhs == rhs);

    public static bool operator <(TradeDate lhs, TradeDate rhs) =>
        lhs.CompareTo(rhs) < 0;

    public static bool operator <=(TradeDate lhs, TradeDate rhs) =>
        lhs.CompareTo(rhs) <= 0;

    public static bool operator >(TradeDate lhs, TradeDate rhs) =>
        lhs.CompareTo(rhs) > 0;

    public static bool operator >=(TradeDate lhs, TradeDate rhs) =>
        lhs.CompareTo(rhs) >= 0;
}