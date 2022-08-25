// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.FxData.Models;
using System;
using Xunit;

namespace SquidEyes.UnitTests;

public class RateTests
{
    [Theory]
    [InlineData(5, 1, 0.00001f)]
    [InlineData(3, 1, 0.001f)]
    [InlineData(5, 999999, 9.99999f)]
    [InlineData(3, 999999, 999.999f)]
    public void IntConstructorWithGoodArg(int digits, int intValue, float floatValue)
    {
        var rate = Rate1.From(intValue);

        rate.AsInt32().Should().Be(intValue);
        rate.ToFloat(digits).Should().Be(floatValue);
        rate.ToString().Should().Be(intValue.ToString());
        rate.ToString(digits).Should().Be(floatValue.ToString("N" + digits));
    }

    //////////////////////////

    [Theory]
    [InlineData(5, 1, 0.00001f)]
    [InlineData(3, 1, 0.001f)]
    [InlineData(5, 999999, 9.99999f)]
    [InlineData(3, 999999, 999.999f)]
    public void FloatConstructorWithGoodArgs(int digits, int intValue, float floatValue)
    {
        var rate = Rate1.From(floatValue, digits);

        rate.AsInt32().Should().Be(intValue);
        rate.ToFloat(digits).Should().Be(floatValue);
        rate.ToString().Should().Be(intValue.ToString());
        rate.ToString(digits).Should().Be(floatValue.ToString("N" + digits));
    }

    //////////////////////////

    [Fact]
    public void ConstructorWithoutArg()
    {
        var rate = new Rate1();

        rate.Should().Be(Rate1.From(1));
        rate.AsInt32().Should().Be(1);
    }

    //////////////////////////

    [Fact]
    public void ToStringWithBadDigitsThrowsError()
    {
        FluentActions.Invoking(() => Rate1.From(Rate1.Minimum).ToString(4))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Fact]
    public void IsRateWithBadDigitsThrowsError()
    {
        FluentActions.Invoking(() => Rate1.IsRate(1.2345f, 4))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Fact]
    public void SetToDefault()
    {
        Rate1 rate = default;

        rate.AsInt32().Should().Be(0);
    }

    //////////////////////////

    [Theory]
    [InlineData(0)]
    [InlineData(1000000)]
    public void IntConstructorWithBadArg(int value)
    {
        FluentActions.Invoking(() => _ = Rate1.From(value))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(0.0f, 5)]
    [InlineData(10.0f, 5)]
    [InlineData(0.00001f, 0)]
    [InlineData(0.001f, 0)]
    public void FloatConstructorWithBadArgs(float value, int digits)
    {
        FluentActions.Invoking(() => _ = Rate1.From(value, digits))
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
        Rate1.IsRate(value, digits).Should().Be(result);

    //////////////////////////

    [Fact]
    public void RateNotEqualToNullRate() =>
          Rate1.From(1).Equals(null).Should().BeFalse();

    //////////////////////////

    [Fact]
    public void GetHashCodeReturnsExpectedResult()
    {
        Rate1.From(1).GetHashCode().Should().Be(Rate1.From(1).GetHashCode());

        Rate1.From(1).GetHashCode().Should().NotBe(Rate1.From(2).GetHashCode());
    }

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GenericEquals(bool result) => Rate1.From(1)
        .Equals(result ? Rate1.From(1) : Rate1.From(2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ObjectEqualsWithGoodRates(bool result) => Rate1.From(1)
        .Equals(result ? Rate1.From(1) : Rate1.From(2)).Should().Be(result);

    //////////////////////////

    [Fact]
    public void ObjectEqualsWithNullRate() =>
        Rate1.From(1).Equals(null).Should().BeFalse();

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EqualsOperator(bool result) => (Rate1.From(1)
        == (result ? Rate1.From(1) : Rate1.From(2))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NotEqualsOperator(bool result) => (Rate1.From(1)
        != (result ? Rate1.From(2) : Rate1.From(1))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(1, 2, -1)]
    [InlineData(2, 2, 0)]
    [InlineData(3, 2, 1)]
    public void CompareToWithMixedArgs(int v1, int v2, int result) =>
        Rate1.From(v1).CompareTo(Rate1.From(v2)).Should().Be(result);

    //////////////////////////

    [Fact]
    public void AddOperator() =>
        (Rate1.From(1) + Rate1.From(2)).Should().Be(Rate1.From(3));

    //////////////////////////

    [Fact]
    public void SubtractOperator() =>
        (Rate1.From(3) - Rate1.From(2)).Should().Be(Rate1.From(1));

    //////////////////////////

    [Theory]
    [InlineData(3, 3, false)]
    [InlineData(2, 3, true)]
    [InlineData(1, 3, true)]
    public void LessThanOperator(int v1, int v2, bool result) =>
        (Rate1.From(v1) < Rate1.From(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(1, 1, false)]
    [InlineData(2, 1, true)]
    [InlineData(3, 1, true)]
    public void GreaterThanOperator(int v1, int v2, bool result) =>
        (Rate1.From(v1) > Rate1.From(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(2, 4, true)]
    [InlineData(2, 3, true)]
    [InlineData(2, 2, true)]
    [InlineData(2, 1, false)]
    public void LessThanOrEqualToOperator(int v1, int v2, bool result) =>
        (Rate1.From(v1) <= Rate1.From(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(4, 2, true)]
    [InlineData(3, 2, true)]
    [InlineData(2, 2, true)]
    [InlineData(1, 2, false)]
    public void GreaterThanOrEqualToOperator(int v1, int v2, bool result) =>
        (Rate1.From(v1) >= Rate1.From(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(Rate1.Minimum)]
    [InlineData(Rate1.Maximum)]
    public void IntToRateToOperatorWithGoodArg(int value) =>
        Rate1.From(value).AsInt32().Should().Be(value);

    //////////////////////////

    [Theory]
    [InlineData(Rate1.Maximum + 1)]
    [InlineData(Rate1.Minimum - 1)]
    public void IntToRateToOperatorWithBadArg(int value)
    {
        FluentActions.Invoking(() => _ = Rate1.From(value))
            .Should().Throw<ArgumentOutOfRangeException>();
    }
}