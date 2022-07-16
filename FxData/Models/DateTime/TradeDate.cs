// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.FxData.Models;

public struct TradeDate : IEquatable<TradeDate>, IComparable<TradeDate>
{
    public TradeDate(int year, int month, int day)
        : this(new DateOnly(year, month, day))
    {
    }

    public TradeDate(DateOnly value)
    {
        if (!Known.IsTradeDate(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public DateOnly Value { get; }

    public TradeDate ToNextTradeDate() =>
        GetThisOrNextTradeDate(Value.AddDays(1));

    public TradeDate ToThisOrNextTradeDate() =>
        GetThisOrNextTradeDate(Value);

    public TradeDate ToPrevTradeDate() =>
        GetThisOrPrevTradeDate(Value.AddDays(-1));

    public TradeDate ToThisOrPrevTradeDate() =>
        GetThisOrPrevTradeDate(Value);

    public int CompareTo(TradeDate other) => Value.CompareTo(other.Value);

    public bool Equals(TradeDate other) => Value == other.Value;

    public override bool Equals(object? other) =>
        other is TradeDate tradeDate && Equals(tradeDate);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString("MM/dd/yyyy");

    private static TradeDate GetThisOrNextTradeDate(DateOnly value)
    {
        while (value <= Known.MaxTradeDate.Value && !Known.IsTradeDate(value))
            value = value.AddDays(1);

        if (value > Known.MaxTradeDate.Value)
            throw new ArgumentOutOfRangeException(nameof(value));

        return new TradeDate(value);
    }

    private static TradeDate GetThisOrPrevTradeDate(DateOnly value)
    {
        while (value >= Known.MinTradeDate.Value && !Known.IsTradeDate(value))
            value = value.AddDays(-1);

        if (value > Known.MinTradeDate.Value)
            throw new ArgumentOutOfRangeException(nameof(value));

        return new TradeDate(value);
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