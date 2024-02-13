// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Fundamentals;
using SquidEyes.FxData.Models;
using System;
using static SquidEyes.UnitTests.Properties.TestData;

namespace SquidEyes.UnitTests.Testing;

internal static class TestHelper
{
    public static TickSet GetEmptyTickSet(int day)
    {
        var (pair, session) = GetPairAndSession(day);

        return new TickSet(Source.Dukascopy, pair, session);
    }

    public static (Pair, Session) GetPairAndSession(int day)
    {
        return (Known.Pairs[Symbol.EURUSD], Session.From(
            TradeDate.From(2016, 1, day), Market.NewYork));
    }

    public static TickSet GetTickSet(int day)
    {
        var (pair, session) = GetPairAndSession(day);

        var tickSet = new TickSet(Source.Dukascopy, pair, session);

        //var bytes = day switch
        //{
        //    4 => DC_EURUSD_20160104_NYC_EST_STS,
        //    5 => DC_EURUSD_20160105_NYC_EST_STS,
        //    6 => DC_EURUSD_20160106_NYC_EST_STS,
        //    7 => DC_EURUSD_20160107_NYC_EST_STS,
        //    8 => DC_EURUSD_20160108_NYC_EST_STS,
        //    _ => throw new ArgumentOutOfRangeException(nameof(day))
        //};

        //tickSet.LoadFromStream(bytes.ToStream(), DataKind.STS);

        return tickSet;
    }
}