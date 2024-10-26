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
//using SquidEyes.FxData;
//using SquidEyes.FxData.Models;
//using SquidEyes.UnitTests.Testing;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using Xunit;

//namespace SquidEyes.UnitTests;

//public class TickSetTests : IClassFixture<TickSetFixture>
//{
//    private readonly TickSetFixture fixture;

//    public TickSetTests(TickSetFixture fixture)
//    {
//        this.fixture = fixture;
//    }

//    [Theory]
//    [InlineData(DataKind.CSV, DataKind.CSV)]
//    [InlineData(DataKind.CSV, DataKind.STS)]
//    [InlineData(DataKind.STS, DataKind.CSV)]
//    [InlineData(DataKind.STS, DataKind.STS)]
//    public void SourceMatchesTarget(DataKind sourceKind, DataKind targetKind)
//    {
//        TickSet.Version.Should().Be(new MajorMinor(1, 0));

//        var source = fixture.TickSets[(4, sourceKind)];
//        var target = fixture.TickSets[(4, targetKind)];

//        SourceEqualsTarget(source, target);
//    }

//    ////////////////////////////

//    [Theory]
//    [InlineData(4)]
//    [InlineData(5)]
//    [InlineData(6)]
//    [InlineData(7)]
//    [InlineData(8)]
//    public void FileCsvToGeneratedSts(int day)
//    {
//        var csv = fixture.TickSets[(day, DataKind.CSV)];

//        var stream = new MemoryStream();

//        csv.SaveToStream(stream, DataKind.STS);

//        stream.Position = 0;

//        var sts = TestHelper.GetEmptyTickSet(day);

//        sts.LoadFromStream(stream, DataKind.STS);

//        SourceEqualsTarget(csv, sts);
//    }

//    ////////////////////////////

//    [Fact]
//    public void SaveToStreamWithBadDataKindThrowsError()
//    {
//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        var stream = new MemoryStream();

//        FluentActions.Invoking(() => tickSet.SaveToStream(stream, 0))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    [Fact]
//    public void SaveEmptyTickSetHasCountOfZero()
//    {
//        var tickSet = TestHelper.GetEmptyTickSet(4);

//        var stream = new MemoryStream();

//        tickSet.SaveToStream(stream, DataKind.STS);

//        tickSet.Count.Should().Be(0);
//    }

//    ////////////////////////////

//    [Fact]
//    public void EmptyTickSetRoundTrips()
//    {
//        var source = TestHelper.GetEmptyTickSet(4);

//        var stream = new MemoryStream();

//        source.SaveToStream(stream, DataKind.STS);

//        stream.Position = 0;

//        var target = TestHelper.GetEmptyTickSet(4);

//        target.LoadFromStream(stream, DataKind.STS);

//        SourceEqualsTarget(source, target);
//    }

//    ////////////////////////////

//    [Fact]
//    public void LoadFromStreamWithBadDataKindThrowsError()
//    {
//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        var stream = new MemoryStream();

//        FluentActions.Invoking(() => tickSet.LoadFromStream(stream, 0))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    [Theory]
//    [InlineData(Source.ForexCom, Symbol.EURUSD, true)]
//    [InlineData(Source.Dukascopy, Symbol.GBPUSD, true)]
//    [InlineData(Source.Dukascopy, Symbol.EURUSD, false)]
//    public void TickSetMismatchOnLoadThrowsError(Source source, Symbol symbol, bool goodVersion)
//    {
//        var session = new Session(TradeDate.MinValue, Market.Combined);

//        var tickSet = new TickSet(source, Known.Pairs[symbol], session);

//        var stream = Properties.TestData.DC_EURUSD_20160104_NYC_EST_STS.ToStream();

//        if (!goodVersion)
//        {
//            var writer = new BinaryWriter(stream);

//            new MajorMinor(2, 0).Write(writer);

//            writer.Flush();

//            stream.Position = 0;
//        }

//        FluentActions.Invoking(() => tickSet.LoadFromStream(stream, DataKind.STS))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    [Fact]
//    public void ClearDoesIndeedClear()
//    {
//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        tickSet.Count.Should().Be(29948);

//        tickSet.Clear();

//        tickSet.Count.Should().Be(0);

//        tickSet.Add(GetTick(tickSet, 0));

//        tickSet.Count.Should().Be(1);

//        tickSet.Clear();

//        tickSet.Count.Should().Be(0);
//    }

//    ////////////////////////////

