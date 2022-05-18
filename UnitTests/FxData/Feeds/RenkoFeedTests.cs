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
using SquidEyes.FxData.Models;
using SquidEyes.Trading.FxData;
using SquidEyes.UnitTests.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static SquidEyes.FxData.Models.Trend;
using static SquidEyes.UnitTests.Properties.TestData;

namespace SquidEyes.UnitTests.FxData
{
    public class RenkoFeedTests
    {
        [Theory]
        [InlineData(false, 5, BidOrAsk.Ask)]
        [InlineData(true, 4, BidOrAsk.Ask)]
        [InlineData(true, 5, (BidOrAsk)0)]
        public void ConstructorWithBadArgs(bool goodSession, int brickTicks, BidOrAsk bidOrAsk)
        {
            var session = goodSession ? new Session(Known.MinTradeDate, Market.Combined) : null!;

            FluentActions.Invoking(() => new RenkoFeed(session, brickTicks, bidOrAsk))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void OutOfSessionTickDetected()
        {
            var session = new Session(Known.MinTradeDate, Market.Combined);
            
            var feed = new RenkoFeed(session, 10, BidOrAsk.Bid);

            var tick = new Tick(new TickOn(session.MinTickOn.Value.AddDays(1)),
                Rate.MIN_VALUE, Rate.MAX_VALUE);

            FluentActions.Invoking(() => feed.HandleTick(tick))
                .Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData(4, BidOrAsk.Bid, 134)]
        [InlineData(5, BidOrAsk.Bid, 218)]
        [InlineData(6, BidOrAsk.Bid, 230)]
        [InlineData(7, BidOrAsk.Bid, 154)]
        [InlineData(8, BidOrAsk.Bid, 249)]
        [InlineData(4, BidOrAsk.Ask, 142)]
        [InlineData(5, BidOrAsk.Ask, 211)]
        [InlineData(6, BidOrAsk.Ask, 229)]
        [InlineData(7, BidOrAsk.Ask, 158)]
        [InlineData(8, BidOrAsk.Ask, 249)]
        public void GeneratesExpectedBricks(
            int day, BidOrAsk bidOrAsk, int expected)
        {
            var tickSet = TestHelper.GetTickSet(day, DataKind.STS);

            var feed = new RenkoFeed(tickSet.Session, 10, bidOrAsk);

            var results = new List<(Tick Tick, Brick Brick)>();

            feed.OnBrick += (s, e) => results.Add((e.Tick, e.Brick));

            foreach (var tick in tickSet)
                feed.HandleTick(tick);

            results.Count.Should().Be(expected);

            var csv = (day, bidOrAsk) switch
            {
                (4, BidOrAsk.Bid) => RENKOFEEDTESTS_4_BID,
                (5, BidOrAsk.Bid) => RENKOFEEDTESTS_5_BID,
                (6, BidOrAsk.Bid) => RENKOFEEDTESTS_6_BID,
                (7, BidOrAsk.Bid) => RENKOFEEDTESTS_7_BID,
                (8, BidOrAsk.Bid) => RENKOFEEDTESTS_8_BID,
                (4, BidOrAsk.Ask) => RENKOFEEDTESTS_4_ASK,
                (5, BidOrAsk.Ask) => RENKOFEEDTESTS_5_ASK,
                (6, BidOrAsk.Ask) => RENKOFEEDTESTS_6_ASK,
                (7, BidOrAsk.Ask) => RENKOFEEDTESTS_7_ASK,
                (8, BidOrAsk.Ask) => RENKOFEEDTESTS_8_ASK,
                _ => throw new ArgumentOutOfRangeException()
            };

            var index = 0;

            Brick lastBrick = null!;

            foreach (var fields in new CsvEnumerator(csv.ToStream(), 7))
            {
                var result = results[index++];

                result.Tick.Should().Be(Tick.Parse(string.Join(
                    ",", fields.Take(3)), tickSet.Pair, tickSet.Session));

                var openOn = TickOn.Parse(fields[3], tickSet.Session);
                var open = Rate.Parse(fields[4], tickSet.Pair.Digits);
                var closeOn = TickOn.Parse(fields[5], tickSet.Session);
                var close = Rate.Parse(fields[6], tickSet.Pair.Digits);

                var brick = new Brick(openOn, open, closeOn, close);

                brick.OpenOn.Should().Be(openOn);
                brick.Open.Should().Be(open);
                brick.CloseOn.Should().Be(closeOn);
                brick.Close.Should().Be(close);

                if (lastBrick != null)
                {
                    switch ((brick.Trend, lastBrick.Trend))
                    {
                        case (Rising, Rising):
                            brick.Open.Should().Be(lastBrick.Close);
                            break;
                        case (Rising, Falling):
                            brick.Open.Should().Be(lastBrick.Open);
                            break;
                        case (Falling, Rising):
                            brick.Open.Should().Be(lastBrick.Open);
                            break;
                        case (Falling, Falling):
                            brick.Open.Should().Be(lastBrick.Close);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                lastBrick = brick;
            }
        }
    }
}