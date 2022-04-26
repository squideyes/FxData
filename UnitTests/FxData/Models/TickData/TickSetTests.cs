// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using FluentAssertions;
using SquidEyes.Basics;
using SquidEyes.FxData.Context;
using SquidEyes.FxData.FxData;
using SquidEyes.UnitTests.Testing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;

namespace SquidEyes.UnitTests.FxData;

public class TickSetTests 
{
    [Theory]
    [InlineData(DataKind.CSV, DataKind.CSV)]
    [InlineData(DataKind.CSV, DataKind.STS)]
    [InlineData(DataKind.STS, DataKind.CSV)]
    [InlineData(DataKind.STS, DataKind.STS)]
    public void RoundtripWorks(DataKind sourceKind, DataKind targetKind)
    {
        TickSet.Version.Should().Be(new MajorMinor(3, 0));

        var source = TestHelper.GetTickSet(4, sourceKind);

        var stream = new MemoryStream();

        source.SaveToStream(stream, targetKind);

        stream.Position = 0;

        var target = TestHelper.GetTickSet(4, targetKind);

        target.LoadFromStream(stream, targetKind);

        SourceEqualsTarget(source, target);
    }

    ////////////////////////////

    [Fact]
    public void ClearDoesIndeedClear()
    {
        var tickSet = TestHelper.GetTickSet(4, DataKind.STS);

        tickSet.Count.Should().Be(29948);

        tickSet.Clear();

        tickSet.Count.Should().Be(0);

        tickSet.Add(GetTick(tickSet, 0));

        tickSet.Count.Should().Be(1);

        tickSet.Clear();

        tickSet.Count.Should().Be(0);
    }

    ////////////////////////////

    [Fact]
    public void AddRangeWithGoodArgs()
    {
        var tickSet = TestHelper.GetEmptyTickSet(4);

        var tick1 = GetTick(tickSet, 0);
        var tick2 = GetTick(tickSet, 1);
        var tick3 = GetTick(tickSet, 2);

        tickSet.AddRange(new List<Tick> { tick1, tick2, tick3 });

        tickSet.Count.Should().Be(3);

        tickSet[0].Should().Be(tick1);
        tickSet[1].Should().Be(tick2);
        tickSet[2].Should().Be(tick3);
    }

    ////////////////////////////

    [Fact]
    public void AddRangeWithEmptyList()
    {
        var tickSet = TestHelper.GetEmptyTickSet(4);

        tickSet.AddRange(new List<Tick> { });

        tickSet.Count.Should().Be(0);
    }

    ////////////////////////////

    [Fact]
    public void ToStringReturnsGetFileNameResult()
    {
        var tickSet = TestHelper.GetTickSet(4, DataKind.STS);

        tickSet.ToString().Should().Be(tickSet.GetFileName(DataKind.STS));
    }

    ////////////////////////////

    [Theory]
    [InlineData(DataKind.CSV)]
    [InlineData(DataKind.STS)]
    public void GetMetadataReturnsExpectedValues(DataKind dataKind)
    {
        static DateTime ParseCreatedOn(string value)
        {
            if (value == null)
                return default;
            else
                return DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);
        }

        var tickSet = TestHelper.GetTickSet(4, DataKind.STS);

        var metaData = tickSet.GetMetadata(dataKind);

        var createdOn = ParseCreatedOn(metaData["CreatedOn"]);

        createdOn.Should().NotBe(default);
        createdOn.Kind.Should().Be(DateTimeKind.Utc);

