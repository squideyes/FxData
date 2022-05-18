﻿// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.FxData.Context;
using SquidEyes.FxData.Models;
using System.Collections.Generic;

namespace SquidEyes.UnitTests.Context;

internal static class MoneyData
{
    private static readonly Dictionary<BidOrAsk, UsdValueOf> data = new();

    static MoneyData()
    {
        UsdValueOf GetData(BidOrAsk bidOrAsk)
        {
            var usdValueOf = new UsdValueOf(bidOrAsk);

            var session = new Session(new TradeDate(2020, 1, 6), Market.NewYork);

            void Update(Pair pair, int bid, int ask) =>
                usdValueOf!.Update(pair, new Tick(session.MinTickOn, bid, ask));

            Update(Known.Pairs[Symbol.EURUSD], 113460, 113480);
            Update(Known.Pairs[Symbol.GBPUSD], 135370, 135380);
            Update(Known.Pairs[Symbol.USDJPY], 115690, 115710);

            return usdValueOf;
        }

        data[BidOrAsk.Bid] = GetData(BidOrAsk.Bid);
        data[BidOrAsk.Ask] = GetData(BidOrAsk.Ask);
    }

    public static UsdValueOf GetUsdValueOf(BidOrAsk bidOrAsk) => data[bidOrAsk];
}