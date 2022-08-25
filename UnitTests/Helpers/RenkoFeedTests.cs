// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.FxData.Helpers;
using SquidEyes.FxData.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace SquidEyes.UnitTests;

public class RenkoFeedTests
{
    [Theory]
    [InlineData(BidOrAsk.Bid)]
    [InlineData(BidOrAsk.Ask)]
    public void BricksFormCorrectly(BidOrAsk bidOrAsk)
    {
        int closedBrickCount = 0;
        int tickId = 1;
        var feed = GetFeed(true, bidOrAsk);

        void Validate(int expectedTickId, int expectedClosedBrickCount,
            BrickArgs args, int tickSeconds, int tickRate, int openSeconds,
            int openRate, int closeSeconds, int closeRate, bool isClosed)
        {
            if (tickId != expectedTickId)
                return;

            var minTickOn = new DateTime(2022, 8, 1, 10, 0, 0);

            TickOn GetTickOn(int seconds) => new(minTickOn.AddSeconds(seconds));

            Rate1 Adjust(Rate1 value) => 
                bidOrAsk == BidOrAsk.Bid ? value : value + Rate1.From(2);

            var rate = bidOrAsk == BidOrAsk.Bid ? args.Tick.Bid : args.Tick.Ask;

            closedBrickCount.Should().Be(expectedClosedBrickCount);
            args.Tick.TickOn.Should().Be(GetTickOn(tickSeconds));
            rate.Should().Be(Adjust(Rate1.From(tickRate)));
            args.Brick.Open.TickOn.Should().Be(GetTickOn(openSeconds));
            args.Brick.Open.Rate.Should().Be(Adjust(Rate1.From(openRate)));
            args.Brick.Close.TickOn.Should().Be(GetTickOn(closeSeconds));
            args.Brick.Close.Rate.Should().Be(Adjust(Rate1.From(closeRate)));
            args.IsClosed.Should().Be(isClosed);
        }

        feed.OnBrick += (s, e) =>
        {
            if (e.IsClosed)
                closedBrickCount++;

            Validate(1, 0, e, 00, 400, 00, 400, 00, 400, false);
            Validate(2, 0, e, 01, 420, 00, 400, 01, 420, false);
            Validate(3, 1, e, 02, 421, 00, 400, 02, 420, true);
            Validate(4, 1, e, 02, 421, 02, 420, 02, 421, false);
            Validate(5, 1, e, 03, 440, 02, 420, 03, 440, false);
            Validate(6, 2, e, 04, 441, 02, 420, 04, 440, true);
            Validate(7, 2, e, 04, 441, 04, 440, 04, 441, false);
            Validate(8, 2, e, 05, 460, 04, 440, 05, 460, false);
            Validate(9, 3, e, 06, 461, 04, 440, 06, 460, true);
            Validate(10, 3, e, 06, 461, 06, 460, 06, 461, false);
            Validate(11, 3, e, 07, 480, 06, 460, 07, 480, false);
            Validate(12, 4, e, 08, 481, 06, 460, 08, 480, true);
            Validate(13, 4, e, 08, 481, 08, 480, 08, 481, false);
            Validate(14, 4, e, 09, 440, 08, 480, 09, 440, false);
            Validate(15, 5, e, 10, 439, 06, 460, 10, 440, true);
            Validate(16, 5, e, 10, 439, 10, 440, 10, 439, false);
            Validate(17, 6, e, 11, 500, 06, 460, 11, 480, true);
            Validate(18, 6, e, 11, 500, 11, 480, 11, 500, false);
            Validate(19, 7, e, 12, 501, 11, 480, 12, 500, true);
            Validate(20, 7, e, 12, 501, 12, 500, 12, 501, false);
            Validate(21, 7, e, 13, 520, 12, 500, 13, 520, false);
            Validate(22, 8, e, 14, 521, 12, 500, 14, 520, true);
            Validate(23, 8, e, 14, 521, 14, 520, 14, 521, false);

            tickId++;
        };

        foreach (var tick in GetTicks())
            feed.HandleTick(tick);

        feed.Count.Should().Be(8);
    }

    [Fact]
    public void OpenBricksNotRaised()
    {
        var feed = GetFeed(false, BidOrAsk.Bid);

        var bricks = new List<Brick>();

        feed.OnBrick += (s, e) =>
        {
            if (!e.IsClosed)
                throw new ArgumentOutOfRangeException(nameof(e));
        };

        foreach (var tick in GetTicks())
            feed.HandleTick(tick);

        feed.Count.Should().Be(8);
    }

    [Fact]
    public void ExpectedPatternReturned()
    {
        var expected = new List<string>
            {
                "",
                "",
                "",
                "",
                "",
                "U",
                "U",
                "U",
                "UU",
                "UU",
                "UU",
                "UUU",
                "UUU",
                "UUU",
                "UUUU",
                "UUUU",
                "UUUUD",
                "UUUUD",
                "UUUUDU",
                "UUUUDU",
                "UUUUDU",
                "UUUUDUU",
                "UUUUDUU"
            };

        var feed = GetFeed(true, BidOrAsk.Bid);

        int tickId = 0;

        feed.OnBrick += (s, e) =>
            feed.GetPattern(true).Should().Be(expected[tickId++]);

        foreach (var tick in GetTicks())
            feed.HandleTick(tick);
    }

    private static RenkoFeed GetFeed(bool raiseOpenBricks, BidOrAsk bidOrAsk)
    {
        var tradeDate = TradeDate.From(2022, 8, 1);

        var session = new Session(tradeDate, Market.NewYork);

        return new RenkoFeed(session, bidOrAsk, Rate1.From(20), raiseOpenBricks);
    }

    private static List<Tick> GetTicks()
    {
        var tradeDate = TradeDate.From(2022, 8, 1);

        var session = new Session(tradeDate, Market.NewYork);

        Tick GetTick(DateTime dateTime, int bid) =>
            new(TickOn.From(dateTime, session), Rate1.From(bid), Rate1.From(bid + 2));

        return new List<Tick>
        {
            GetTick(new DateTime(2022, 8, 1, 10, 0, 0), 400),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 1), 420),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 2), 421),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 3), 440),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 4), 441),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 5), 460),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 6), 461),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 7), 480),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 8), 481),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 9), 440),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 10), 439),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 11), 500),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 12), 501),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 13), 520),
            GetTick(new DateTime(2022, 8, 1, 10, 0, 14), 521)
        };
    }
}