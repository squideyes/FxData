// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using System.Collections.Immutable;
using static SquidEyes.FxData.Models.Symbol;
using static System.DayOfWeek;

namespace SquidEyes.FxData.Models;

public static class Known
{
    public static class UnitsPerLot
    {
        public const int Standard = 100000;
        public const int Mini = 10000;
        public const int Micro = 1000;
    }

    public const int MinYear = 2016;
    public const int MaxYear = 2028;

    private static readonly HashSet<DateOnly> validTradeDates;

    static Known()
    {
        validTradeDates = GetValidTradeDates();

        Pairs = GetPairs();
        ConvertWith = GetConvertWith();
        TradeDates = validTradeDates.Select(d => new TradeDate(d)).ToImmutableSortedSet();
        MinTradeDate = TradeDates.First();
        MaxTradeDate = TradeDates.Last();
        Currencies = ImmutableSortedSet.CreateRange(EnumList.FromAll<Currency>());
    }

    public static IReadOnlyDictionary<Symbol, Pair> Pairs { get; }
    public static IReadOnlyDictionary<Pair, (Pair Base, Pair Quote)> ConvertWith { get; }
    public static ImmutableSortedSet<TradeDate>? TradeDates { get; }
    public static TradeDate MinTradeDate { get; }
    public static TradeDate MaxTradeDate { get; }
    public static ImmutableSortedSet<Currency> Currencies { get; }

    public static bool IsTradeDate(DateOnly value) => validTradeDates.Contains(value);

    public static HashSet<TradeDate> GetTradeDates(int year, int month)
    {
        if (!year.Between(MinYear, MaxYear))
            throw new ArgumentOutOfRangeException(nameof(year));

        if (!month.Between(1, 12))
            throw new ArgumentOutOfRangeException(nameof(month));

        var minDate = new DateOnly(year, month, 1);

        while (!IsTradeDate(minDate))
            minDate = minDate.AddDays(1);

        var maxDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        while (!IsTradeDate(maxDate))
            maxDate = maxDate.AddDays(-1);

        var tradeDates = new HashSet<TradeDate>();

        for (var date = minDate; date <= maxDate; date = date.AddDays(1))
        {
            if (date.IsWeekday() && !IsHoliday(date))
                tradeDates.Add(new TradeDate(date));
        }

        return tradeDates;
    }

    private static bool IsHoliday(DateOnly value)
    {
        return (value.Month, value.Day, value.DayOfWeek) switch
        {
            (1, 1, Monday) => true,
            (1, 1, Tuesday) => true,
            (1, 1, Wednesday) => true,
            (1, 1, Thursday) => true,
            (1, 1, Friday) => true,
            (12, 25, Monday) => true,
            (12, 25, Tuesday) => true,
            (12, 25, Wednesday) => true,
            (12, 25, Thursday) => true,
            (12, 25, Friday) => true,
            _ => false,
        };
    }

    private static HashSet<DateOnly> GetValidTradeDates()
    {
        var minDate = new DateOnly(MinYear, 1, 1);

        while (minDate.DayOfWeek != Monday || IsHoliday(minDate))
            minDate = minDate.AddDays(1);

        var maxDate = new DateOnly(MaxYear, 12, 31);

        while (maxDate.DayOfWeek != Friday || IsHoliday(minDate))
            maxDate = maxDate.AddDays(-1);

        var tradeDates = new HashSet<DateOnly>();

        for (var date = minDate; date <= maxDate; date = date.AddDays(1))
        {
            if (date.IsWeekday() && !IsHoliday(date))
                tradeDates.Add(date);
        }

        return tradeDates;
    }

    private static IReadOnlyDictionary<Symbol, Pair> GetPairs()
    {
        var pairs = new Dictionary<Symbol, Pair>();

        void Add(Symbol symbol, int digits) =>
            new Pair(symbol, digits).AsAction(p => pairs.Add(p.Symbol, p));

        Add(EURUSD, 5);
        Add(GBPUSD, 5);
        Add(USDJPY, 3);

        return pairs;
    }

    private static IReadOnlyDictionary<Pair, (Pair, Pair)> GetConvertWith()
    {
        Dictionary<Pair, (Pair, Pair)> convertWith = new();

        void AddLookups(Symbol symbol, Pair toBase, Pair toQuote) =>
            convertWith.Add(Pairs[symbol], (toBase, toQuote));

        AddLookups(EURUSD, Pairs[EURUSD], Pairs[EURUSD]);
        AddLookups(GBPUSD, Pairs[GBPUSD], Pairs[GBPUSD]);
        AddLookups(USDJPY, Pairs[USDJPY], Pairs[USDJPY]);

        return convertWith;
    }
}