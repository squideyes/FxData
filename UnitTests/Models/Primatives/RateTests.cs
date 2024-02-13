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
    [InlineData(0.00001f, 5, "N5")]
    [InlineData(9.99999f, 5, "N5")]
    [InlineData(0.001f, 3, "N3")]
    [InlineData(999.999f, 3, "N3")]
    public void From_WithGoodArgs_ReturnsGoodRate(
        float value, int digits, string format)
    {
        var rate = Rate.From(value, digits);

        rate.AsFloat().Should().Be(value);
        rate.ToString().Should().Be(value.ToString());
        rate.ToString(digits).Should().Be(value.ToString(format));
    }

    ////////////////////////////

    [Theory]
    [InlineData(0.0f, 5)]
    [InlineData(100000.0f, 5)]
    [InlineData(0.0f, 3)]
    [InlineData(1000.0f, 3)]
    public void From_WithBadValue_ThrowsError(int value, int digits)
    {
        FluentActions.Invoking(() => _ = Rate.From(value, digits))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Fact]
    public void From_WithBadDigits_ThrowsError()
    {
        FluentActions.Invoking(() => _ = Rate.From(0.00001f, 4))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Fact]
    public void IsRateValue_WithBadDigits_ThrowsError()
    {
        FluentActions.Invoking(() => Rate.IsRateValue(0.00001f, 4))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    [Theory]
    [InlineData(0.0, 5, false)]
    [InlineData(0.00001f, 5, true)]
    [InlineData(9.99999f, 5, true)]
    [InlineData(10.0, 5, false)]
    [InlineData(0.0, 3, false)]
    [InlineData(0.001f, 3, true)]
    [InlineData(999.999f, 3, true)]
    [InlineData(1000.0, 3, false)]
    public void IsRateValue_WithGoodDigits_ReturnsExpectedResult(
        float value, int digits, bool result)
    {
        Rate.IsRateValue(value, digits).Should().Be(result);
    }

    //////////////////////////

    [Theory]
    [InlineData(0.00001f, 5)]
    [InlineData(9.99999f, 5)]
    [InlineData(0.001f, 3)]
    [InlineData(999.999f, 3)]
    public void GetHashCodeReturnsExpectedResult(
        float value, int digits)
    {
        Rate.From(value, digits).GetHashCode()
            .Should().Be(value.GetHashCode());
    }

    ////////////////////////////

    [Theory]
    [InlineData(0.00001f, 9.99999f, 5, true)]
    [InlineData(0.001f, 999.999f, 3, true)]
    [InlineData(0.00001f, 9.99999f, 5, false)]
    [InlineData(0.001f, 999.999f, 3, false)]
    public void GenericEquals(
        float minValue, float maxValue, int digits, bool result)
    {
        var a = Rate.From(minValue, digits);
        var b = Rate.From(minValue, digits);
        var c = Rate.From(maxValue, digits);

        a.Equals(result ? b : c).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(0.00001f, 9.99999f, 5, true)]
    [InlineData(0.001f, 999.999f, 3, true)]
    [InlineData(0.00001f, 9.99999f, 5, false)]
    [InlineData(0.001f, 999.999f, 3, false)]
    public void ObjectEquals(
        float minValue, float maxValue, int digits, bool result)
    {
        var a = Rate.From(minValue, digits);
        object b = Rate.From(minValue, digits);
        object c = Rate.From(maxValue, digits);

        a.Equals(result ? b : c).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(0.00001f, 9.99999f, 5, true)]
    [InlineData(0.001f, 999.999f, 3, true)]
    [InlineData(0.00001f, 9.99999f, 5, false)]
    [InlineData(0.001f, 999.999f, 3, false)]
    public void EqualsOperator(
        float minValue, float maxValue, int digits, bool result)
    {
        var a = Rate.From(minValue, digits);
        var b = Rate.From(minValue, digits);
        var c = Rate.From(maxValue, digits);

        (a == (result ? b : c)).Should().Be(result);
    }

    ////////////////////////////

    [Theory]
    [InlineData(0.00001f, 9.99999f, 5, true)]
    [InlineData(0.001f, 999.999f, 3, true)]
    [InlineData(0.00001f, 9.99999f, 5, false)]
    [InlineData(0.001f, 999.999f, 3, false)]
    public void NotEqualsOperator(
        float minValue, float maxValue, int digits, bool result)
    {
        var a = Rate.From(minValue, digits);
        var b = Rate.From(minValue, digits);
        var c = Rate.From(maxValue, digits);

        (a != (result ? c : b)).Should().Be(result);
    }

    ////////////////////////////

    //[Theory]
    //[InlineData(5, 1, 2, -1)]
    //[InlineData(5, 2, 2, 0)]
    //[InlineData(5, 3, 2, 1)]
    //[InlineData(3, 1, 2, -1)]
    //[InlineData(3, 2, 2, 0)]
    //[InlineData(3, 3, 2, 1)]
    //public void CompareToWithMixedArgs(int digits, int v1, int v2, int result) =>
    //    Rate.From(v1, digits).CompareTo(Rate.From(v2, digits)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(5)]
    //[InlineData(3)]
    //public void AddOperator(int digits) =>
    //    (Rate.From(1, digits) + Rate.From(2, digits)).Should().Be(Rate.From(3, digits));

    ////////////////////////////

    //[Theory]
    //[InlineData(5)]
    //[InlineData(3)]
    //public void SubtractOperator(int digits) =>
    //    (Rate.From(3, digits) - Rate.From(2, digits)).Should().Be(Rate.From(1, digits));

    ////////////////////////////

    //[Theory]
    //[InlineData(5, 3, 3, false)]
    //[InlineData(5, 2, 3, true)]
    //[InlineData(5, 1, 3, true)]
    //[InlineData(3, 3, 3, false)]
    //[InlineData(3, 2, 3, true)]
    //[InlineData(3, 1, 3, true)]
    //public void LessThanOperator(int digits, int v1, int v2, bool result) =>
    //    (Rate.From(v1, digits) < Rate.From(v2, digits)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(5, 1, 1, false)]
    //[InlineData(5, 2, 1, true)]
    //[InlineData(5, 3, 1, true)]
    //[InlineData(3, 1, 1, false)]
    //[InlineData(3, 2, 1, true)]
    //[InlineData(3, 3, 1, true)]
    //public void GreaterThanOperator(int digits, int v1, int v2, bool result) =>
    //    (Rate.From(v1, digits) > Rate.From(v2, digits)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(5, 2, 4, true)]
    //[InlineData(5, 2, 3, true)]
    //[InlineData(5, 2, 2, true)]
    //[InlineData(5, 2, 1, false)]
    //[InlineData(3, 2, 4, true)]
    //[InlineData(3, 2, 3, true)]
    //[InlineData(3, 2, 2, true)]
    //[InlineData(3, 2, 1, false)]
    //public void LessThanOrEqualToOperator(int digits, int v1, int v2, bool result) =>
    //    (Rate.From(v1, digits) <= Rate.From(v2, digits)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(5, 4, 2, true)]
    //[InlineData(5, 3, 2, true)]
    //[InlineData(5, 2, 2, true)]
    //[InlineData(5, 1, 2, false)]
    //[InlineData(3, 4, 2, true)]
    //[InlineData(3, 3, 2, true)]
    //[InlineData(3, 2, 2, true)]
    //[InlineData(3, 1, 2, false)]
    //public void GreaterThanOrEqualToOperator(int digits, int v1, int v2, bool result) =>
    //    (Rate.From(v1, digits) >= Rate.From(v2, digits)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(5, Rate.MinInt32)]
    //[InlineData(5, Rate.MaxInt32)]
    //[InlineData(3, Rate.MinInt32)]
    //[InlineData(3, Rate.MaxInt32)]
    //public void IntToRateToOperatorWithGoodArg(int digits, int value) =>
    //    Rate.From(value, digits).AsInt32().Should().Be(value);

    ////////////////////////////

    //[Theory]
    //[InlineData(5, Rate.MaxInt32 + 1)]
    //[InlineData(5, Rate.MinInt32 - 1)]
    //[InlineData(3, Rate.MaxInt32 + 1)]
    //[InlineData(3, Rate.MinInt32 - 1)]
    //public void IntToRateToOperatorWithBadArg(int digits, int value)
    //{
    //    FluentActions.Invoking(() => _ = Rate.From(value, digits))
    //        .Should().Throw<ArgumentOutOfRangeException>();
    //}
}