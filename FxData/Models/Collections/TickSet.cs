// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.FxData.Context;
using System.IO.Compression;
using System.Text;

namespace SquidEyes.FxData.Models;

public class TickSet : ListBase<Tick>
{
    private const byte MINIFIED = 0b1000_0000;
    private const byte MINIFIEDTICKONMASK = 0b0111_1111;
    private const byte PACKED = 0b0000_0000;
    private const byte TICKON1 = 0b0001_0000;
    private const byte TICKON2 = 0b0010_0000;
    private const byte TICKON4 = 0b0011_0000;
    private const byte BID1 = 0b0000_0100;
    private const byte BID2 = 0b0000_1000;
    private const byte BID4 = 0b0000_1100;
    private const byte ASK1 = 0b0000_0001;
    private const byte ASK2 = 0b0000_0010;
    private const byte ASK4 = 0b0000_0011;

    public static readonly MajorMinor Version = new(1, 0);

    private TickOn? lastTickOn = null;

    public TickSet(Source source, Pair pair, Session session)
    {
        Source = source.Validated(nameof(source), v => v.IsEnumValue());

        Pair = pair.Validated(
            nameof(pair), v => Known.Pairs.ContainsKey(pair.Symbol));

        Session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public Source Source { get; }
    public Pair Pair { get; }
    public Session Session { get; }

    public Tick this[int index] => Items[index];

    public void Clear()
    {
        Items.Clear();

        lastTickOn = null;
    }

    public void Add(Tick tick)
    {
        if (tick == default)
            throw new ArgumentOutOfRangeException(nameof(tick));

        if (!Session.InSession(tick.TickOn))
            throw new ArgumentOutOfRangeException(nameof(tick));

        if (lastTickOn.HasValue && tick.TickOn < lastTickOn)
            throw new ArgumentOutOfRangeException(nameof(tick));

        lastTickOn = tick.TickOn;

        Items.Add(tick);
    }

    public void AddRange(IEnumerable<Tick> ticks) =>
        ticks.ForEach(tick => Add(tick));

    public string GetFileName(DataKind dataKind)
    {
        var sb = new StringBuilder();

        sb.Append(Source.ToCode());
        sb.AppendDelimited(Pair, '_');
        sb.AppendDelimited(Session.TradeDate.Value.ToString("yyyyMMdd"), '_');
        sb.AppendDelimited(Session.Market.ToCode(), '_');
        sb.Append("_EST.");
        sb.Append(dataKind.ToLower());

        return sb.ToString();
    }

    public string GetBlobName(DataKind dataKind)
    {
        var sb = new StringBuilder();

        sb.Append(Source.ToCode());
        sb.AppendDelimited("TICKSETS", '/');
        sb.AppendDelimited(Session.Market.ToCode(), '/');
        sb.AppendDelimited(Pair, '/');
        sb.AppendDelimited(Session.TradeDate.Value.Year, '/');
        sb.AppendDelimited(GetFileName(dataKind), '/');

        return sb.ToString();
    }

    public string GetFullPath(string basePath, DataKind dataKind)
    {
        if (!basePath.IsFolderName())
            throw new ArgumentOutOfRangeException(nameof(basePath));

        if (!dataKind.IsEnumValue())
            throw new ArgumentOutOfRangeException(nameof(dataKind));

        var sb = new StringBuilder();

        sb.Append(Source.ToCode());
        sb.AppendDelimited("TICKSETS", Path.DirectorySeparatorChar);
        sb.AppendDelimited(Session.Market.ToCode(), Path.DirectorySeparatorChar);
        sb.AppendDelimited(Pair, Path.DirectorySeparatorChar);
        sb.AppendDelimited(Session.TradeDate.Value.Year, Path.DirectorySeparatorChar);
        sb.AppendDelimited(GetFileName(dataKind), Path.DirectorySeparatorChar);

        return Path.Combine(basePath, sb.ToString());
    }

    public override string ToString() => GetFileName(DataKind.STS);

    public Dictionary<string, string> GetMetadata(DataKind dataKind)
    {
        return new Dictionary<string, string>
        {
            { "Count", Count.ToString() },
            { "CreatedOn", DateTime.UtcNow.ToString("O") },
            { "Market", Session.Market.ToString() },
            { "Pair", Pair.ToString() },
            { "SaveAs", dataKind.ToString() },
            { "Source", Source.ToString() },
            { "TradeDate", Session.TradeDate.ToString() },
            { "Version", Version.ToString() }
        };
    }

    public void SaveToStream(Stream stream, DataKind dataKind)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!dataKind.IsEnumValue())
            throw new ArgumentOutOfRangeException(nameof(dataKind));

