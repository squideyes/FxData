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

namespace SquidEyes.UnitTests
{
    public class RenkoFeedTests
    {
        [Theory]
        [InlineData(4, BidOrAsk.Bid, 930)]
        [InlineData(5, BidOrAsk.Bid, 488)]
        [InlineData(6, BidOrAsk.Bid, 1114)]
        [InlineData(7, BidOrAsk.Bid, 1020)]
        [InlineData(8, BidOrAsk.Bid, 1986)]
        [InlineData(4, BidOrAsk.Ask, 942)]
        [InlineData(5, BidOrAsk.Ask, 487)]
        [InlineData(6, BidOrAsk.Ask, 1084)]
        [InlineData(7, BidOrAsk.Ask, 1013)]
        [InlineData(8, BidOrAsk.Ask, 2014)]
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
                (4, BidOrAsk.Bid) => RENKOFEEDTESTS_4_Bid,
                (5, BidOrAsk.Bid) => RENKOFEEDTESTS_5_Bid,
                (6, BidOrAsk.Bid) => RENKOFEEDTESTS_6_Bid,
                (7, BidOrAsk.Bid) => RENKOFEEDTESTS_7_Bid,
                (8, BidOrAsk.Bid) => RENKOFEEDTESTS_8_Bid,
                (4, BidOrAsk.Ask) => RENKOFEEDTESTS_4_Ask,
                (5, BidOrAsk.Ask) => RENKOFEEDTESTS_5_Ask,
                (6, BidOrAsk.Ask) => RENKOFEEDTESTS_6_Ask,
                (7, BidOrAsk.Ask) => RENKOFEEDTESTS_7_Ask,
                (8, BidOrAsk.Ask) => RENKOFEEDTESTS_8_Ask,
                _ => throw new ArgumentOutOfRangeException()
            };

            int index = 0;

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