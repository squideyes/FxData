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
//using System;
//using Xunit;

//namespace SquidEyes.UnitTests;

//public class MiscExtendersTests
//{
//    [Theory]
//    [InlineData(Source.Dukascopy, "DC")]
//    [InlineData(Source.ForexCom, "FC")]
//    [InlineData(Source.HistData, "HD")]
//    [InlineData(Source.OandaCorp, "OC")]
//    [InlineData(Source.SquidEyes, "SE")]
//    public void ToCodeReturnsExpectedValue(Source source, string expected) =>   
//        source.ToCode().Should().Be(expected);

//    [Theory]
//    [InlineData("DC", Source.Dukascopy)]
//    [InlineData("FC", Source.ForexCom)]
//    [InlineData("HD", Source.HistData)]
//    [InlineData("OC", Source.OandaCorp)]
//    [InlineData("SE", Source.SquidEyes)]
//    public void ToSourceReturnsExpectedValue(string source, Source expected) =>
//        source.ToSource().Should().Be(expected);

//    [Fact]
//    public void ToSourceThrowsErrorOnBadArg()
//    {
//        FluentActions.Invoking(() => "XX".ToSource())
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    [Fact]
//    public void ToCodeThrowsErrorOnBadArg()
//    {
//        FluentActions.Invoking(() => ((Source)0).ToCode())
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    [Fact]
//    public void ToRateWithBadMidOrAskThrowsException()
//    {
//        var session = new Session(TradeDate.MinValue, Market.Combined);

//        var tick = new Tick(session.MinTickOn, Rate2.From(1, 5), Rate2.From(2, 5));

//        FluentActions.Invoking(() => tick.ToRate(0))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }
//}