        if (dataKind == DataKind.CSV)
        {
            var writer = new StreamWriter(stream);

            foreach (var tick in this)
                writer.WriteLine(tick.ToCsvString(Pair));

            writer.Flush();
        }
        else
        {
            var data = new MemoryStream();

            var writer = new BinaryWriter(data);

            Version.Write(writer);
            writer.Write(DateTime.UtcNow.Ticks);
            writer.Write(Source.ToString());
            writer.Write(Pair.ToString());
            writer.Write(Session.TradeDate.Value.DayNumber);
            writer.Write(Session.Market.ToString());
            writer.Write(Count);

            var lastTick = First();

            writer.Write(lastTick.TickOn.Value.Ticks);
            writer.Write(lastTick.Bid.Value);
            writer.Write(lastTick.Ask.Value);

            foreach (var tick in this.Skip(1))
            {
                var tickOnDelta = (int)(tick.TickOn.Value - lastTick.TickOn.Value).TotalMilliseconds;
                var bidDelta = tick.Bid.Value - lastTick.Bid.Value;
                var askDelta = tick.Ask.Value - lastTick.Ask.Value;

                if (tickOnDelta <= 64
                    && bidDelta >= -7 && bidDelta <= 7
                    && askDelta >= -7 && askDelta <= 7)
                {
                    var bidData = (bidDelta >= 0 ?
                        0b0000_0000 : 0b1000_0000) | (Math.Abs(bidDelta) << 4);

                    var askData = (askDelta >= 0 ?
                        0b0000_0000 : 0b0000_1000) | (Math.Abs(askDelta));

                    writer.Write((byte)(0b1000_0000 | tickOnDelta));
                    writer.Write((byte)(bidData | askData));
                }
                else
                {
                    var header = PACKED;

                    if (tickOnDelta != 0)
                        header |= GetTickOnFlags(tickOnDelta);

                    if (bidDelta != 0)
                        header |= GetBidFlags(bidDelta);

                    if (askDelta != 0)
                        header |= GetAskFlags(askDelta);

                    if (tickOnDelta != 0 || bidDelta != 0 || askDelta != 0)
                    {
                        writer.Write(header);

                        if (tickOnDelta != 0)
                            writer.Write(GetBytes(tickOnDelta));

                        if (bidDelta != 0)
                            writer.Write(GetBytes(bidDelta));

                        if (askDelta != 0)
                            writer.Write(GetBytes(askDelta));
                    }
                }

                lastTick = tick;
            }

            writer.Flush();

            using GZipStream gzip = new(stream, CompressionLevel.Optimal, true);

            data.Position = 0;

            data.CopyTo(gzip);
        }
    }

    public void LoadFromStream(Stream stream, DataKind dataKind)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!dataKind.IsEnumValue())
            throw new ArgumentOutOfRangeException(nameof(dataKind));

        Items.Clear();

        lastTickOn = null;

        if (dataKind == DataKind.CSV)
        {
            foreach (var fields in new CsvEnumerator(stream, 3))
            {
                var tickOn = new TickOn(DateTime.Parse(fields[0]), Session);
                var bid = new Rate(float.Parse(fields[1]), Pair.Digits);
                var ask = new Rate(float.Parse(fields[2]), Pair.Digits);

                Add(new Tick(tickOn, bid, ask));
            }
        }
        else
        {
            using var decompressed = new MemoryStream();

            using var gzip = new GZipStream(
                stream, CompressionMode.Decompress, true);

            gzip.CopyTo(decompressed);

            var reader = new BinaryReader(decompressed);

            decompressed.Position = 0;

            if (MajorMinor.Read(reader) != Version)
                throw new ArgumentOutOfRangeException(nameof(stream));

            _ = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);

            if (reader.ReadString().ToEnumValue<Source>() != Source)
                throw new ArgumentOutOfRangeException(nameof(stream));

            if (reader.ReadString().ToEnumValue<Symbol>() != Pair.Symbol)
                throw new ArgumentOutOfRangeException(nameof(stream));

            var tradeDate = new TradeDate(
                DateOnly.FromDayNumber(reader.ReadInt32()));

            var market = reader.ReadString().ToEnumValue<Market>();

            _ = new Session(tradeDate, market);

            var count = reader.ReadInt32();

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var tickOn = new TickOn(new DateTime(reader.ReadInt64()));
            var bid = new Rate(reader.ReadInt32());
            var ask = new Rate(reader.ReadInt32());

            var lastTick = new Tick(tickOn, bid, ask);

            Items.Add(lastTick);

            for (var i = 1; i < count; i++)
            {
                var header = reader.ReadByte();

                Tick tick;

                if ((header & MINIFIED) == MINIFIED)
                {
                    tickOn = new TickOn(lastTick.TickOn.Value
                        .AddMilliseconds(header & MINIFIEDTICKONMASK));

                    (bid, ask) = ReadMinimizedBidAndAsk(reader, lastTick);

                    tick = new Tick(tickOn, bid, ask);
                }
                else
                {
                    tickOn = ReadTickOn(reader, header, lastTick);
                    bid = ReadBid(reader, header, lastTick);
                    ask = ReadAsk(reader, header, lastTick);

                    tick = new Tick(tickOn, bid, ask);
                }

                Items.Add(tick);

                lastTick = tick;
            }
        }
    }

    private static int ReadValue(
        BinaryReader reader, byte header, byte mask, int shift)
    {
        return ((header & mask) >> shift) switch
        {
            0 => 0,
            1 => (sbyte)reader.ReadByte(),
            2 => reader.ReadInt16(),
            _ => reader.ReadInt32(),
        };
    }

    private static (Rate, Rate) ReadMinimizedBidAndAsk(BinaryReader reader, Tick lastTick)
    {
        const int NEGATIVE = 0b000_1000;
        const int MASK = 0b000_0111;

        int value = reader.ReadByte();

        var bidData = (value & 0b1111_0000) >> 4;

        var askData = value & 0b0000_1111;

        var bidDelta = (bidData & MASK) *
            ((bidData & NEGATIVE) == NEGATIVE ? -1 : 1);

        var askDelta = (askData & MASK) *
            ((askData & NEGATIVE) == NEGATIVE ? -1 : 1);

        var bid = new Rate(lastTick.Bid.Value + bidDelta);

        var ask = new Rate(lastTick.Ask.Value + askDelta);

        return (bid, ask);
    }

    private static TickOn ReadTickOn(BinaryReader reader, byte header, Tick lastTick)
    {
        return new TickOn(lastTick.TickOn.Value.AddMilliseconds(
            ReadValue(reader, header, 0b0011_0000, 4)));
    }

    private static Rate ReadBid(BinaryReader reader, byte header, Tick lastTick) =>
        new(lastTick.Bid.Value + ReadValue(reader, header, 0b0000_1100, 2));

    private static Rate ReadAsk(BinaryReader reader, byte header, Tick lastTick) =>
        new(lastTick.Ask.Value + ReadValue(reader, header, 0b0000_0011, 0));

    private static byte GetTickOnFlags(int value) =>
        GetFlags(value, TICKON1, TICKON2, TICKON4);

    private static byte GetBidFlags(int value) =>
        GetFlags(value, BID1, BID2, BID4);

    private static byte GetAskFlags(int value) =>
        GetFlags(value, ASK1, ASK2, ASK4);

    private static byte[] GetBytes(int value)
    {
        if (value == 0)
            return Array.Empty<byte>();
        else if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
            return new[] { (byte)value };
        else if (value >= short.MinValue && value <= short.MaxValue)
            return BitConverter.GetBytes((short)value);
        else
            return BitConverter.GetBytes(value);
    }

    private static byte GetFlags(int value, byte flag1, byte flag2, byte flag4)
    {
        if (value == 0)
            return 0b0000_0000;
        else if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
            return flag1;
        else if (value >= short.MinValue && value <= short.MaxValue)
            return flag2;
        else
            return flag4;
    }

    public static TickSet Create(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentOutOfRangeException(nameof(fileName));

        if (!Enum.TryParse(Path.GetExtension(fileName)[1..], true, out DataKind _))
            throw new ArgumentOutOfRangeException(nameof(fileName));

        var fields = Path.GetFileNameWithoutExtension(fileName).Split('_');

        if (fields.Length != 5)
            throw new ArgumentOutOfRangeException(nameof(fileName));

        var source = fields[0].ToSource();

        if (!Enum.TryParse(fields[1], out Symbol symbol))
            throw new ArgumentOutOfRangeException(nameof(fileName));

        var pair = Known.Pairs[symbol];

        var session = new Session(
            new TradeDate(DateOnly.ParseExact(fields[2], "yyyyMMdd", null)), 
            fields[3].ToMarket());

        if (fields[4] != "EST")
            throw new ArgumentOutOfRangeException(nameof(fileName));

        return new TickSet(source, pair, session);
    }
}