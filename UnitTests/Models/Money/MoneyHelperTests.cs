//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//using FluentAssertions;
//using SquidEyes.FxData.Helpers;
//using SquidEyes.FxData.Models;
//using System.Collections.Generic;
//using Xunit;

//namespace SquidEyes.UnitTests;

//public class MoneyHelperTests
//{
//    private class MinMargin : IMinMargin
//    {
//        private readonly Dictionary<Symbol, double> percents = new()
//        {
//            { Symbol.EURUSD, 0.02 },
//            { Symbol.GBPUSD, 0.03 },
//            { Symbol.USDJPY, 0.02 }
//        };

//        public double this[Symbol symbol] => percents[symbol];
//        public double this[Pair pair] => percents[pair.Symbol];
//    }

//    [Theory]
//    [InlineData(Symbol.EURUSD, Side.Buy, 111460, 113460, 2269.21)]
//    [InlineData(Symbol.EURUSD, Side.Buy, 111460, 111460, 0.0)]
//    [InlineData(Symbol.EURUSD, Side.Buy, 114960, 113460, -1701.9)]
//    [InlineData(Symbol.EURUSD, Side.Sell, 113980, 113480, 567.4)]
//    [InlineData(Symbol.EURUSD, Side.Sell, 113980, 113980, 0.0)]
//    [InlineData(Symbol.EURUSD, Side.Sell, 113380, 113480, -113.47)]
//    public void GetGrossProfitReturnsExpectedValue(
//        Symbol symbol, Side side, int entry, int exit, double expected)
//    {
//        var helper = new MoneyHelper(
//            MoneyData.GetUsdValueOf(BidOrAsk.Ask), new MinMargin());

//        var actual = helper.GetPNL(Known.Pairs[symbol], side, 
//            100000, Rate.From(entry, 5), Rate.From(exit, 5));

//        actual.Should().Be(expected);
//    }

//    [Theory]
//    [InlineData(Symbol.USDJPY, Leverage.FiftyToOne, 0.0, 2000.0)]
//    [InlineData(Symbol.USDJPY, Leverage.FiftyToOne, 0.1, 2200.0)]
//    [InlineData(Symbol.EURUSD, Leverage.FiftyToOne, 0.0, 2269.2)]
//    [InlineData(Symbol.EURUSD, Leverage.FiftyToOne, 0.1, 2496.12)]
//    public void GetMarginInUsdReturnsExpectedValue(
//        Symbol symbol, Leverage leverage, double cushion, double expected)
//    {
//        var helper = new MoneyHelper(
//            MoneyData.GetUsdValueOf(BidOrAsk.Bid), new MinMargin());

//        var pair = Known.Pairs[symbol]; 

//        var actual = helper.GetMarginInUsd(pair, 100000, leverage, cushion);

//        actual.Should().Be(expected);
//    }
//}