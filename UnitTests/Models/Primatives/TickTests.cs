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
using SquidEyes.FxData.Models;
using System;
using Xunit;

namespace SquidEyes.UnitTests;

public class TickTests
{
    [Fact]
    public void ConstructWithGoodArgs() =>
        _ = new Tick(GetTickOn(), Rate2.From(1, 5), Rate2.From(2, 5));

    ////////////////////////////

    [Theory]
    [InlineData(false, Rate2.MinInt32, Rate2.MaxInt32)]
    [InlineData(true, Rate2.MinInt32 - 1, Rate2.MaxInt32)]
    [InlineData(true, Rate2.MinInt32, Rate2.MaxInt32 + 1)]
    [InlineData(true, Rate2.MinInt32 + 1, Rate2.MinInt32)]
    public void ConstructWithBadArgs(bool goodTickOn, int bid, int ask)
    {
        TickOn tickOn;

        if (goodTickOn)
            tickOn = GetTickOn();
        else
            tickOn = default;

        FluentActions.Invoking(() => _ = new Tick(tickOn,
            Rate2.From(bid, 5), Rate2.From(ask, 5)))
                .Should().Throw<ArgumentException>();
    }

    ////////////////////////////

    [Fact]
    public void ConstructWithDefaultBid()
    {
        FluentActions.Invoking(() => _ = new Tick(GetTickOn(),
            default, Rate2.From(Rate2.MinInt32, 5)))
                .Should().Throw<ArgumentException>();
    }

    ////////////////////////////

    [Fact]
    public void ConstructWithDefaultAsk()
    {
        FluentActions.Invoking(() => _ = new Tick(GetTickOn(),
            Rate2.From(Rate2.MinInt32, 5), default))
                .Should().Throw<ArgumentException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(5, 1, 999999, "01/04/2016 08:00:00.000,0.00001,9.99999")]
    [InlineData(3, 1, 999999, "01/04/2016 08:00:00.000,0.001,999.999")]
    public void DigitsToCsvString(int digits, int bidValue, int askValue, string result)
    {
        GetTick(digits, bidValue, askValue).AsFunc(x => x.ToCsvString().Should().Be(result));
    }

    ////////////////////////////

    [Theory]
    [InlineData(5, 1, 999999, "01/04/2016 08:00:00.000,0.00001,9.99999")]
    [InlineData(3, 1, 999999, "01/04/2016 08:00:00.000,0.001,999.999")]
    public void PairToCsvString(
        int digits, int bidValue, int askValue, string result)
    {
        GetTick(digits, bidValue, askValue).AsFunc(x => x.ToCsvString().Should().Be(result));
    }

    ////////////////////////////

    [Fact]
    public void OverriddenToString()
    {
        GetTick(5, 1, 999999).AsFunc(x => x.ToString()
            .Should().Be("01/04/2016 08:00:00.000,1,999999"));
    }

    ////////////////////////////

    [Theory]
    [InlineData(1, 2, 1)]
    [InlineData(1, 3, 2)]
    [InlineData(1, 4, 3)]
    public void SpreadSetCorrectly(int bidValue, int askValue, int result)
    {
        GetTick(5, bidValue, askValue)
            .AsFunc(x => x.Spread.Should().Be(Rate2.From(result, 5)));
    }

    ////////////////////////////

    [Fact]
    public void TickNotEqualToNullTick() =>
        GetTick(5, 1, 2).Equals(null).Should().BeFalse();

    ////////////////////////////

    [Fact]
    public void TickNotEqualToNullObject() =>
        GetTick(5, 1, 2).Equals(null).Should().BeFalse();

    ////////////////////////////

    [Fact]
    public void GetHashCodeReturnsExpectedResult()
    {
        var tick = GetTick(5, 1, 2);

        var hashCode = HashCode.Combine(
            tick.TickOn, tick.Bid, tick.Ask);

        tick.GetHashCode().Should().Be(hashCode);
    }

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TickEqualsTick(bool result)
    {
        var tick1 = GetTick(5, 1, 2);
        var tick2 = result ? GetTick(5, 1, 2) : GetTick(5, 2, 3);

        tick1.Equals(tick2).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TickEqualsTickOperator(bool result)
    {
        var tick1 = GetTick(5, 1, 2);
        var tick2 = result ? GetTick(5, 1, 2) : GetTick(5, 2, 3);

        (tick1 == tick2).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TickNotEqualsTickOperator(bool result)
    {
        var tick1 = GetTick(5, 1, 2);
        var tick2 = result ? GetTick(5, 2, 3) : GetTick(5, 1, 2);

        (tick1 != tick2).Should().Be(result);
    }

    ////////////////////////////

    [Fact]
    public void IsEmptyReturnsExpectedValue()
    {
        GetTick(5, 1, 2).IsEmpty.Should().BeFalse();
        new Tick().IsEmpty.Should().BeTrue();
    }

    ////////////////////////////

    [Theory]
    [InlineData(5)]
    [InlineData(3)]
    public void ParseReturnsExpectedValue(int digits)
    {
        var session = new Session(TradeDate.MinValue, Market.Combined);

        var tick = Tick.Parse(
            "01/04/2016 03:00:00.000,0.00001,9.99999", Known.Pairs[Symbol.EURUSD], session);

        tick.TickOn.Should().Be(TickOn.From(new DateTime(2016, 1, 4, 3, 0, 0, 0), session));
        tick.Bid.Should().Be(Rate2.From(1, digits));
        tick.Ask.Should().Be(Rate2.From(999999, digits));
    }

    ////////////////////////////   

    [Theory]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public void ParseWithBadArgsThrowsError(bool goodValue, bool goodPair, bool goodSession)
    {
        var value = goodValue ? "01/04/2016 03:00:00.000,0.00001,9.99999" : "";
        var pair = goodPair ? Known.Pairs[Symbol.EURUSD] : null!;
        var session = new Session(TradeDate.MinValue, goodSession ? Market.Combined : Market.NewYork);

        FluentActions.Invoking(() => Tick.Parse(value, pair, session)).Should().Throw<Exception>();
    }

    ////////////////////////////   

    [Theory]
    [InlineData(5, 0, true)]
    [InlineData(5, 1, false)]
    [InlineData(3, 0, true)]
    [InlineData(3, 1, false)]
    public void InSessionReturnsExpectedValue(int digits, int days, bool expected)
    {
        var tick = GetTick(digits, 1, 2);

        var tradeDate = new TradeDate(TradeDate.MinValue.Value.AddDays(days));

        var session = new Session(tradeDate, Market.NewYork);

        tick.InSession(session).Should().Be(expected);
    }

    ////////////////////////////

    private static TickOn GetTickOn() =>
        new Session(TradeDate.MinValue, Market.NewYork).MinTickOn;

    private static Tick GetTick(int digits, int bidValue, int askValue) =>
        new(GetTickOn(), Rate2.From(bidValue, digits), Rate2.From(askValue, digits));
}