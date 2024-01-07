// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Fundamentals;

namespace SquidEyes.FxData.Models;

public class Session : IEquatable<Session>
{
    public Session(TradeDate tradeDate, Market market)
    {
        TradeDate = tradeDate.MayNotBe().Default();

        Market = market.MustBe().EnumValue();

        var (minDateTime, maxDateTime) =
            DateTimeHelper.GetMinAndMaxDateTimes(tradeDate, market);

        MinTickOn = new TickOn(minDateTime);
        MaxTickOn = new TickOn(maxDateTime);
    }

    public TradeDate TradeDate { get; }
    public Market Market { get; }
    public TickOn MinTickOn { get; }
    public TickOn MaxTickOn { get; }

    public bool Equals(Session? other)
    {
        if (other is null)
            return false;

        return (TradeDate == other.TradeDate)
            && (Market == other.Market);
    }

    public override bool Equals(object? other) =>
        other is Session session && Equals(session);

    public override int GetHashCode() =>
        HashCode.Combine(TradeDate, Market);

    public bool InSession(DateTime value)
    {
        if (value.Kind != DateTimeKind.Unspecified)
            throw new ArgumentOutOfRangeException(nameof(value));

        return value >= MinTickOn.Value && value <= MaxTickOn.Value;
    }

    public override string ToString()
    {
        var min = MinTickOn.Value.ToTimeText(true);
        var max = MaxTickOn.Value.ToTimeText(true);

        return $"{TradeDate} ({Market}: {min} to {max})";
    }

    public static bool operator ==(Session lhs, Session rhs)
    {
        if (lhs is null)
            return rhs is null;

        return lhs.Equals(rhs);
    }

    public static bool operator !=(Session lhs, Session rhs) =>
        !(lhs == rhs);
}