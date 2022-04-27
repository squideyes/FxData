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

namespace SquidEyes.UnitTests.FxData;

public class RateTests
{
    [Theory]
    [InlineData(5, 1, 0.00001f)]
    [InlineData(3, 1, 0.001f)]
    [InlineData(5, 999999, 9.99999f)]
    [InlineData(3, 999999, 999.999f)]
    public void IntConstructorWithGoodArg(int digits, int intValue, float floatValue)
    {
        var rate = new Rate(intValue);

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
        var rate = new Rate(floatValue, digits);

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

        rate.Should().Be(new Rate(1));
        rate.Value.Should().Be(1);
        rate.IsEmpty.Should().Be(false);
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
    public void IntContructorWithBadArg(int value)
    {
        FluentActions.Invoking(() => _ = new Rate(value))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    //////////////////////////

    [Theory]
    [InlineData(0.0f, 5)]
    [InlineData(10.0f, 5)]
    [InlineData(0.00001f, 0)]
    [InlineData(0.001f, 0)]
    public void FloatContructorWithBadArgs(float value, int digits)
    {
        FluentActions.Invoking(() => _ = new Rate(value, digits))
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
          new Rate(1).Equals(null).Should().BeFalse();

    //////////////////////////

    [Fact]
    public void GetHashCodeReturnsExpectedResult()
    {
        new Rate(1).GetHashCode().Should().Be(new Rate(1).GetHashCode());

        new Rate(1).GetHashCode().Should().NotBe(new Rate(2).GetHashCode());
    }

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GenericEquals(bool result) => new Rate(1)
        .Equals(result ? new Rate(1) : new Rate(2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ObjectEqualsWithGoodRates(bool result) => new Rate(1)
        .Equals(result ? new Rate(1) : new Rate(2)).Should().Be(result);

    //////////////////////////

    [Fact]
    public void ObjectEqualsWithNullRate() =>
        new Rate(1).Equals(null).Should().BeFalse();

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EqualsOperator(bool result) => (new Rate(1)
        == (result ? new Rate(1) : new Rate(2))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NotEqualsOperator(bool result) => (new Rate(1)
        != (result ? new Rate(2) : new Rate(1))).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(1, 2, -1)]
    [InlineData(2, 2, 0)]
    [InlineData(3, 2, 1)]
    public void CompareToWithMixedArgs(int v1, int v2, int result) =>
        new Rate(v1).CompareTo(new Rate(v2)).Should().Be(result);

    //////////////////////////

    [Fact]
    public void AddOperator() =>
        (new Rate(1) + new Rate(2)).Should().Be(new Rate(3));

    //////////////////////////

    [Fact]
    public void SubtractOperator() =>
        (new Rate(3) - new Rate(2)).Should().Be(new Rate(1));

    //////////////////////////

    [Theory]
    [InlineData(3, 3, false)]
    [InlineData(2, 3, true)]
    [InlineData(1, 3, true)]
    public void LessThanOperator(int v1, int v2, bool result) =>
        (new Rate(v1) < new Rate(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(1, 1, false)]
    [InlineData(2, 1, true)]
    [InlineData(3, 1, true)]
    public void GreaterThanOperator(int v1, int v2, bool result) =>
        (new Rate(v1) > new Rate(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(2, 4, true)]
    [InlineData(2, 3, true)]
    [InlineData(2, 2, true)]
    [InlineData(2, 1, false)]
    public void LessThanOrEqualToOperator(int v1, int v2, bool result) =>
        (new Rate(v1) <= new Rate(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(4, 2, true)]
    [InlineData(3, 2, true)]
    [InlineData(2, 2, true)]
    [InlineData(1, 2, false)]
    public void GreaterThanOrEqualToOperator(int v1, int v2, bool result) =>
        (new Rate(v1) >= new Rate(v2)).Should().Be(result);

    //////////////////////////

    [Theory]
    [InlineData(Rate.MinValue)]
    [InlineData(Rate.MaxValue)]
    public void IntToRateToOperatorWithGoodArg(int value) =>
        ((Rate)value).Value.Should().Be(value);

    //////////////////////////

    [Theory]
    [InlineData(Rate.MaxValue + 1)]
    [InlineData(Rate.MinValue - 1)]
    public void IntToRateToOperatorWithBadArg(int value)
    {
        FluentActions.Invoking(() => _ = (Rate)value)
            .Should().Throw<ArgumentOutOfRangeException>();
    }
}