// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.FxData.Models;
using System.Text;
using static SquidEyes.FxData.Models.Source;

namespace SquidEyes.FxData.DukasFetch;

internal class Worker : BackgroundService
{
    private readonly IHost host;
    private readonly ILogger logger;
    private readonly Settings settings;
    private readonly AzureClient client;

    public Worker(IHost host, ILogger<Worker> logger, Settings settings)
    {
        this.host = host;
        this.logger = logger;
        this.settings = settings;

        client = new AzureClient(logger, settings);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();

        sb.Append($"Symbols: {string.Join(",", settings.Symbols!)}");
        sb.Append($"; MinYear: {settings.MinYear}");
        sb.Append($"; Replace: {settings.Replace}");
        sb.Append($"; ConnString: \"{settings.ConnString}\"");

        logger.LogInformation(sb.ToString());

        var date = DateOnly.FromDateTime(
            DateTime.UtcNow.ToEasternFromUtc().AddDays(-1).Date);

        while (!Known.IsTradeDate(date))
            date = date.AddDays(-1);

        var maxTradeDate = new TradeDate(date);

        if (await ProcessFetchJobsAsync(maxTradeDate, cancellationToken))
            await ProcessBundleJobsAsync(maxTradeDate, cancellationToken);

        await host.StopAsync(cancellationToken);
    }

    private async Task ProcessBundleJobsAsync(
        TradeDate maxTradeDate, CancellationToken cancellationToken)
    {
        var (jobs, skipped) = await GetBundleJobsAsync(maxTradeDate, cancellationToken);

        if (jobs.Count == 0)
        {
            logger.LogWarning("There are NO MISSING bundle files!");
        }
        else
        {
            logger.LogInformation($"ENQUEUED {jobs.Count:N0} BUNDLE jobs");

            try
            {
                foreach (var job in jobs)
                    await job.BundleSaveAndLogAsync(logger, client, cancellationToken);

                logger.LogInformation(
                    $"CREATED {jobs.Count:N0} bundles (skipped {skipped:N0})");
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
            }
        }
    }

    private async Task<bool> ProcessFetchJobsAsync(
        TradeDate maxTradeDate, CancellationToken cancellationToken)
    {
        var (jobs, skipped) = await GetFetchJobsAsync(maxTradeDate, cancellationToken);

        if (jobs.Count == 0)
        {
            logger.LogWarning("There are NO UNFETCHED tick-data files!");

            return true;
        }
        else
        {
            logger.LogInformation($"ENQUEUED {jobs.Count:N0} tick-data FETCH + SAVE jobs");

            try
            {
                foreach (var job in jobs)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    await job.FetchUploadAndLogAsync(logger, client, cancellationToken);
                }

                logger.LogInformation(
                    $"FETCHED & SAVED {jobs.Count:N0} tick-sets (skipped {skipped:N0})");

                return true;
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);

                return false;
            }
        }
    }

    private async Task<(List<BundleJob> Jobs, int Skipped)> GetBundleJobsAsync(
        TradeDate maxTradeDate, CancellationToken cancellationToken)
    {
        var jobs = new List<BundleJob>();

        var skipped = 0;

        var blobNames = await client.GetBlobNamesAsync("BUNDLES", cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return (jobs, skipped);

        var yms = GetTradeDates(maxTradeDate).Select(d => (d.Value.Year, d.Value.Month))
            .Distinct().OrderBy(d => d).ToList().AsFunc(d => d.Take(d.Count - 1)).ToList();

        foreach (var (Year, Month) in yms)
        {
            foreach (var market in EnumList.FromAll<Market>())
            {
                foreach (var pair in Known.Pairs.Values)
                {
                    var bundle = new Bundle(Dukascopy, pair, Year, Month, market);

                    if (blobNames.Contains(bundle.BlobName))
                        skipped++;
                    else
                        jobs.Add(new BundleJob(pair.Symbol, Year, Month, market));
                }
            }
        }

        return (jobs, skipped);
    }

    private List<TradeDate> GetTradeDates(TradeDate maxTradeDate)
    {
        return Known.TradeDates!.Where(
            d => d.Value.Year >= settings.MinYear && d <= maxTradeDate).ToList();
    }

    private async Task<(List<FetchJob> Jobs, int Skipped)> GetFetchJobsAsync(
        TradeDate maxTradeDate, CancellationToken cancellationToken)
    {
        var jobs = new List<FetchJob>();

        var skipped = 0;

        var blobNames = await client.GetBlobNamesAsync(
            "TICKSETS", cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return (jobs, skipped);

        bool Exists(Pair pair, TradeDate tradeDate)
        {
            foreach (var market in EnumList.FromAll<Market>())
            {
                var tickSet = new TickSet(
                    Dukascopy, pair, new Session(tradeDate, market));

                if (!blobNames.Contains(tickSet.GetBlobName(DataKind.STS)))
                    return false;
            }

            return true;
        }

        var tradeDates = GetTradeDates(maxTradeDate);

        foreach (var pair in settings.Symbols!.Select(s => Known.Pairs[s]))
        {
            foreach (var tradeDate in tradeDates)
            {
                if (Exists(pair, tradeDate))
                    skipped++;
                else
                    jobs.Add(new FetchJob(pair, tradeDate));
            }
        }

        return (jobs, skipped);
    }
}