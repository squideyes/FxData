// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using Be.IO;
using SquidEyes.Basics;
using SquidEyes.FxData.Models;
using System.Text;

namespace SquidEyes.FxData.DukasFetch;

internal class Fetcher
{
    private readonly HttpClient client = new();

    public Fetcher(TickSet tickSet)
    {
        TickSet = tickSet;
    }

    public TickSet TickSet { get; }

    public async Task<(bool Success, List<Tick> Ticks)> GetTicksAsync(
        int hour, CancellationToken cancellationToken)
    {
        Rate ToRate(int value) => Rate.From(MathF.Round(
            value / TickSet.Pair.Factor, TickSet.Pair.Digits), TickSet.Pair.Digits);

        var response = await client.GetAsync(GetUri(hour), cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return (false, null!);

        if (!response.IsSuccessStatusCode)
            return (false, null!);

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return (false, null!);

        if (bytes.Length == 0)
            return (true, null!);

        var decompressed = SevenZipHelper.Decompress(bytes);

        using var reader = new BeBinaryReader(
            new MemoryStream(decompressed));

        var ticks = new List<Tick>();

        var minTickOn = TickSet.Session.TradeDate.Value.ToDateTime(TimeOnly.MinValue);

        while (reader.BaseStream.Position != reader.BaseStream.Length)
        {
            var ms = reader.ReadInt32();
            var ask = ToRate(reader.ReadInt32());
            var bid = ToRate(reader.ReadInt32());
            _ = reader.ReadSingle();
            _ = reader.ReadSingle();

            var tickOn = new TickOn(minTickOn.AddHours(hour)
                .AddMilliseconds(ms), TickSet.Session);

            ticks.Add(new Tick(tickOn, bid, ask));
        }

        return (true, ticks);
    }

    private Uri GetUri(int hour)
    {
        var sb = new StringBuilder();

        var mto = TickSet.Session.TradeDate.Value.ToDateTime(
            TimeOnly.MinValue).AddHours(hour).ToUtcFromEastern();

        sb.Append("https://datafeed.dukascopy.com/datafeed");
        sb.AppendDelimited(TickSet.Pair, '/');
        sb.AppendDelimited(mto.Year, '/');
        sb.AppendDelimited((mto.Month - 1).ToString("00"), '/');
        sb.AppendDelimited(mto.Day.ToString("00"), '/');
        sb.AppendDelimited(mto.Hour.ToString("00"), '/');
        sb.Append("h_ticks.bi5");

        return new Uri(sb.ToString());
    }

    public override string ToString() => TickSet.ToString();
}