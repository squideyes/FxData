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

//public class KnownTests
//{
//    [Fact]
//    public void KnownBaselineUnchanged()
//    {
//        foreach (var symbol in Enum.GetValues<Symbol>())
//            Known.Pairs.ContainsKey(symbol).Should().BeTrue();

//        foreach (var currency in Enum.GetValues<Currency>())
//            Known.Currencies.Contains(currency).Should().BeTrue();

//        Known.TradeDates!.Count.Should().Be(4147);

//        foreach (var pair in Known.Pairs.Values)
//            Known.ConvertWith.ContainsKey(pair);

//        TradeDate.MinValue.Should().Be(Known.TradeDates.First());

//        TradeDate.MinValue.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);

//        TradeDate.MaxValue.Should().Be(Known.TradeDates.Last());

//        TradeDate.MaxValue.Value.DayOfWeek.Should().Be(DayOfWeek.Friday);  
//    }

//    [Fact]
//    public void GetTradeDatesByYearMonthReturnsExpectedDates()
//    {
//        var byMonth = new Dictionary<(int, int), List<TradeDate>>();

//        foreach (var tradeDate in Known.TradeDates!)
//        {
//            var key = (tradeDate.Value.Year, tradeDate.Value.Month);

//            if (!byMonth.ContainsKey(key))
//                byMonth.Add(key, new List<TradeDate>());

//            byMonth[key].Add(tradeDate);
//        }

//        for (var year = TradeDate.MinValue.Year; year <= TradeDate.MaxValue.Year; year++)
//        {
//            for (var month = 1; month < 12; month++)
//            {
//                var tradeDates = Known.GetTradeDates(year, month).ToList();

//                var lookup = byMonth[(year, month)];

//                tradeDates.Count.Should().Be(lookup.Count);

//                for (var i = 0; i < lookup.Count; i++)
//                    tradeDates[i].Should().Be(lookup[i]);
//            }
//        }
//    }
//}