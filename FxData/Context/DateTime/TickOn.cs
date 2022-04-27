// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.FxData.Context;

public struct TickOn : IEquatable<TickOn>, IComparable<TickOn>
{
    public TickOn(DateTime value, Session session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        if (!session.InSession(value))
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public TickOn() => throw new InvalidOperationException();

    internal TickOn(DateTime value) => Value = value;

    public DateTime Value { get; private set; }

    public bool IsEmpty => Value == default;

    public TradeDate TradeDate => new(DateOnly.FromDateTime(Value.Date));

    public override string ToString() => Value.ToDateTimeText();

    public int CompareTo(TickOn other) => Value.CompareTo(other.Value);

    public bool Equals(TickOn other) => Value.Equals(other.Value);

    public override bool Equals(object? other) =>
        other is TickOn tickOn && Equals(tickOn);

    public override int GetHashCode() => Value.GetHashCode();

    public static TickOn Parse(string value, Session session) =>
        new(DateTime.Parse(value), session);

    public static bool operator ==(TickOn left, TickOn right) =>
        left.Equals(right);

    public static bool operator !=(TickOn left, TickOn right) =>
        !(left == right);

    public static bool operator <(TickOn left, TickOn right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(TickOn left, TickOn right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(TickOn left, TickOn right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(TickOn left, TickOn right) =>
        left.CompareTo(right) >= 0;
}