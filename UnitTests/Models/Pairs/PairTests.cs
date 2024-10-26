//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//using SquidEyes.Fundamentals;
//using SquidEyes.FxData.Models;
//using static SquidEyes.FxData.Models.Symbol;

//namespace SquidEyes.UnitTests;

//public class PairTests
//{
//    [Theory]
//    [InlineData(EURUSD)]
//    [InlineData(GBPUSD)]
//    [InlineData(USDJPY)]
//    public void ConstructorWithGoodArgs(Symbol symbol)
//    {
//        var (pair, digits) = GetPairAndDigits(symbol);

//        pair.Symbol.Should().Be(symbol);

//        pair.Digits.Should().Be(digits);

//        pair.Base.Should().Be(symbol.ToString()[0..3]
//            .ToEnumValue<Currency>());

//        pair.Quote.Should().Be(symbol.ToString()[3..]
//            .ToEnumValue<Currency>());

//        pair.OneTick.Should().Be(MathF.Pow(10, -digits));

//        pair.MinValue.Should().Be(pair.OneTick);

//        pair.OnePip.Should().Be(MathF.Pow(10, -(digits - 1)));

//        pair.MaxValue.Should().Be(MathF.Round(
//            (pair.OneTick * 1000000.0f) - pair.OneTick, digits));

//        pair.Factor.Should().Be((int)MathF.Pow(10, digits));
//    }

//    [Theory]
//    [InlineData((Symbol)0, 5)]
//    [InlineData(EURUSD, 0)]
//    public void ConstructorWithBadArgs(Symbol symbol, int digits)
//    {
//        FluentActions.Invoking(() => _ = new Pair(symbol, digits))
//            .Should().Throw<VerbException>();
//    }

//    [Theory]
//    [InlineData(EURUSD, 0.0, false)]
//    [InlineData(EURUSD, 0.00001, true)]
//    [InlineData(EURUSD, 9.99999, true)]
//    [InlineData(EURUSD, 100.0, false)]
//    [InlineData(USDJPY, 0.0, false)]
//    [InlineData(USDJPY, 0.001, true)]
//    [InlineData(USDJPY, 999.999, true)]
//    [InlineData(USDJPY, 1000.0, false)]
//    public void IsPriceWithMixedArgs(
//        Symbol symbol, float value, bool result)
//    {
//        GetPair(symbol).IsRate(value).Should().Be(result);
//    }

//    [Theory]
//    [InlineData(EURUSD, 0.000011f, 0.00001f)]
//    [InlineData(EURUSD, 0.000014f, 0.00001f)]
//    [InlineData(EURUSD, 0.000015f, 0.00002f)]
//    [InlineData(USDJPY, 0.0011f, 0.001f)]
//    [InlineData(USDJPY, 0.0014f, 0.001f)]
//    [InlineData(USDJPY, 0.0015f, 0.002f)]
//    public void RoundWithMixedArgs(
//        Symbol symbol, float value, float result)
//    {
//        GetPair(symbol).Round(value).Should().Be(result);
//    }

//    [Theory]
//    [InlineData(EURUSD)]
//    [InlineData(GBPUSD)]
//    [InlineData(USDJPY)]
//    public void ToStringReturnsSymbolToString(Symbol symbol) =>
//        GetPair(symbol).ToString().Should().Be(symbol.ToString());

//    [Theory]
//    [InlineData(EURUSD, 0.00001f, "0.00001")]
//    [InlineData(EURUSD, 0.10000f, "0.10000")]
//    [InlineData(EURUSD, 1.0f, "1.00000")]
//    [InlineData(EURUSD, 99.99999f, "99.99999")]
//    [InlineData(USDJPY, 0.001f, "0.001")]
//    [InlineData(USDJPY, 0.100f, "0.100")]
//    [InlineData(USDJPY, 1.0f, "1.000")]
//    [InlineData(USDJPY, 9999.999f, "9,999.999")]
//    public void FormatWithGoodArg(Symbol symbol, float value, string result)
//    {
//        GetPair(symbol).Format(value).Should().Be(result);
//    }

//    [Fact]
//    public void PairNotEqualToNullPair() =>
//          GetPair(EURUSD).Equals(null).Should().BeFalse();

//    [Fact]
//    public void PairNotEqualToNullObject() =>
//        GetPair(EURUSD).Equals((object?)null).Should().BeFalse();

//    [Fact]
//    public void GetHashCodeEqualsSymbolHashCode() => GetPair(EURUSD)
//        .GetHashCode().Should().Be(EURUSD.GetHashCode());

//    [Theory]
//    [InlineData(GBPUSD, GBPUSD, true)]
//    [InlineData(GBPUSD, EURUSD, false)]
//    public void PairEqualPair(Symbol s1, Symbol s2, bool result) =>
//        GetPair(s1).Equals(GetPair(s2)).Should().Be(result);

//    [Theory]
//    [InlineData(GBPUSD, GBPUSD, true)]
//    [InlineData(GBPUSD, EURUSD, false)]
//    public void PairEqualsOperator(Symbol s1, Symbol s2, bool result) =>
//        (GetPair(s1) == GetPair(s2)).Should().Be(result);

//    [Theory]
//    [InlineData(GBPUSD, GBPUSD, false)]
//    [InlineData(GBPUSD, EURUSD, true)]
//    public void PairNotEqualsOperator(Symbol s1, Symbol s2, bool result) =>
//        (GetPair(s1) != GetPair(s2)).Should().Be(result);

//    private static (Pair Pair, int Digits) GetPairAndDigits(Symbol symbol)
//    {
//        var digits = symbol.ToString().Contains("JPY") ? 3 : 5;

//        return (new Pair(symbol, digits), digits);
//    }

//    private static Pair GetPair(Symbol symbol) => GetPairAndDigits(symbol).Pair;
//}