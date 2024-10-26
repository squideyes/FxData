//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//using SquidEyes.Fundamentals;
//using System;
//using System.Collections.Immutable;
//using static SquidEyes.FxData.Models.Symbol;
//using ISD = System.Collections.Immutable.ImmutableSortedDictionary<SquidEyes.FxData.Models.TradeDate, SquidEyes.FxData.Models.Session>;

//namespace SquidEyes.FxData.Models;

//public static class Known
//{
//    public static class UnitsPerLot
//    {
//        public const int Standard = 100000;
//        public const int Mini = 10000;
//        public const int Micro = 1000;
//    }

//    static Known()
//    {
//        var tradeDates = GetTradeDates();

//        TradeDates = [.. tradeDates];
//        Sessions = GetSessions(tradeDates);
//        Pairs = GetPairs();
//        ConvertWith = GetConvertWith();
//        Currencies = ImmutableSortedSet.CreateRange(EnumList.FromAll<Currency>());
//    }

//    public static ImmutableDictionary<Market, ISD> Sessions { get; }
//    public static IReadOnlyDictionary<Symbol, Pair> Pairs { get; }
//    public static ImmutableSortedSet<Currency> Currencies { get; }
//    public static ImmutableSortedSet<TradeDate> TradeDates { get; }
//    public static IReadOnlyDictionary<Pair, (Pair Base, Pair Quote)> ConvertWith { get; }

//    public static SortedSet<TradeDate> GetTradeDates(int year, int month)
//    {
//        if (!year.IsBetween(TradeDate.MinValue.Year, TradeDate.MaxValue.Year))
//            throw new ArgumentOutOfRangeException(nameof(year));

//        if (!month.IsBetween(1, 12))
//            throw new ArgumentOutOfRangeException(nameof(month));

//        return new SortedSet<TradeDate>(
//            TradeDates.Where(d => d.Year == year && d.Month == month));
//    }

//    private static IReadOnlyDictionary<Symbol, Pair> GetPairs()
//    {
//        var pairs = new Dictionary<Symbol, Pair>();

//        void Add(Symbol symbol, int digits) =>
//            new Pair(symbol, digits).Do(p => pairs.Add(p.Symbol, p));

//        Add(EURUSD, 5);
//        Add(GBPUSD, 5);
//        Add(USDJPY, 3);

//        return pairs;
//    }

//    private static IReadOnlyDictionary<Pair, (Pair, Pair)> GetConvertWith()
//    {
//        Dictionary<Pair, (Pair, Pair)> convertWith = new();

//        void AddLookups(Symbol symbol, Pair toBase, Pair toQuote) =>
//            convertWith.Add(Pairs[symbol], (toBase, toQuote));

//        AddLookups(EURUSD, Pairs[EURUSD], Pairs[EURUSD]);
//        AddLookups(GBPUSD, Pairs[GBPUSD], Pairs[GBPUSD]);
//        AddLookups(USDJPY, Pairs[USDJPY], Pairs[USDJPY]);

//        return convertWith;
//    }

//    private static SortedSet<TradeDate> GetTradeDates()
//    {
//        var tradeDates = new SortedSet<TradeDate>();

//        for (var date = TradeDate.MinDateOnly;
//            date <= TradeDate.MaxDateOnly; date = date.AddDays(1))
//        {
//            if (TradeDate.IsTradeDate(date))
//                tradeDates.Add(TradeDate.From(date));
//        }

//        return tradeDates;
//    }

//    private static ImmutableDictionary<Market, ISD> GetSessions(SortedSet<TradeDate> tradeDates)
//    {
//        var dict = new Dictionary<Market, ISD>
//        {
//            { Market.NewYork, GetSessionsForMarket(tradeDates, Market.NewYork) },
//            { Market.London, GetSessionsForMarket(tradeDates, Market.London) },
//            { Market.Combined, GetSessionsForMarket(tradeDates, Market.Combined) }
//        };

//        return dict.ToImmutableDictionary();
//    }

//    private static ISD GetSessionsForMarket(SortedSet<TradeDate> tradeDates, Market market)
//    {
//        var dict = new Dictionary<TradeDate, Session>();

//        foreach (var tradeDate in tradeDates)
//            dict.Add(tradeDate, Session.From(tradeDate, market));

//        return dict.ToImmutableSortedDictionary();
//    }
//}