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
using Xunit;

namespace SquidEyes.UnitTests;

public class RateTests
{
    [Theory]
    [InlineData(5)]
    //[InlineData(3)]
    public void EveryRateRoundtrips(int digits)
    {
        for (int i = Rate.MinInt32; i <= Rate.MaxInt32; i++)
        {
            var source = Rate.From(i, digits);
            var asFloat = source.AsFloat();
            var target = Rate.From(asFloat, digits);

            target.Should().Be(source);
        }
    }


    [Theory]
    [InlineData(5, 1, 0.00001f)]
    [InlineData(3, 1, 0.001f)]
    [InlineData(5, 999999, 9.99999f)]
    [InlineData(3, 999999, 999.999f)]
    public void IntFromWithGoodArg(int digits, int intValue, float floatValue)
    {
        var rate = Rate.From(intValue, digits);

        rate.Digits.Should().Be(digits);
        rate.AsInt32().Should().Be(intValue);
        rate.AsFloat().Should().Be(floatValue);
        rate.ToString().Should().Be(floatValue.ToString("N" + digits));
    }

    //////////////////////////

    [Theory]
    [InlineData(5, 1, 0.00001f)]
    [InlineData(3, 1, 0.001f)]
    [InlineData(5, 999999, 9.99999f)]
    [InlineData(3, 999999, 999.999f)]
    public void FloatFromWithGoodArgs(int digits, int intValue, float floatValue)
    {
        var rate = Rate.From(floatValue, digits);

        rate.Digits.Should().Be(digits);
        rate.AsInt32().Should().Be(intValue);
        rate.AsFloat().Should().Be(floatValue);
        rate.ToString().Should().Be(floatValue.ToString("N" + digits));
    }

    //////////////////////////

    [Theory]
    [InlineData(5)]
    public void ConstructorWithoutArg(int digits)
    {
        var rate = new Rate();

        rate.Should().Be(Rate.From(1, digits));
        rate.AsInt32().Should().Be(1);
    }

    //////////////////////////

    [Fact]
    public void ToStringWithBadDigitsThrowsError()
    {
        FluentActions.Invoking(() => Rate.From(Rate.MinInt32, 4).ToString())
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Fact]
    public void IsRateWithBadDigitsThrowsError()
    {
        FluentActions.Invoking(() => Rate.IsRateValue(1.2345f, 4))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Fact]
    public void SetToDefault()
    {
        Rate rate = default;

        rate.AsInt32().Should().Be(0);
    }

    //////////////////////////

