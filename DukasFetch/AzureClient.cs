// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SquidEyes.FxData.Models;

namespace SquidEyes.FxData.DukasFetch;

internal class AzureClient
{
    private readonly BlobContainerClient container;
    private readonly ILogger logger;

    public AzureClient(ILogger logger, Settings setting)
    {
        this.logger = logger;
        container = new BlobContainerClient(setting.ConnString, "fxdata");
    }

    public async Task LoadFromBlobAsync(TickSet tickSet, CancellationToken cancellationToken)
    {
        var blob = container.GetBlobClient(tickSet.GetBlobName(DataKind.STS));

        var result = await blob.DownloadStreamingAsync(
            cancellationToken: cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return;

        tickSet.LoadFromStream(result.Value.Content, DataKind.STS);
    }

    public async Task UploadAsync(Bundle bundle, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();

        bundle.SaveToStream(stream);

        stream.Position = 0;

        var blobName = bundle.BlobName;

        var options = new BlobUploadOptions()
        {
            Metadata = bundle.GetMetadata(),
            HttpHeaders = new BlobHttpHeaders() { ContentType = "application/tickset-bundle" }
        };

        var blob = container.GetBlobClient(blobName);

        if (cancellationToken.IsCancellationRequested)
            return;

        var response = await blob.UploadAsync(stream, options);
    }

    public async Task UploadAsync(TickSet tickSet, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();

        tickSet.SaveToStream(stream, DataKind.STS);

        stream.Position = 0;

        var blobName = tickSet.GetBlobName(DataKind.STS);

        var options = new BlobUploadOptions()
        {
            Metadata = tickSet.GetMetadata(DataKind.STS),
            HttpHeaders = new BlobHttpHeaders() { ContentType = "application/tickset" }
        };

        var blob = container.GetBlobClient(blobName);

        if (cancellationToken.IsCancellationRequested)
            return;

        var response = await blob.UploadAsync(stream, options);
    }

    public async Task<HashSet<string>> GetBlobNamesAsync(
        string folder, CancellationToken cancellationToken)
    {
        try
        {
            var blobNames = new HashSet<string>();

            var prefix = $"{Source.Dukascopy.ToCode()}/{folder}/";

            var resultSegment = container.GetBlobsAsync(prefix: prefix, 
                cancellationToken: cancellationToken).AsPages(default, 5000);

            await foreach (Page<BlobItem> page in resultSegment)
            {
                if(cancellationToken.IsCancellationRequested)
                    return blobNames;

                foreach (BlobItem item in page.Values)
                    blobNames.Add(item.Name);
            }

            return blobNames;
        }
        catch (RequestFailedException e)
        {
            logger.LogWarning($"GetBlobNamesAsync Error (Message: {e.Message}, Container: {container.Name})");

            throw;
        }
    }
}