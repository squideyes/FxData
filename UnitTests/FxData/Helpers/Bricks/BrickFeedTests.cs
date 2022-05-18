// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.FxData.Context;
using SquidEyes.FxData.Helpers;
using SquidEyes.FxData.Models;
using SquidEyes.UnitTests.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using SquidEyes.Basics;

namespace SquidEyes.UnitTests.FxData;

public class BrickFeedTests
{
    [Fact]
    public void BricksFormCorrectly()
    {
        var session = new Session(new TradeDate(2016, 1, 4), Market.NewYork);

        TickOn GetTickOn(int seconds) =>
            new(session!.MinTickOn.Value.AddSeconds(seconds));

        Tick GetTick(int seconds, Rate rate) => new(GetTickOn(seconds), rate, rate);

        var ticks = new List<Tick>
        {
            GetTick(0, 10),
            GetTick(1, 14),
            GetTick(2, 16),
            GetTick(3, 15),
            GetTick(4, 11),
            GetTick(5, 29),
            GetTick(6, 30),
            GetTick(7, 31),
            GetTick(8, 35),
            GetTick(9, 34),
        };

        var feed = new BrickFeed(5);

        int index = 0;

        feed.OnBrick += (s, e) =>
        {
            void Validate(int openSeconds, Rate open, int closeSeconds,
                Rate close, bool isFirstTickOfBar, bool isClosed, bool isVirtual)
            {
                e.Brick.OpenOn.Should().Be(GetTickOn(openSeconds));
                e.Brick.Open.Should().Be(open);
                e.Brick.CloseOn.Should().Be(GetTickOn(closeSeconds));
                e.Brick.Close.Should().Be(close);
                e.IsFirstTickOfBar.Should().Be(isFirstTickOfBar);
                e.Brick.IsClosed.Should().Be(isClosed);
                e.Brick.IsVirtual.Should().Be(isVirtual);
            }

            switch (index++)
            {
                case 0: Validate(0, 10, 0, 10, true, false, false); break;
                case 1: Validate(0, 10, 1, 14, false, false, false); break;
                case 2: Validate(0, 10, 2, 15, false, true, false); break;
                case 3: Validate(2, 15, 2, 16, true, false, false); break;
                case 4: Validate(2, 15, 3, 15, false, false, false); break;
                case 5: Validate(2, 15, 4, 11, false, false, false); break;
                case 6: Validate(2, 15, 5, 20, false, true, true); break;
                case 7: Validate(5, 20, 5, 25, false, true, false); break;
                case 8: Validate(5, 25, 5, 29, true, false, false); break;
                case 9: Validate(5, 25, 6, 30, false, false, false); break;
                case 10: Validate(5, 25, 7, 30, false, true, false); break;
                case 11: Validate(7, 30, 7, 31, true, false, false); break;
                case 12: Validate(7, 30, 8, 35, false, false, false); break;
                case 13: Validate(7, 30, 9, 34, false, false, false); break;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        };

        foreach (var tick in ticks)
            feed.HandleTick(tick);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void GeneratedBricksMatchBaselines(int day)
    {
        var pair = Known.Pairs[Symbol.EURUSD];

        var session = new Session(new TradeDate(2016, 1, day), Market.NewYork);

        var tickSet = new TickSet(Source.Dukascopy, pair, session);

        var bytes = day switch
        {
            4 => TestData.DC_EURUSD_20160104_NYC_EST_STS,
            5 => TestData.DC_EURUSD_20160105_NYC_EST_STS,
            6 => TestData.DC_EURUSD_20160106_NYC_EST_STS,
            7 => TestData.DC_EURUSD_20160107_NYC_EST_STS,
            8 => TestData.DC_EURUSD_20160108_NYC_EST_STS,
            _ => throw new ArgumentOutOfRangeException(nameof(day))
        };

        using var stream = new MemoryStream(bytes);

        tickSet.LoadFromStream(stream, DataKind.STS);

        var feed = new BrickFeed(20);

        var bricks = new List<Brick>();

        Brick? lastBrick = null;

        feed.OnBrick += (s, e) =>
        {
            if (e.Brick.IsClosed)
                bricks.Add(e.Brick);
            else
                lastBrick = e.Brick;
        };

        foreach (var tick in tickSet)
            feed.HandleTick(tick);

        bricks.Add(lastBrick!);

        var baselines = day switch
        {
            4 => TestData.BRICKFEED_BASELINES_4.ToLines(),
            5 => TestData.BRICKFEED_BASELINES_5.ToLines(),
            6 => TestData.BRICKFEED_BASELINES_6.ToLines(),
            7 => TestData.BRICKFEED_BASELINES_7.ToLines(),
            8 => TestData.BRICKFEED_BASELINES_8.ToLines(),
            _ => throw new ArgumentOutOfRangeException(nameof(day))
        };

        bricks.Count.Should().Be(baselines.Count);

        for (var i = 0; i < bricks.Count; i++)
            bricks[i].ToCsvString(pair).Should().Be(baselines[i]);
    }
}