//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//using SquidEyes.FxData.Models;

//namespace SquidEyes.UnitTests;

//public class RateTests
//{
//    private const double Precision5 = 0.00001; 
//    private const double Precision3 = 0.001; 

//    [Theory]
//    [InlineData(0, 0)]
//    [InlineData(100000, 5)]
//    [InlineData(-1000, 3)]
//    public void Digits_ReturnsExpectedValue(int value, int expectedDigits)
//    {
//        var rate = Rate.Create(value);
//        rate.Digits.Should().Be(expectedDigits);
//    }

//    [Theory]
//    [InlineData(1.23456, 5, 123456)]
//    [InlineData(1.234, 3, -1234)]
//    public void Create_WithDoubleAndDigits_CreatesExpectedRate(double input, int digits, int expectedValue)
//    {
//        var rate = Rate.Create(input, digits);
//        rate.Value.Should().Be(expectedValue);
//    }

//    [Theory]
//    [InlineData(123456, 1.23456, 5)]
//    [InlineData(-1234, 1.234, 3)]
//    public void AsDouble_ReturnsExpectedValue(int value, double expected, int digits)
//    {
//        var rate = Rate.Create(value);
//        rate.AsDouble().Should().BeApproximately(expected, digits == 5 ? Precision5 : Precision3,
//            because: $"{digits}-digit rate should be precise to {digits} decimal places");
//    }

//    [Fact]
//    public void AsDouble_RoundTripsAllValidValues()
//    {
//        for (int i = 100000; i <= 200000; i += 100)
//        {
//            var originalRate = Rate.Create(i);
//            var doubleValue = originalRate.AsDouble();
//            var roundTrippedRate = Rate.Create(doubleValue, 5);
//            var roundTrippedDouble = roundTrippedRate.AsDouble();

//            roundTrippedDouble.Should().BeApproximately(doubleValue, Precision5,
//                $"5-digit rate {doubleValue} should maintain its double value after round-trip within {Precision5} precision");
//        }

//        var edgeCases5 = new[] 
//        { 
//            Rate.MinInt32,
//            Rate.MinInt32 + 1,
//            100000, // 1.00000
//            123456, // 1.23456
//            200000, // 2.00000
//            999999, // 9.99999
//            Rate.MaxInt32 - 1,
//            Rate.MaxInt32
//        };

//        foreach (var value in edgeCases5)
//        {
//            var originalRate = Rate.Create(value);
//            var doubleValue = originalRate.AsDouble();
//            var roundTrippedRate = Rate.Create(doubleValue, 5);
//            var roundTrippedDouble = roundTrippedRate.AsDouble();

//            roundTrippedDouble.Should().BeApproximately(doubleValue, Precision5,
//                $"5-digit rate edge case {doubleValue} should maintain its double value after round-trip within {Precision5} precision");
//        }

//        for (int i = 1000; i <= 2000; i++)
//        {
//            var value = i / 1000.0;
//            var originalRate = Rate.Create(value, 3);
//            var doubleValue = originalRate.AsDouble();
//            var roundTrippedRate = Rate.Create(doubleValue, 3);
//            var roundTrippedDouble = roundTrippedRate.AsDouble();

//            roundTrippedDouble.Should().BeApproximately(doubleValue, Precision3,
//                $"3-digit rate {value} should maintain its double value after round-trip within {Precision3} precision");
//        }

//        var edgeCases3 = new[] 
//        { 
//            0.001, 
//            0.002,
//            1.000,
//            1.234,
//            2.000,
//            9.998,
//            9.999  
//        };

//        foreach (var value in edgeCases3)
//        {
//            var originalRate = Rate.Create(value, 3);
//            var doubleValue = originalRate.AsDouble();
//            var roundTrippedRate = Rate.Create(doubleValue, 3);
//            var roundTrippedDouble = roundTrippedRate.AsDouble();

//            roundTrippedDouble.Should().BeApproximately(doubleValue, Precision3,
//                $"3-digit rate edge case {value} should maintain its double value after round-trip within {Precision3} precision");
//        }
//    }

//    [Theory]
//    [InlineData(123456, "1.23456")]
//    [InlineData(-1234, "1.234")]
//    public void ToString_ReturnsExpectedFormat(int value, string expected)
//    {
//        var rate = Rate.Create(value);
//        rate.ToString().Should().Be(expected);
//    }

