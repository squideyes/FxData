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
        var rate = Rate.From(intValue);

        rate.Value.Should().Be(intValue);
        rate.AsFloat(digits).Should().Be(floatValue);
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
        var rate = Rate.From(floatValue, digits);

        rate.Value.Should().Be(intValue);
        rate.AsFloat(digits).Should().Be(floatValue);
        rate.ToString().Should().Be(intValue.ToString());
        rate.ToString(digits).Should().Be(floatValue.ToString("N" + digits));
    }

    //////////////////////////

    [Fact]
    public void ConstructorWithoutArg()
    {
        var rate = new Rate();

        rate.Should().Be(Rate.From(1));
        rate.Value.Should().Be(1);
        rate.IsEmpty.Should().Be(false);
    }

    //////////////////////////

    [Fact]
    public void ToStringWithBadDigitsThrowsError()
    {
        FluentActions.Invoking(() => Rate.From(Rate.Minimum).ToString(4))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Fact]
    public void IsRateWithBadDigitsThrowsError()
    {
        FluentActions.Invoking(() => Rate.IsRate(1.2345f, 4))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Fact]
    public void SetToDefault()
    {
        Rate rate = default;

        rate.Value.Should().Be(0);
        rate.IsEmpty.Should().Be(true);
    }

    //////////////////////////

    [Theory]
    [InlineData(0)]
    [InlineData(1000000)]
    public void IntConstructorWithBadArg(int value)
    {
        FluentActions.Invoking(() => _ = Rate.From(value))
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
        Rate.IsRate(value, digits).Should().Be(result);

    //////////////////////////

    [Fact]
    public void RateNotEqualToNullRate() =>
          Rate.From(1).Equals(null).Should().BeFalse();

    //////////////////////////

    [Fact]
    public void GetHashCodeReturnsExpectedResult()
    {
        Rate.From(1).GetHashCode().Should().Be(Rate.From(1).GetHashCode());

        Rate.From(1).GetHashCode().Should().NotBe(Rate.From(2).GetHashCode());
    }

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GenericEquals(bool result) => Rate.From(1)
        .Equals(result ? Rate.From(1) : Rate.From(2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ObjectEqualsWithGoodRates(bool result) => Rate.From(1)
        .Equals(result ? Rate.From(1) : Rate.From(2)).Should().Be(result);

    //////////////////////////

    [Fact]
    public void ObjectEqualsWithNullRate() =>
        Rate.From(1).Equals(null).Should().BeFalse();

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EqualsOperator(bool result) => (Rate.From(1)
        == (result ? Rate.From(1) : Rate.From(2))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NotEqualsOperator(bool result) => (Rate.From(1)
        != (result ? Rate.From(2) : Rate.From(1))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(1, 2, -1)]
    [InlineData(2, 2, 0)]
    [InlineData(3, 2, 1)]
    public void CompareToWithMixedArgs(int v1, int v2, int result) =>
        Rate.From(v1).CompareTo(Rate.From(v2)).Should().Be(result);

    //////////////////////////

    [Fact]
    public void AddOperator() =>
        (Rate.From(1) + Rate.From(2)).Should().Be(Rate.From(3));

    //////////////////////////

    [Fact]
    public void SubtractOperator() =>
        (Rate.From(3) - Rate.From(2)).Should().Be(Rate.From(1));

    //////////////////////////

    [Theory]
    [InlineData(3, 3, false)]
    [InlineData(2, 3, true)]
    [InlineData(1, 3, true)]
    public void LessThanOperator(int v1, int v2, bool result) =>
        (Rate.From(v1) < Rate.From(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(1, 1, false)]
    [InlineData(2, 1, true)]
    [InlineData(3, 1, true)]
    public void GreaterThanOperator(int v1, int v2, bool result) =>
        (Rate.From(v1) > Rate.From(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(2, 4, true)]
    [InlineData(2, 3, true)]
    [InlineData(2, 2, true)]
    [InlineData(2, 1, false)]
    public void LessThanOrEqualToOperator(int v1, int v2, bool result) =>
        (Rate.From(v1) <= Rate.From(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(4, 2, true)]
    [InlineData(3, 2, true)]
    [InlineData(2, 2, true)]
    [InlineData(1, 2, false)]
    public void GreaterThanOrEqualToOperator(int v1, int v2, bool result) =>
        (Rate.From(v1) >= Rate.From(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(Rate.Minimum)]
    [InlineData(Rate.Maximum)]
    public void IntToRateToOperatorWithGoodArg(int value) =>
        Rate.From(value).Value.Should().Be(value);

    //////////////////////////

    [Theory]
    [InlineData(Rate.Maximum + 1)]
    [InlineData(Rate.Minimum - 1)]
    public void IntToRateToOperatorWithBadArg(int value)
    {
        FluentActions.Invoking(() => _ = Rate.From(value))
            .Should().Throw<ArgumentOutOfRangeException>();
    }
}