// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Fundamentals;
using SquidEyes.FxData.Models;
using System;
using Xunit;

namespace SquidEyes.UnitTests;

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
                var tickOn = TickOn.From(value, session);

                tickOn.Value.Should().Be(value);

                TradeDate.From(DateOnly.FromDateTime(
                    tickOn.AsDateTime())).Should().Be(tradeDate);
                
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
    }

    //////////////////////////

    [Theory]
    [InlineData(Market.NewYork)]
    [InlineData(Market.London)]
    [InlineData(Market.Combined)]
    public void ContructWithBadValue(Market market)
    {
        var session = new Session(TradeDate.MinValue, market);

        var value = TradeDate.MinValue.Value.ToDateTime(new TimeOnly());

        FluentActions.Invoking(() => _ = TickOn.From(value, session))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork)]
    [InlineData(Market.London)]
    [InlineData(Market.Combined)]
    public void ConstructWithBadSession(Market market)
    {
        var session = new Session(TradeDate.MinValue, market);

        FluentActions.Invoking(() => _ = TickOn.From(session.MinTickOn.Value, null!))
            .Should().Throw<ArgumentNullException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(Market.NewYork)]
    [InlineData(Market.London)]
    [InlineData(Market.Combined)]
    public void GetHashCodeReturnsExpectedResult(Market market)
    {
        var (lhs, rhs) = GetTicksOns(market, 1, 2);

        lhs.GetHashCode().Should().Be(lhs.GetHashCode());
        rhs.GetHashCode().Should().NotBe(lhs.GetHashCode());
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
        var (lhs, rhs) = GetTicksOns(market, 1, 2);

        lhs.Equals(result ? lhs : rhs).Should().Be(result);
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
        var (lhs, rhs) = GetTicksOns(market, 1, 2);

        lhs.Equals((object)(result ? lhs : rhs)).Should().Be(result);
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
        var (lhs, rhs) = GetTicksOns(market, 1, 2);

        (lhs == (result ? lhs : rhs)).Should().Be(result);
    }

    ////////////////////////////

    [Fact]
    public void EqualsOperatorWithDefault()
    {
        TickOn lhs = default;
        TickOn rhs = default;

        (lhs == rhs).Should().BeTrue();
    }

    //////////////////////////

    [Theory]
    [InlineData(Market.NewYork, true)]
    [InlineData(Market.London, true)]
    [InlineData(Market.Combined, true)]
    [InlineData(Market.NewYork, false)]
    [InlineData(Market.London, false)]
    [InlineData(Market.Combined, false)]
    public void NotEqualsOperator(Market market, bool result)
    {
        var (lhs, rhs) = GetTicksOns(market, 1, 2);

        (lhs != (result ? rhs : lhs)).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void NotEqualsOperatorWithDefault(bool lhsIsDefault, bool rhsIsDefault)
    {
        var session = new Session(TradeDate.MinValue, Market.NewYork);

        TickOn lhs = lhsIsDefault ? default : session.MinTickOn;
        TickOn rhs = rhsIsDefault ? default : session.MaxTickOn;

        (lhs != rhs).Should().BeTrue();
    }

    //////////////////////////

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
        var (lhs, rhs) = GetTicksOns(market, v1, v2);

        (lhs < rhs).Should().Be(result);
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
        var (lhs, rhs) = GetTicksOns(market, v1, v2);

        (lhs > rhs).Should().Be(result);
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
        var (lhs, rhs) = GetTicksOns(market, v1, v2);

        (lhs <= rhs).Should().Be(result);
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
        var (lhs, rhs) = GetTicksOns(market, v1, v2);

        (lhs >= rhs).Should().Be(result);
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
        var (lhs, rhs) = GetTicksOns(market, v1, v2);

        lhs.CompareTo(rhs).Should().Be(result);
    }

    ////////////////////////////

    private static (TickOn Left, TickOn rhs) GetTicksOns(
        Market market, int lhs, int rhs)
    {
        return (GetTickOn(market, lhs), GetTickOn(market, rhs));
    }

    private static TickOn GetTickOn(Market market, int days)
    {
        return new Session(new TradeDate(TradeDate.MinValue.Value.AddDays(days)), market)
            .Convert(s => TickOn.From(s.MinTickOn.Value, s));
    }
}