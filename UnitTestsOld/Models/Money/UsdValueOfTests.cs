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
using Xunit;

namespace SquidEyes.UnitTests;

public class UsdValueOfTests
{
    [Theory]
    [InlineData(Currency.EUR, BidOrAsk.Bid, 113460)]
    [InlineData(Currency.EUR, BidOrAsk.Ask, 113480)]
    [InlineData(Currency.GBP, BidOrAsk.Bid, 135370)]
    [InlineData(Currency.GBP, BidOrAsk.Ask, 135380)]
    [InlineData(Currency.JPY, BidOrAsk.Bid, 9)]
    [InlineData(Currency.JPY, BidOrAsk.Ask, 9)]
    [InlineData(Currency.USD, BidOrAsk.Bid, 100000)]
    [InlineData(Currency.USD, BidOrAsk.Ask, 100000)]
    public void GetRateInUsdReturnedExpectedValue(
        Currency currency, BidOrAsk bidOrAsk, int rate)
    {
        var usdValueOf = MoneyData.GetUsdValueOf(bidOrAsk);

        usdValueOf.GetRateInUsd(currency).AsInt32().Should().Be(rate);
    }
}