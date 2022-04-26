// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Basics;
using SquidEyes.FxData.Context;
using System;
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class TickOnTests
{
    [Theory]
    [InlineData(Market.NewYork)]
    [InlineData(Market.London)]
    [InlineData(Market.Combined)]
    public void ConstructWithGoodArgs(Market market)
    {
        foreach (var tradeDate in Known.TradeDates!)
        {
            var session = new Session(tradeDate, market);

            void Validate(DateTime value)
            {
                var tickOn = new TickOn(value, session);

                tickOn.Value.Should().Be(value);
                tickOn.TradeDate.Should().Be(tradeDate);
                tickOn.ToString().Should().Be(value.ToDateTimeText());
            }

            Validate(session.MinTickOn.Value);
            Validate(session.MaxTickOn.Value);
        }
    }

    //////////////////////////

    [Fact]
    public void NoArgsConstruct()
    {
        FluentActions.Invoking(() => _ = new TickOn())
            .Should().Throw<InvalidOperationException>();
    }

    //////////////////////////

    [Fact]
    public void SetToDefault()
    {
        TickOn tickOn = default;

        tickOn.Value.Kind.Should().Be(DateTimeKind.Unspecified);
        tickOn.Should().Be(default);
        tickOn.Value.Should().Be(default);
        tickOn.IsEmpty.Should().Be(true);
    }

    //////////////////////////

    [Theory]
    [InlineData(Market.NewYork)]
    [InlineData(Market.London)]
    [InlineData(Market.Combined)]
    public void ContructWithBadValue(Market market)
    {
        var session = new Session(Known.MinTradeDate, market);

        var value = Known.MinTradeDate.Value.ToDateTime(new TimeOnly());

        FluentActions.Invoking(() => _ = new TickOn(value, session))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork)]
    [InlineData(Market.London)]
    [InlineData(Market.Combined)]
    public void ConstructWithBadSession(Market market)
    {
        var session = new Session(Known.MinTradeDate, market);

        FluentActions.Invoking(() => _ = new TickOn(session.MinTickOn.Value, null!))
            .Should().Throw<ArgumentNullException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork)]
    [InlineData(Market.London)]
    [InlineData(Market.Combined)]
    public void GetHashCodeReturnsExpectedResult(Market market)
    {
        var (left, right) = GetTicksOns(market, 1, 2);

        left.GetHashCode().Should().Be(left.GetHashCode());
        right.GetHashCode().Should().NotBe(left.GetHashCode());
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, true)]
    [InlineData(Market.London, true)]
    [InlineData(Market.Combined, true)]
    [InlineData(Market.NewYork, false)]
    [InlineData(Market.London, false)]
    [InlineData(Market.Combined, false)]
    public void GenericEquals(Market market, bool result)
    {
        var (left, right) = GetTicksOns(market, 1, 2);

        left.Equals(result ? left : right).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, true)]
    [InlineData(Market.London, true)]
    [InlineData(Market.Combined, true)]
    [InlineData(Market.NewYork, false)]
    [InlineData(Market.London, false)]
    [InlineData(Market.Combined, false)]
    public void ObjectEquals(Market market, bool result)
    {
        var (left, right) = GetTicksOns(market, 1, 2);

        left.Equals((object)(result ? left : right)).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, true)]
    [InlineData(Market.London, true)]
    [InlineData(Market.Combined, true)]
    [InlineData(Market.NewYork, false)]
    [InlineData(Market.London, false)]
    [InlineData(Market.Combined, false)]
    public void EqualsOperator(Market market, bool result)
    {
        var (left, right) = GetTicksOns(market, 1, 2);

        (left == (result ? left : right)).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, true)]
    [InlineData(Market.London, true)]
    [InlineData(Market.Combined, true)]
    [InlineData(Market.NewYork, false)]
    [InlineData(Market.London, false)]
    [InlineData(Market.Combined, false)]
    public void NotEqualsOperator(Market market, bool result)
    {
        var (left, right) = GetTicksOns(market, 1, 2);

        (left != (result ? right : left)).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, 3, 3, false)]
    [InlineData(Market.NewYork, 2, 3, true)]
    [InlineData(Market.NewYork, 1, 3, true)]
    [InlineData(Market.London, 3, 3, false)]
    [InlineData(Market.London, 2, 3, true)]
    [InlineData(Market.London, 1, 3, true)]
    [InlineData(Market.Combined, 3, 3, false)]
    [InlineData(Market.Combined, 2, 3, true)]
    [InlineData(Market.Combined, 1, 3, true)]
    public void LessThanOperator(Market market, int v1, int v2, bool result)
    {
        var (left, right) = GetTicksOns(market, v1, v2);

        (left < right).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, 1, 1, false)]
    [InlineData(Market.NewYork, 2, 1, true)]
    [InlineData(Market.NewYork, 3, 1, true)]
    [InlineData(Market.London, 1, 1, false)]
    [InlineData(Market.London, 2, 1, true)]
    [InlineData(Market.London, 3, 1, true)]
    [InlineData(Market.Combined, 1, 1, false)]
    [InlineData(Market.Combined, 2, 1, true)]
    [InlineData(Market.Combined, 3, 1, true)]
    public void GreaterThanOperator(Market market, int v1, int v2, bool result)
    {
        var (left, right) = GetTicksOns(market, v1, v2);

        (left > right).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, 2, 4, true)]
    [InlineData(Market.NewYork, 2, 3, true)]
    [InlineData(Market.NewYork, 2, 2, true)]
    [InlineData(Market.NewYork, 2, 1, false)]
    [InlineData(Market.London, 2, 4, true)]
    [InlineData(Market.London, 2, 3, true)]
    [InlineData(Market.London, 2, 2, true)]
    [InlineData(Market.London, 2, 1, false)]
    [InlineData(Market.Combined, 2, 4, true)]
    [InlineData(Market.Combined, 2, 3, true)]
    [InlineData(Market.Combined, 2, 2, true)]
    [InlineData(Market.Combined, 2, 1, false)]
    public void LessThanOrEqualToOperator(Market market, int v1, int v2, bool result)
    {
        var (left, right) = GetTicksOns(market, v1, v2);

        (left <= right).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, 4, 2, true)]
    [InlineData(Market.NewYork, 3, 2, true)]
    [InlineData(Market.NewYork, 2, 2, true)]
    [InlineData(Market.NewYork, 1, 2, false)]
    [InlineData(Market.London, 4, 2, true)]
    [InlineData(Market.London, 3, 2, true)]
    [InlineData(Market.London, 2, 2, true)]
    [InlineData(Market.London, 1, 2, false)]
    [InlineData(Market.Combined, 4, 2, true)]
    [InlineData(Market.Combined, 3, 2, true)]
    [InlineData(Market.Combined, 2, 2, true)]
    [InlineData(Market.Combined, 1, 2, false)]
    public void GreaterThanOrEqualToOperator(Market market, int v1, int v2, bool result)
    {
        var (left, right) = GetTicksOns(market, v1, v2);

        (left >= right).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork, 1, 2, -1)]
    [InlineData(Market.NewYork, 2, 2, 0)]
    [InlineData(Market.NewYork, 3, 2, 1)]
    [InlineData(Market.London, 1, 2, -1)]
    [InlineData(Market.London, 2, 2, 0)]
    [InlineData(Market.London, 3, 2, 1)]
    [InlineData(Market.Combined, 1, 2, -1)]
    [InlineData(Market.Combined, 2, 2, 0)]
    [InlineData(Market.Combined, 3, 2, 1)]
    public void CompareToWithMixedArgs(Market market, int v1, int v2, int result)
    {
        var (left, right) = GetTicksOns(market, v1, v2);

        left.CompareTo(right).Should().Be(result);
    }

    ////////////////////////////

    private static (TickOn Left, TickOn right) GetTicksOns(
        Market market, int left, int right)
    {
        return (GetTickOn(market, left), GetTickOn(market, right));
    }

    private static TickOn GetTickOn(Market market, int days)
    {
        return new Session(new TradeDate(Known.MinTradeDate.Value.AddDays(days)), market)
            .AsFunc(s => new TickOn(s.MinTickOn.Value, s));
    }
}