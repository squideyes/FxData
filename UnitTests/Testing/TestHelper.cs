// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.FxData.Context;
using SquidEyes.FxData.FxData;
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
        return (Known.Pairs[Symbol.EURUSD], new Session(
            new TradeDate(2016, 1, day), Market.NewYork));
    }

    public static TickSet GetTickSet(int day, DataKind dataKind)
    {
        var (pair, session) = GetPairAndSession(day);

        var tickSet = new TickSet(Source.Dukascopy, pair, session);

        if (dataKind == DataKind.CSV)
        {
            var csv = day switch
            {
                4 => DC_EURUSD_20160104_NYC_EST_CSV,
                5 => DC_EURUSD_20160105_NYC_EST_CSV,
                6 => DC_EURUSD_20160106_NYC_EST_CSV,
                7 => DC_EURUSD_20160107_NYC_EST_CSV,
                8 => DC_EURUSD_20160108_NYC_EST_CSV,
                _ => throw new ArgumentOutOfRangeException(nameof(day))
            };

            foreach (var fields in new CsvEnumerator(csv.ToStream(), 3))
            {
                var tickOn = new TickOn(DateTime.Parse(fields[0]), tickSet.Session);
                var bid = new Rate(float.Parse(fields[1]), pair.Digits);
                var ask = new Rate(float.Parse(fields[2]), pair.Digits);

                tickSet.Add(new Tick(tickOn, bid, ask));
            }
        }
        else
        {
            var bytes = day switch
            {
                4 => DC_EURUSD_20160104_NYC_EST_STS,
                5 => DC_EURUSD_20160105_NYC_EST_STS,
                6 => DC_EURUSD_20160106_NYC_EST_STS,
                7 => DC_EURUSD_20160107_NYC_EST_STS,
                8 => DC_EURUSD_20160108_NYC_EST_STS,
                _ => throw new ArgumentOutOfRangeException(nameof(day))
            };

            tickSet.LoadFromStream(bytes.ToStream(), DataKind.STS);
        }

        return tickSet;
    }
}