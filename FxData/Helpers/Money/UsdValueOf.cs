// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.FxData.Models;
using System.Collections.Concurrent;

namespace SquidEyes.FxData.Helpers;

public class UsdValueOf
{
    private readonly ConcurrentDictionary<Pair, Rate> rates = new();

    private readonly BidOrAsk bidOrAsk;

    public UsdValueOf(BidOrAsk bidOrAsk)
    {
        this.bidOrAsk = bidOrAsk.Validated(nameof(bidOrAsk), v => v.IsEnumValue());
    }

    public void Update(Pair pair, Tick tick)
    {
        if (Equals(tick, null!))
            throw new ArgumentNullException(nameof(tick));

        var rate = bidOrAsk switch
        {
            BidOrAsk.Bid => tick.Bid,
            BidOrAsk.Ask => tick.Ask,
            _ => throw new ArgumentOutOfRangeException(nameof(tick))
        };

        rates.AddOrUpdate(pair, rate, (p, v) => rate);
    }

    public Rate GetRateInUsd(Currency currency)
    {
        Rate GetUsdRate(Symbol symbol) => 
            rates.GetOrAdd(Known.Pairs[symbol], p => default);

        Rate GetJpyRate(Symbol symbol)
        {
            var rate = GetUsdRate(symbol).AsFloat();

            return Known.Pairs[symbol].AsFunc(p => Rate.From(1.0f / rate, 5));
        }

        return currency switch
        {
            Currency.JPY => GetJpyRate(Symbol.USDJPY),
            Currency.EUR => GetUsdRate(Symbol.EURUSD),
            Currency.GBP => GetUsdRate(Symbol.GBPUSD),
            Currency.USD => Rate.From(100000, 5),
            _ => throw new ArgumentOutOfRangeException(nameof(currency))
        };
    }
}