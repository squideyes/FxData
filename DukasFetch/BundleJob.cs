// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.FxData.Models;

namespace SquidEyes.FxData.DukasFetch;

internal class BundleJob
{
    public BundleJob(Symbol symbol, int year, int month, Market market)
    {
        Pair = Known.Pairs[symbol];
        Year = year;
        Month = month;
        Market = market;
    }

    public Pair Pair { get; }
    public int Year { get; }
    public int Month { get; }
    public Market Market { get; }

    public override string ToString() => $"{Pair} {Market} {Month:00}/{Year}";

    public async Task BundleSaveAndLogAsync(
        ILogger logger, AzureClient client, CancellationToken cancellationToken)
    {
        var bundle = new Bundle(Source.Dukascopy, Pair, Year, Month, Market);

        var tradeDates = Known.GetTradeDates(Year, Month);

        foreach (var tradeDate in tradeDates)
        {
            var tickSet = new TickSet(
                Source.Dukascopy, Pair, new Session(tradeDate, Market));

            if (cancellationToken.IsCancellationRequested)
                return;

            await client.LoadFromBlobAsync(tickSet, cancellationToken);

            bundle.Add(tickSet);

            logger.LogDebug($"ADDED {tickSet} to {bundle}");
        }

        if (cancellationToken.IsCancellationRequested)
            return;

        await client.UploadAsync(bundle, cancellationToken);

        logger.LogInformation($"SAVED {tradeDates.Count} tick-sets to {bundle}");
    }
}