// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Basics;
using SquidEyes.FxData.Context;
using SquidEyes.FxData.Helpers;
using SquidEyes.FxData.Models;
using SquidEyes.UnitTests.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SquidEyes.UnitTests.FxData;

public class BrickFeedTests
{
    [Fact]
    public void BricksFormCorrectly()
    {
        var session = new Session(new TradeDate(2022, 5, 2), Market.NewYork);

        var ticks = new List<Tick>
        {
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 0), session), 10, 10),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 1), session), 14, 14),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 2), session), 16, 16),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 3), session), 15, 15),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 4), session), 11, 11),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 5), session), 29, 29),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 6), session), 30, 30),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 7), session), 31, 31),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 8), session), 35, 35),
            new Tick(new TickOn(new DateTime(2022, 5, 2, 9, 30, 9), session), 24, 24),
        };

        var feed = new BrickFeed(5);

        int index = 0;

        feed.OnBrick += (s, e) =>
        {
            void Validate(int openSeconds, Rate open, int closeSeconds,
                Rate close, int bricksCount)
            {
                DataPoint GetTick(int seconds, Rate rate) => new(new TickOn(
                    new DateTime(2022, 5, 2, 9, 30, seconds), session), rate);

                var brick = new Brick(GetTick(openSeconds, open), GetTick(closeSeconds, close));

                feed.Count.Should().Be(bricksCount);

                e.Brick.Open.TickOn.Should().Be(brick.Open.TickOn);
                e.Brick.Open.Rate.Should().Be(brick.Open.Rate);
                e.Brick.Close.TickOn.Should().Be(brick.Close.TickOn);
                e.Brick.Close.Rate.Should().Be(brick.Close.Rate);

                feed[0].Open.TickOn.Should().Be(brick.Open.TickOn);
                feed[0].Open.Rate.Should().Be(brick.Open.Rate);
                feed[0].Close.TickOn.Should().Be(brick.Close.TickOn);
                feed[0].Close.Rate.Should().Be(brick.Close.Rate);
            }

            switch (index++)
            {
                case 0: Validate(0, 10, 2, 15, 1); break;
                case 1: Validate(2, 15, 5, 20, 2); break;
                case 2: Validate(5, 20, 5, 25, 3); break;
                case 3: Validate(5, 25, 7, 30, 4); break;
                case 4: Validate(7, 30, 9, 25, 5); break;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        };

        ticks.ForEach(tick => feed.HandleTick(tick));
    }

    [Theory]
    [InlineData(4, "DUUDUUUUU")]
    [InlineData(5, "DUUUUUUUU")]
    [InlineData(6, "DDDDDDUDU")]
    [InlineData(7, "UUDDDUUUD")]
    [InlineData(8, "UDUDUDUDD")]
    public void ExpectedPatternReturned(int day, string pattern)
    {
        var feed = new BrickFeed(10);

        foreach (var tick in GetTickSet(day))
            feed.HandleTick(tick);

        feed.GetPattern().Should().Be(pattern);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void GeneratedBricksMatchBaselines(int day)
    {
        var bricks = GetBricks(day);

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
            bricks[i].ToCsvString().Should().Be(baselines[i]);
    }

    private static List<Brick> GetBricks(int day)
    {
        var bricks = new List<Brick>();

        var feed = new BrickFeed(10);

        feed.OnBrick += (s, e) => bricks.Add(e.Brick);

        foreach (var tick in GetTickSet(day))
            feed.HandleTick(tick);

        return bricks;
    }

    private static TickSet GetTickSet(int day)
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

        return tickSet;
    }
}