//    [Fact]
//    public void AddRangeWithGoodArgs()
//    {
//        var tickSet = TestHelper.GetEmptyTickSet(4);

//        var tick1 = GetTick(tickSet, 0);
//        var tick2 = GetTick(tickSet, 1);
//        var tick3 = GetTick(tickSet, 2);

//        tickSet.AddRange(new List<Tick> { tick1, tick2, tick3 });

//        tickSet.Count.Should().Be(3);

//        tickSet[0].Should().Be(tick1);
//        tickSet[1].Should().Be(tick2);
//        tickSet[2].Should().Be(tick3);
//    }

//    ////////////////////////////

//    [Fact]
//    public void AddRangeWithEmptyList()
//    {
//        var tickSet = TestHelper.GetEmptyTickSet(4);

//        tickSet.AddRange(new List<Tick> { });

//        tickSet.Count.Should().Be(0);
//    }

//    ////////////////////////////

//    [Fact]
//    public void AddDefaultTickThrowsError()
//    {
//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        FluentActions.Invoking(() => tickSet.Add(default))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    [Fact]
//    public void AddTickOutOfSessionThrowsError()
//    {
//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        var tickOn = new TickOn(tickSet.First().TickOn.Value.AddDays(1));

//        FluentActions.Invoking(() => tickSet.Add(new Tick(tickOn, 
//            Rate2.From(Rate2.MinInt32, 5), Rate2.From(Rate2.MaxInt32, 5))))
//                .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    [Fact]
//    public void GetFolderPathWithBadBasePathThrowsError()
//    {
//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        FluentActions.Invoking(() => tickSet.GetFullPath("", DataKind.STS))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    [Fact]
//    public void GetFolderPathWithBadDataKindThrowsError()
//    {
//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        FluentActions.Invoking(() => tickSet.GetFullPath("C:\\Data", 0))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    [Fact]
//    public void ToStringReturnsGetFileNameResult()
//    {
//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        tickSet.ToString().Should().Be(tickSet.GetFileName(DataKind.STS));
//    }

//    ////////////////////////////

//    [Theory]
//    [InlineData(DataKind.CSV)]
//    [InlineData(DataKind.STS)]
//    public void GetMetadataReturnsExpectedValues(DataKind dataKind)
//    {
//        static DateTime ParseCreatedOn(string? value) =>
//            value == null ? default : DateTime.Parse(value, null, DateTimeStyles.RoundtripKind);

//        var tickSet = fixture.TickSets[(4, DataKind.STS)];

//        var metaData = tickSet.GetMetadata(dataKind);

//        var createdOn = ParseCreatedOn(metaData["CreatedOn"]);

//        createdOn.Should().NotBe(default);
//        createdOn.Kind.Should().Be(DateTimeKind.Utc);

//        metaData["Count"].Should().Be(tickSet.Count.ToString());
//        metaData["Market"].Should().Be(tickSet.Session.Market.ToString());
//        metaData["Pair"].Should().Be(tickSet.Pair.ToString());
//        metaData["SaveAs"].Should().Be(dataKind.ToString());
//        metaData["Source"].Should().Be(tickSet.Source.ToString());
//        metaData["TradeDate"].Should().Be(tickSet.Session.TradeDate.ToString());
//        metaData["Version"].Should().Be(TickSet.Version.ToString());
//    }

//    ////////////////////////////

//    [Theory]
//    [InlineData(DataKind.CSV, "DC_EURUSD_20160104_NYC_EST.csv")]
//    [InlineData(DataKind.STS, "DC_EURUSD_20160104_NYC_EST.sts")]
//    public void GoodFileNameGenerated(DataKind dataKind, string fileName) =>
//        fixture.TickSets[(4, dataKind)].GetFileName(dataKind).Should().Be(fileName);

//    ////////////////////////////

//    [Theory]
//    [InlineData(DataKind.CSV, "DC/TICKSETS/NYC/EURUSD/2016/DC_EURUSD_20160104_NYC_EST.csv")]
//    [InlineData(DataKind.STS, "DC/TICKSETS/NYC/EURUSD/2016/DC_EURUSD_20160104_NYC_EST.sts")]
//    public void GoodBlobNameGenerated(DataKind dataKind, string blobName) =>
//        fixture.TickSets[(4, dataKind)].GetBlobName(dataKind).Should().Be(blobName);

//    ////////////////////////////