    [Theory]
    [InlineData(0, 5)]
    [InlineData(1000000, 5)]
    [InlineData(0, 3)]
    [InlineData(1000000, 3)]
    public void IntFromWithBadArg(int value, int digits)
    {
        FluentActions.Invoking(() => _ = Rate.From(value, digits))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(0.0f, 5)]
    [InlineData(10.0f, 5)]
    [InlineData(0.00001f, 0)]
    [InlineData(0.001f, 0)]
    public void FloatFromWithBadArgs(float value, int digits)
    {
        FluentActions.Invoking(() => _ = Rate.From(value, digits))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(0.0, 5, false)]
    [InlineData(0.00001f, 5, true)]
    [InlineData(9.99999f, 5, true)]
    [InlineData(10.0, 5, false)]
    [InlineData(0.0, 3, false)]
    [InlineData(0.001f, 3, true)]
    [InlineData(999.999f, 3, true)]
    [InlineData(1000.0, 3, false)]
    public void IsRateWithMixedArgs(float value, int digits, bool result) =>
        Rate.IsRateValue(value, digits).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5)]
    [InlineData(3)]
    public void RateNotEqualToNullRate(int digits) =>
        Rate.From(1, digits).Equals(null).Should().BeFalse();

    //////////////////////////

    [Theory]
    [InlineData(5)]
    [InlineData(3)]
    public void GetHashCodeReturnsExpectedResult(int digits)
    {
        Rate.From(1, digits).GetHashCode().Should()
            .Be(Rate.From(1, digits).GetHashCode());

        Rate.From(1, digits).GetHashCode().Should()
            .NotBe(Rate.From(2, digits).GetHashCode());
    }

    //////////////////////////

    [Theory]
    [InlineData(5, true)]
    [InlineData(5, false)]
    [InlineData(3, true)]
    [InlineData(3, false)]
    public void GenericEquals(int digits, bool result) => Rate.From(1, digits)
        .Equals(result ? Rate.From(1, digits) : Rate.From(2, digits)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5, true)]
    [InlineData(5, false)]
    [InlineData(3, true)]
    [InlineData(3, false)]
    public void ObjectEqualsWithGoodRates(int digits, bool result) => Rate.From(1, digits)
        .Equals(result ? Rate.From(1, digits) : Rate.From(2, digits)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5)]
    [InlineData(3)]
    public void ObjectEqualsWithNullRate(int digits) =>
        Rate.From(1, digits).Equals(null).Should().BeFalse();

    //////////////////////////

    [Theory]
    [InlineData(5, true)]
    [InlineData(5, false)]
    [InlineData(3, true)]
    [InlineData(3, false)]
    public void EqualsOperator(int digits, bool result) => (Rate.From(1, digits)
        == (result ? Rate.From(1, digits) : Rate.From(2, digits))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5, true)]
    [InlineData(5, false)]
    [InlineData(3, true)]
    [InlineData(3, false)]
    public void NotEqualsOperator(int digits, bool result) => (Rate.From(1, digits)
        != (result ? Rate.From(2, digits) : Rate.From(1, digits))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5, 1, 2, -1)]
    [InlineData(5, 2, 2, 0)]
    [InlineData(5, 3, 2, 1)]
    [InlineData(3, 1, 2, -1)]
    [InlineData(3, 2, 2, 0)]
    [InlineData(3, 3, 2, 1)]
    public void CompareToWithMixedArgs(int digits, int v1, int v2, int result) =>
        Rate.From(v1, digits).CompareTo(Rate.From(v2, digits)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5)]
    [InlineData(3)]
    public void AddOperator(int digits) =>
        (Rate.From(1, digits) + Rate.From(2, digits)).Should().Be(Rate.From(3, digits));

    //////////////////////////

    [Theory]
    [InlineData(5)]
    [InlineData(3)]
    public void SubtractOperator(int digits) =>
        (Rate.From(3, digits) - Rate.From(2, digits)).Should().Be(Rate.From(1, digits));

    //////////////////////////

    [Theory]
    [InlineData(5, 3, 3, false)]
    [InlineData(5, 2, 3, true)]
    [InlineData(5, 1, 3, true)]
    [InlineData(3, 3, 3, false)]
    [InlineData(3, 2, 3, true)]
    [InlineData(3, 1, 3, true)]
    public void LessThanOperator(int digits, int v1, int v2, bool result) =>
        (Rate.From(v1, digits) < Rate.From(v2, digits)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5, 1, 1, false)]
    [InlineData(5, 2, 1, true)]
    [InlineData(5, 3, 1, true)]
    [InlineData(3, 1, 1, false)]
    [InlineData(3, 2, 1, true)]
    [InlineData(3, 3, 1, true)]
    public void GreaterThanOperator(int digits, int v1, int v2, bool result) =>
        (Rate.From(v1, digits) > Rate.From(v2, digits)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5, 2, 4, true)]
    [InlineData(5, 2, 3, true)]
    [InlineData(5, 2, 2, true)]
    [InlineData(5, 2, 1, false)]
    [InlineData(3, 2, 4, true)]
    [InlineData(3, 2, 3, true)]
    [InlineData(3, 2, 2, true)]
    [InlineData(3, 2, 1, false)]
    public void LessThanOrEqualToOperator(int digits, int v1, int v2, bool result) =>
        (Rate.From(v1, digits) <= Rate.From(v2, digits)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5, 4, 2, true)]
    [InlineData(5, 3, 2, true)]
    [InlineData(5, 2, 2, true)]
    [InlineData(5, 1, 2, false)]
    [InlineData(3, 4, 2, true)]
    [InlineData(3, 3, 2, true)]
    [InlineData(3, 2, 2, true)]
    [InlineData(3, 1, 2, false)]
    public void GreaterThanOrEqualToOperator(int digits, int v1, int v2, bool result) =>
        (Rate.From(v1, digits) >= Rate.From(v2, digits)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(5, Rate.MinInt32)]
    [InlineData(5, Rate.MaxInt32)]
    [InlineData(3, Rate.MinInt32)]
    [InlineData(3, Rate.MaxInt32)]
    public void IntToRateToOperatorWithGoodArg(int digits, int value) =>
        Rate.From(value, digits).AsInt32().Should().Be(value);

    //////////////////////////

    [Theory]
    [InlineData(5, Rate.MaxInt32 + 1)]
    [InlineData(5, Rate.MinInt32 - 1)]
    [InlineData(3, Rate.MaxInt32 + 1)]
    [InlineData(3, Rate.MinInt32 - 1)]
    public void IntToRateToOperatorWithBadArg(int digits, int value)
    {
        FluentActions.Invoking(() => _ = Rate.From(value, digits))
            .Should().Throw<ArgumentOutOfRangeException>();
    }
}