        metaData["Count"].Should().Be(tickSet.Count.ToString());
        metaData["Market"].Should().Be(tickSet.Session.Market.ToString());
        metaData["Pair"].Should().Be(tickSet.Pair.ToString());
        metaData["SaveAs"].Should().Be(dataKind.ToString());
        metaData["Source"].Should().Be(tickSet.Source.ToString());
        metaData["TradeDate"].Should().Be(tickSet.Session.TradeDate.ToString());
        metaData["Version"].Should().Be(TickSet.Version.ToString());
    }

    ////////////////////////////

    [Theory]
    [InlineData(DataKind.CSV, "DC_EURUSD_20160104_NYC_EST.csv")]
    [InlineData(DataKind.STS, "DC_EURUSD_20160104_NYC_EST.sts")]
    public void GoodFileNameGenerated(DataKind dataKind, string fileName) =>
        TestHelper.GetTickSet(4, dataKind).GetFileName(dataKind).Should().Be(fileName);

    ////////////////////////////

    [Theory]
    [InlineData(DataKind.CSV, "DC/TICKSETS/NYC/EURUSD/2016/DC_EURUSD_20160104_NYC_EST.csv")]
    [InlineData(DataKind.STS, "DC/TICKSETS/NYC/EURUSD/2016/DC_EURUSD_20160104_NYC_EST.sts")]
    public void GoodBlobNameGenerated(DataKind dataKind, string blobName) =>
        TestHelper.GetTickSet(4, dataKind).GetBlobName(dataKind).Should().Be(blobName);

    ////////////////////////////

    [Theory]
    [InlineData(DataKind.CSV, @"C:\DC\TICKSETS\NYC\EURUSD\2016\DC_EURUSD_20160104_NYC_EST.csv")]
    [InlineData(DataKind.STS, @"C:\DC\TICKSETS\NYC\EURUSD\2016\DC_EURUSD_20160104_NYC_EST.sts")]
    public void GoodFullPathGenerated(DataKind dataKind, string fullPath) =>
        TestHelper.GetTickSet(4, dataKind).GetFullPath("C:\\", dataKind).Should().Be(fullPath);

    ////////////////////////////

    [Theory]
    [InlineData("DC_EURUSD_20160104_NYC_EST.csv")]
    [InlineData("DC_EURUSD_20160104_NYC_EST.sts")]
    public void GoodFileNamesParsedWithoutError(string fileName)
    {
        var tickSet = TickSet.Create(fileName);

        tickSet.Count.Should().Be(0);
        tickSet.Pair.Should().Be(Known.Pairs[Symbol.EURUSD]);
        tickSet.Session.TradeDate.Should().Be(new TradeDate(2016, 1, 4));
        tickSet.Source.Should().Be(Source.Dukascopy);
    }

    ////////////////////////////

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("XX_EURUSD_20160104_NYC_EST.sts")]
    [InlineData("_EURUSD_20160104_NYC_EST.sts")]
    [InlineData("DC_XXXXXX_20160104_NYC_EST.sts")]
    [InlineData("DC__20160104_NYC_EST.sts")]
    [InlineData("DC_EURUSD_20160103_NYC_EST.sts")]
    [InlineData("DC_EURUSD__NYC_EST.sts")]
    [InlineData("DC_EURUSD_20160104_XXX_EST.sts")]
    [InlineData("DC_EURUSD_20160104__EST.sts")]
    [InlineData("DC_EURUSD_20160104_NYC_XXX.sts")]
    [InlineData("DC_EURUSD_20160104_NYC_.sts")]
    [InlineData("DC_EURUSD_20160104_NYC_EST.xxx")]
    [InlineData("DC_EURUSD_20160104_NYC_EST.")]
    public void CreateWithBadArgsThrowsError(string fileName)
    {
        FluentActions.Invoking(() => TickSet.Create(fileName))
            .Should().Throw<Exception>();
    }

    ////////////////////////////

    [Fact]
    public void AddOutOfTickOnOrder()
    {
        var (pair, session) = TestHelper.GetPairAndSession(4);

        Tick GetTick(int seconds, int bid, int ask) => new(new TickOn(
            session!.MinTickOn.Value.AddSeconds(seconds)), bid, ask);

        var tickSet = new TickSet(Source.Dukascopy, pair, session)
        {
            GetTick(1, 1, 2)
        };

        FluentActions.Invoking(() => tickSet.Add(GetTick(0, 1, 2)))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    ////////////////////////////

    private static Tick GetTick(TickSet tickSet, int msOffset)
    {
        var tickOn = new TickOn(
            tickSet.Session.MinTickOn.Value.AddMilliseconds(msOffset));

        var bid = new Rate(tickSet.Pair.MinValue, 5);

        var ask = new Rate(tickSet.Pair.MaxValue, 5);

        return new Tick(tickOn, bid, ask);
    }

    private static void SourceEqualsTarget(TickSet csv, TickSet sts)
    {
        csv.Count.Should().Be(sts.Count);
        csv.Pair.Should().Be(sts.Pair);
        csv.Session.Should().BeEquivalentTo(sts.Session);
        csv.Source.Should().Be(sts.Source);

        for (var i = 0; i < csv.Count; i++)
            csv[i].Should().Be(sts[i]);
    }
}