//    [Theory]
//    [InlineData("1.23456", 5)]
//    [InlineData("1.234", 3)]
//    public void Parse_CreatesExpectedRate(string input, int digits)
//    {
//        var rate = Rate.Parse(input, digits);
//        rate.ToString().Should().Be(input);
//    }

//    [Fact]
//    public void Empty_ReturnsRateWithZeroValue()
//    {
//        var empty = Rate.Empty;
//        empty.Value.Should().Be(0);
//    }

//    [Fact]
//    public void Create_WithValueOutOfRange_ThrowsArgumentOutOfRangeException()
//    {
//        var action1 = () => Rate.Create(0.0, 5);
//        var action2 = () => Rate.Create(1000000.0, 5);

//        action1.Should().Throw<ArgumentOutOfRangeException>();
//        action2.Should().Throw<ArgumentOutOfRangeException>();
//    }

//    [Fact]
//    public void Create_WithInvalidDigits_ThrowsInvalidOperationException()
//    {
//        var action = () => Rate.Create(1.0, 4);
//        action.Should().Throw<InvalidOperationException>();
//    }

//    [Theory]
//    [InlineData(123456, 123456, true)]
//    [InlineData(123456, 123457, false)]
//    public void Equals_ReturnsExpectedResult(int value1, int value2, bool expected)
//    {
//        var rate1 = Rate.Create(value1);
//        var rate2 = Rate.Create(value2);

//        rate1.Equals(rate2).Should().Be(expected);
//        (rate1 == rate2).Should().Be(expected);
//        (rate1 != rate2).Should().Be(!expected);
//    }

//    [Fact]
//    public void EqualsObject_ReturnsExpectedResult()
//    {
//        var rate = Rate.Create(123456);
        
//        // Test with null
//        rate.Equals(null).Should().BeFalse("Rate should not equal null");
        
//        // Test with different type
//        rate.Equals("123456").Should().BeFalse("Rate should not equal a string");
        
//        // Test with same Rate boxed as object
//        object boxedRate = Rate.Create(123456);
//        rate.Equals(boxedRate).Should().BeTrue("Rate should equal same Rate boxed as object");
        
//        // Test with different Rate boxed as object
//        object differentBoxedRate = Rate.Create(123457);
//        rate.Equals(differentBoxedRate).Should().BeFalse("Rate should not equal different Rate boxed as object");
//    }

//    [Theory]
//    [InlineData(123456, 123457, -1)]  
//    [InlineData(123457, 123456, 1)]   
//    [InlineData(123456, 123456, 0)]   
//    [InlineData(-1234, -1235, -1)]    
//    [InlineData(-1235, -1234, 1)]
//    public void CompareTo_ReturnsExpectedResult(
//        int value1, int value2, int expected)
//    {
//        var rate1 = Rate.Create(value1);
//        var rate2 = Rate.Create(value2);

//        Math.Sign(rate1.CompareTo(rate2)).Should().Be(expected);
//    }

//    [Theory]
//    [InlineData(123456, 123457, false, false, true, true)]
//    [InlineData(123456, 123456, false, true, false, true)]
//    [InlineData(123457, 123456, true, true, false, false)]
//    [InlineData(-1234, -1235, false, false, true, true)]  
//    public void ComparisonOperators_ReturnExpectedResults(
//        int value1, int value2, bool gt, bool gte, bool lt, bool lte)
//    {
//        var rate1 = Rate.Create(value1);
//        var rate2 = Rate.Create(value2);
        
//        (rate1 > rate2).Should().Be(gt, "greater than operator should work correctly");
//        (rate1 >= rate2).Should().Be(gte, "greater than or equal operator should work correctly");
//        (rate1 < rate2).Should().Be(lt, "less than operator should work correctly");
//        (rate1 <= rate2).Should().Be(lte, "less than or equal operator should work correctly");
//    }

//    [Fact]
//    public void GetHashCode_ReturnsSameValueForEqualRates()
//    {
//        var rate1 = Rate.Create(123456);
//        var rate2 = Rate.Create(123456);

//        rate1.GetHashCode().Should().Be(rate2.GetHashCode());
//    }
//}
