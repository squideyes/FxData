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

internal class FetchJob
{
    public FetchJob(Pair pair, TradeDate tradeDate)
    {
        Pair = pair;
        TradeDate = tradeDate;
    }

    public Pair Pair { get; }
    public TradeDate TradeDate { get; }

    public override string ToString() => $"{Pair} {TradeDate}";

    private static async Task SaveStsAsync(
        ILogger logger, AzureClient client, TickSet tickSet, CancellationToken cancellationToken)
    {
        await client.UploadAsync(tickSet, cancellationToken);

        var blobName = tickSet.GetBlobName(DataKind.STS);

        logger.LogInformation($"Saved {tickSet.Count:N0} ticks to {blobName}");
    }

    public async Task FetchUploadAndLogAsync(
        ILogger logger, AzureClient helper, CancellationToken cancellationToken)
    {
        var session = new Session(TradeDate, Market.Combined);

        var combined = new TickSet(Source.Dukascopy, Pair, session);

        var fetcher = new Fetcher(combined);

        var minHour = session.MinTickOn.Value.Hour;
        var maxHour = session.MaxTickOn.Value.Hour;

        for (var hour = minHour; hour <= maxHour; hour++)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var (success, ticks) = await fetcher.GetTicksAsync(hour, cancellationToken);

            if (!success)
                throw new InvalidDataException($"NO DATA for {this} (Hour: {hour:00})");

            if (ticks != null)
                combined.AddRange(ticks);
        }

        if (combined.Count == 0)
            throw new InvalidDataException($"NO DATA for {this}");

        logger.LogInformation($"FETCHED {combined.Count:N0} {Market.Combined} ticks");

        TickSet GetTickSet(Market market)
        {
            var session = new Session(TradeDate, market);

            var tickSet = new TickSet(Source.Dukascopy, Pair, session);

            foreach (var tick in combined!)
            {
                if (session.InSession(tick.TickOn))
                    tickSet.Add(tick);
            }

            return tickSet;
        }

        if (!cancellationToken.IsCancellationRequested)
            await SaveStsAsync(logger, helper, combined, cancellationToken);

        if (!cancellationToken.IsCancellationRequested)
            await SaveStsAsync(logger, helper, GetTickSet(Market.NewYork), cancellationToken);

        if (!cancellationToken.IsCancellationRequested)
            await SaveStsAsync(logger, helper, GetTickSet(Market.London), cancellationToken);
    }
}