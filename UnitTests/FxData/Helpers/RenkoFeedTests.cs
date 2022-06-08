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

namespace SquidEyes.UnitTests.FxData;

public class RenkoFeedTests
{
    private const int TICKS_PER_BRICK = 10;

    [Theory]
    [InlineData(DayOfWeek.Monday, "UUUUUUUD")]
    [InlineData(DayOfWeek.Tuesday, "DUUUUUUU")]
    [InlineData(DayOfWeek.Wednesday, "DDDDDDUU")]
    [InlineData(DayOfWeek.Thursday, "DDDUDUUD")]
    [InlineData(DayOfWeek.Friday, "UDUDUDDU")]
    public void ExpectedPatternReturned(DayOfWeek dayOfWeek, string pattern)
    {
        var tickSet = GetTickSet(dayOfWeek);

        var feed = new RenkoFeed(tickSet.Session, TICKS_PER_BRICK);

        foreach (var tick in tickSet)
            feed.HandleTick(tick);

        feed.GetPattern(8).Should().Be(pattern);
    }

    [Theory]
    [InlineData(DayOfWeek.Monday, 149)]
    [InlineData(DayOfWeek.Tuesday, 235)]
    [InlineData(DayOfWeek.Wednesday, 244)]
    [InlineData(DayOfWeek.Thursday, 170)]
    [InlineData(DayOfWeek.Friday, 272)]
    public void BricksFormCorrectly(DayOfWeek dayOfWeek, int expectedBrickCount)
    {
        var tickSet = GetTickSet(dayOfWeek);

        var feed = new RenkoFeed(
            tickSet.Session, TICKS_PER_BRICK, expectedBrickCount);

        var bricks = new List<Brick>();

        feed.OnBrick += (s, e) =>
        {
            e.Brick.TicksPerBrick.Should().Be(TICKS_PER_BRICK);

            bricks.Add(e.Brick);
        };

        foreach (var tick in tickSet)
            feed.HandleTick(tick);

        bricks.Count.Should().Be(expectedBrickCount);

        static void AssertAreEqual(Brick a, Brick b)
        {
            a.Trend.Should().Be(b.Trend);
            a.Open.TickOn.Should().Be(b.Open.TickOn);
            a.Open.Rate.Should().Be(b.Open.Rate);
            a.Close.TickOn.Should().Be(b.Close.TickOn);
            a.Close.Rate.Should().Be(b.Close.Rate);
        }

        for (var i = 0; i < expectedBrickCount; i++)
            AssertAreEqual(bricks[bricks.Count - 1 - i], feed[i]);

        for (int i = 1; i < expectedBrickCount; i++)
        {
            var last = bricks[i - 1];
            var curr = bricks[i];

            switch (last.Trend, curr.Trend)
            {
                case (Trend.Up, Trend.Up):
                case (Trend.Down, Trend.Down):
                    curr.Open.Rate.Should().Be(last.Close.Rate);
                    break;
                case (Trend.Up, Trend.Down):
                case (Trend.Down, Trend.Up):
                    curr.Open.Rate.Should().Be(last.Open.Rate);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    private static TickSet GetTickSet(DayOfWeek dayOfWeek)
    {
        var pair = Known.Pairs[Symbol.EURUSD];

        var day = (int)dayOfWeek + 3;

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