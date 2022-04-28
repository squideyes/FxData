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
using System;
using Xunit;

namespace SquidEyes.UnitTests.FxData;

public class TickTests
{
    [Fact]
    public void ContructWithGoodArgs() => _ = new Tick(GetTickOn(), 1, 2);

    ////////////////////////////

    [Theory]
    [InlineData(false, Rate.MIN_VALUE, Rate.MAX_VALUE)]
    [InlineData(true, Rate.MIN_VALUE - 1, Rate.MAX_VALUE)]
    [InlineData(true, Rate.MIN_VALUE, Rate.MAX_VALUE + 1)]
    [InlineData(true, Rate.MIN_VALUE + 1, Rate.MIN_VALUE)]
    public void ConstructWithBadArgs(bool goodTickOn, int bid, int ask)
    {
        TickOn tickOn;

        if (goodTickOn)
            tickOn = GetTickOn();
        else
            tickOn = default;

        FluentActions.Invoking(() => _ = new Tick(tickOn,
            bid, ask)).Should().Throw<ArgumentException>();
    }

    ////////////////////////////

    [Fact]
    public void ConstructWithDefaultBid()
    {
        FluentActions.Invoking(() => _ = new Tick(GetTickOn(),
            default, Rate.MIN_VALUE)).Should().Throw<ArgumentException>();
    }

    ////////////////////////////

    [Fact]
    public void ConstructWithDefaultAsk()
    {
        FluentActions.Invoking(() => _ = new Tick(GetTickOn(),
            Rate.MIN_VALUE, default)).Should().Throw<ArgumentException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(5, 1, 999999, "01/04/2016 08:00:00.000,0.00001,9.99999")]
    [InlineData(3, 1, 999999, "01/04/2016 08:00:00.000,0.001,999.999")]
    public void DigitsToCsvString(int digits, int bidValue, int askValue, string result)
    {
        GetTick(bidValue, askValue)
            .AsFunc(x => x.ToCsvString(digits).Should().Be(result));
    }

    ////////////////////////////

    [Theory]
    [InlineData(5, 1, 999999, "01/04/2016 08:00:00.000,0.00001,9.99999")]
    [InlineData(3, 1, 999999, "01/04/2016 08:00:00.000,0.001,999.999")]
    public void PairToCsvString(
        int digits, int bidValue, int askValue, string result)
    {
        var pair = digits == 5 ?
            Known.Pairs[Symbol.EURUSD] : Known.Pairs[Symbol.USDJPY];

        GetTick(bidValue, askValue)
            .AsFunc(x => x.ToCsvString(pair).Should().Be(result));
    }

    ////////////////////////////

    [Fact]
    public void OverriddenToString()
    {
        GetTick(1, 999999).AsFunc(x => x.ToString()
            .Should().Be("01/04/2016 08:00:00.000,1,999999"));
    }

    ////////////////////////////

    [Theory]
    [InlineData(1, 2, 1)]
    [InlineData(1, 3, 2)]
    [InlineData(1, 4, 3)]
    public void SpreadSetCorrectly(int bidValue, int askValue, int result)
    {
        GetTick(bidValue, askValue)
            .AsFunc(x => x.Spread.Should().Be(result));
    }

    ////////////////////////////

    [Fact]
    public void TickNotEqualToNullTick() =>
          GetTick(1, 2).Equals(null).Should().BeFalse();

    ////////////////////////////

    [Fact]
    public void TickNotEqualToNullObject() =>
        GetTick(1, 2).Equals(null).Should().BeFalse();

    ////////////////////////////

    [Fact]
    public void GetHashCodeReturnsExpectedResult()
    {
        var tick = GetTick(1, 2);

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
        var tick1 = GetTick(1, 2);
        var tick2 = result ? GetTick(1, 2) : GetTick(2, 3);

        tick1.Equals(tick2).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TickEqualsTickOperator(bool result)
    {
        var tick1 = GetTick(1, 2);
        var tick2 = result ? GetTick(1, 2) : GetTick(2, 3);

        (tick1 == tick2).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TickNotEqualsTickOperator(bool result)
    {
        var tick1 = GetTick(1, 2);
        var tick2 = result ? GetTick(2, 3) : GetTick(1, 2);

        (tick1 != tick2).Should().Be(result);
    }

    ////////////////////////////

    private static TickOn GetTickOn() =>
        new Session(Known.MinTradeDate, Market.NewYork).MinTickOn;

    private static Tick GetTick(int bidValue, int askValue) =>
        new(GetTickOn(), bidValue, askValue);
}