//    [Theory]
//    [InlineData(DataKind.CSV, @"C:\DC\TICKSETS\NYC\EURUSD\2016\DC_EURUSD_20160104_NYC_EST.csv")]
//    [InlineData(DataKind.STS, @"C:\DC\TICKSETS\NYC\EURUSD\2016\DC_EURUSD_20160104_NYC_EST.sts")]
//    public void GoodFullPathGenerated(DataKind dataKind, string fullPath) =>
//        fixture.TickSets[(4, dataKind)].GetFullPath("C:\\", dataKind).Should().Be(fullPath);

//    ////////////////////////////

//    [Fact]
//    public void CreateWithTooManyFileNameFieldsThrowsError()
//    {
//        FluentActions.Invoking(() => TickSet.Create("DC_EURUSD_20160104_NYC_EST_XXX.sts"))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    [Theory]
//    [InlineData("DC_EURUSD_20160104_NYC_EST.csv")]
//    [InlineData("DC_EURUSD_20160104_NYC_EST.sts")]
//    public void GoodFileNamesParsedWithoutError(string fileName)
//    {
//        var tickSet = TickSet.Create(fileName);

//        tickSet.Count.Should().Be(0);
//        tickSet.Pair.Should().Be(Known.Pairs[Symbol.EURUSD]);
//        tickSet.Session.TradeDate.Should().Be(TradeDate.From(2016, 1, 4));
//        tickSet.Source.Should().Be(Source.Dukascopy);
//    }

//    ////////////////////////////

//    [Theory]
//    [InlineData(null)]
//    [InlineData("")]
//    [InlineData(" ")]
//    [InlineData("XX_EURUSD_20160104_NYC_EST.sts")]
//    [InlineData("_EURUSD_20160104_NYC_EST.sts")]
//    [InlineData("DC_XXXXXX_20160104_NYC_EST.sts")]
//    [InlineData("DC__20160104_NYC_EST.sts")]
//    [InlineData("DC_EURUSD_20160103_NYC_EST.sts")]
//    [InlineData("DC_EURUSD_20320101_NYC_EST.sts")]
//    [InlineData("DC_EURUSD__NYC_EST.sts")]
//    [InlineData("DC_EURUSD_20160104_XXX_EST.sts")]
//    [InlineData("DC_EURUSD_20160104__EST.sts")]
//    [InlineData("DC_EURUSD_20160104_NYC_XXX.sts")]
//    [InlineData("DC_EURUSD_20160104_NYC_.sts")]
//    [InlineData("DC_EURUSD_20160104_NYC_EST.xxx")]
//    [InlineData("DC_EURUSD_20160104_NYC_EST.")]
//    public void CreateWithBadArgsThrowsError(string fileName)
//    {
//        FluentActions.Invoking(() => TickSet.Create(fileName))
//            .Should().Throw<Exception>();
//    }

//    ////////////////////////////

//    [Fact]
//    public void AddOutOfTickOnOrder()
//    {
//        var (pair, session) = TestHelper.GetPairAndSession(4);

//        Tick GetTick(int seconds, int bid, int ask) => new(new TickOn(
//            session!.MinTickOn.Value.AddSeconds(seconds)),
//                Rate2.From(bid, 5), Rate2.From(ask, 5));

//        var tickSet = new TickSet(Source.Dukascopy, pair, session)
//        {
//            GetTick(1, 1, 2)
//        };

//        FluentActions.Invoking(() => tickSet.Add(GetTick(0, 1, 2)))
//            .Should().Throw<ArgumentOutOfRangeException>();
//    }

//    ////////////////////////////

//    private static Tick GetTick(TickSet tickSet, int msOffset)
//    {
//        var tickOn = new TickOn(
//            tickSet.Session.MinTickOn.Value.AddMilliseconds(msOffset));

//        var bid = Rate2.From(tickSet.Pair.MinValue, 5);

//        var ask = Rate2.From(tickSet.Pair.MaxValue, 5);

//        return new Tick(tickOn, bid, ask);
//    }

//    private static void SourceEqualsTarget(TickSet csv, TickSet sts)
//    {
//        csv.Count.Should().Be(sts.Count);
//        csv.Pair.Should().Be(sts.Pair);
//        csv.Session.Should().BeEquivalentTo(sts.Session);
//        csv.Source.Should().Be(sts.Source);

//        for (var i = 0; i < csv.Count; i++)
//            csv[i].Should().Be(sts[i]);
//    }
//}