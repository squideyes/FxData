// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;

namespace SquidEyes.FxData.Models;

public struct Tick : IEquatable<Tick>
{
    public Tick(TickOn tickOn, Rate bid, Rate ask)
    {
        TickOn = tickOn.Validated(nameof(tickOn), v => !v.IsDefaultValue());

        Bid = bid.Validated(nameof(bid), v => !v.IsDefaultValue());

        Ask = ask.Validated(nameof(ask), v => !v.IsDefaultValue() && v >= bid);
    }

    public TickOn TickOn { get; }
    public Rate Bid { get; }
    public Rate Ask { get; }

    public Rate Spread => Rate.From(Ask.Value - Bid.Value);

    public bool IsEmpty => TickOn.IsDefaultValue();

    public bool InSession(Session session) => session.InSession(TickOn);

    public override string ToString() => $"{TickOn},{Bid},{Ask}";

    public string ToCsvString(Pair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);

        return ToCsvString(pair.Digits);
    }

    public string ToCsvString(int digits) =>
        $"{TickOn},{Bid.ToString(digits)},{Ask.ToString(digits)}";

    public bool Equals(Tick other) => TickOn == other.TickOn
        && Bid == other.Bid && Ask == other.Ask;

    public override bool Equals(object? other) =>
        other is Tick tick && Equals(tick);

    public override int GetHashCode() =>
        (TickOn, Bid, Ask).GetHashCode();

    public static bool operator ==(Tick lhs, Tick rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(Tick lhs, Tick rhs) =>
        !(lhs == rhs);

    public static Tick Parse(string value, Pair pair, Session session)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        if (pair is null)
            throw new ArgumentNullException(nameof(pair));

        if (session is null)
            throw new ArgumentNullException(nameof(session));

        var fields = value.Split(',');

        if (fields.Length != 3)
            throw new ArgumentOutOfRangeException(nameof(value));

        var tickOn = TickOn.Parse(fields[0], session);
        var bid = Rate.From(float.Parse(fields[1]), pair.Digits);
        var ask = Rate.From(float.Parse(fields[2]), pair.Digits);

        return new Tick(tickOn, bid, ask);
    }
}