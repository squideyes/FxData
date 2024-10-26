//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//using FluentAssertions;
//using SquidEyes.Fundamentals;
//using SquidEyes.FxData.Models;
//using System;
//using Xunit;
//using static SquidEyes.FxData.Models.Market;

//namespace SquidEyes.UnitTests;

//public class SessionTests
//{
//    [Theory]
//    [InlineData(NewYork, "01/04/2016 08:00:00.000", "01/04/2016 16:59:59.999")]
//    [InlineData(London, "01/04/2016 02:00:00.000", "01/04/2016 10:59:59.999")]
//    [InlineData(Combined, "01/04/2016 02:00:00.000", "01/04/2016 16:59:59.999")]
//    public void ContructorWithGoodArgs(
//        Market market, string minTickOnString, string maxTickOnString)
//    {
//        var tradeDate = TradeDate.From(2016, 1, 4);

//        var session = new Session(tradeDate, market);

//        var minTickOn = TickOn.From(DateTime.Parse(minTickOnString), session);
//        var maxTickOn = TickOn.From(DateTime.Parse(maxTickOnString), session);

//        session.TradeDate.Should().Be(tradeDate);
//        session.Market.Should().Be(market);
//        session.MinTickOn.Should().Be(minTickOn);
//        session.MaxTickOn.Should().Be(maxTickOn);

//        session.InSession(session.MinTickOn.AsDateTime()).Should().BeTrue();
//        session.InSession(session.MaxTickOn.AsDateTime()).Should().BeTrue();

//        session.InSession(session.MinTickOn.Value).Should().BeTrue();
//        session.InSession(session.MaxTickOn.Value).Should().BeTrue();

//        session.ToString().Should().Be(
//            $"{tradeDate} ({market}: {minTickOn.Value:HH:mm:ss.fff} to {maxTickOn.Value:HH:mm:ss.fff})");
//    }

//    [Fact]
//    public void ConstructorWithBadMarket()
//    {
//        FluentActions.Invoking(() => _ = new Session(TradeDate.From(2016, 1, 4), 0))
//            .Should().Throw<VerbException>();
//    }

//    [Fact]
//    public void NewYorkAndLondonFallWithinCombined()
//    {
//        var newYork = new Session(TradeDate.MinValue, NewYork);
//        var london = new Session(TradeDate.MinValue, London);
//        var combined = new Session(TradeDate.MinValue, Combined);

//        london.MinTickOn.Should().Be(combined.MinTickOn);
//        newYork.MaxTickOn.Should().Be(combined.MaxTickOn);
//    }
//}