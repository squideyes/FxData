// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;

namespace SquidEyes.FxData.Context;

public class Session
{
    public Session(TradeDate tradeDate, Market market)
    {
        TradeDate = tradeDate.Validated(nameof(tradeDate), v => v != default);

        Market = market.Validated(nameof(market), v => v.IsEnumValue());

        (TickOn, TickOn) GetMinMax(int start, int hours)
        {
            var minTickOn = new TickOn(tradeDate.Value.ToDateTime(
                new TimeOnly(start, 0), DateTimeKind.Utc).ToEasternFromUtc());

            return (minTickOn, new TickOn(minTickOn.Value.AddHours(hours, true)));
        }

        (MinTickOn, MaxTickOn) = market switch
        {
            Market.NewYork => GetMinMax(13, 9),
            Market.London => GetMinMax(8, 9),
            Market.Combined => GetMinMax(8, 14),
            _ => throw new ArgumentNullException(nameof(market))
        };
    }

    public TradeDate TradeDate { get; }
    public Market Market { get; }
    public TickOn MinTickOn { get; }
    public TickOn MaxTickOn { get; }

    public bool InSession(DateTime value)
    {
        if (value.Kind != DateTimeKind.Unspecified)
            throw new ArgumentOutOfRangeException(nameof(value));

        return value >= MinTickOn.Value && value <= MaxTickOn.Value;
    }

    public bool InSession(TickOn tickOn)
    {
        if (tickOn == default)
            throw new ArgumentOutOfRangeException(nameof(tickOn));

        return tickOn >= MinTickOn && tickOn <= MaxTickOn;
    }

    public override string ToString()
    {
        var minTickOn = MinTickOn.Value.ToTimeText(false);
        var maxTickOn = MaxTickOn.Value.ToTimeText(true);

        return $"{TradeDate} ({Market}: {minTickOn} to {maxTickOn})";
    }
}