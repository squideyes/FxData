//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//using SquidEyes.FxData.Helpers;

//namespace SquidEyes.FxData.Models;

//public struct TickOn : IEquatable<TickOn>, IComparable<TickOn>
//{
//    public TickOn()
//    {
//        throw new InvalidOperationException(
//            "A \"TickOn\" may not be directly constructed!");
//    }

//    public TimeSpan TimeOfDay => Value.TimeOfDay;
//    public long Ticks => Value.Ticks;
//    public int Year => Value.Year;
//    public int Month => Value.Month;
//    public int Day => Value.Day;
//    public int Hour => Value.Hour;
//    public int Minute => Value.Minute;
//    public int Second => Value.Second;
//    public int Millisecond => Value.Millisecond;
//    public DayOfWeek DayOfWeek => Value.DayOfWeek;

//    internal TickOn(DateTime value) => Value = value;

//    internal DateTime Value { get; }

//    public DateTime AsDateTime() => Value;

//    public override string ToString() => Value.ToDateTimeText();

//    public bool Equals(TickOn other) => Value == other.Value;

//    public int CompareTo(TickOn other) => Value.CompareTo(other.Value);

//    public override bool Equals(object? other) =>
//        other is TickOn tickOn && Equals(tickOn);

//    public override int GetHashCode() => Value.GetHashCode();

//    public static TickOn From(DateTime value, Session session)
//    {
//        if (!IsTickOn(value, session))
//            throw new ArgumentOutOfRangeException(nameof(value));

//        return new TickOn(value);
//    }

//    public static TickOn Parse(string value, Session session) =>
//        From(DateTime.Parse(value), session);

//    public static bool TryParse(string value, Session session, out TickOn tickOn) =>
//        Try.GetValue(() => Parse(value, session), out tickOn);

//    public static bool IsTickOn(DateTime value, Session session)
//    {
//        if (value.Kind != DateTimeKind.Unspecified)
//            throw new ArgumentOutOfRangeException(nameof(value));

//        if (session is null)
//            throw new ArgumentNullException(nameof(session));

//        return session.InSession(value);
//    }

//    public static bool operator ==(TickOn lhs, TickOn rhs) =>
//        lhs.Equals(rhs);

//    public static bool operator !=(TickOn lhs, TickOn rhs) =>
//        !(lhs == rhs);

//    public static bool operator <(TickOn lhs, TickOn rhs) =>
//        lhs.CompareTo(rhs) < 0;

//    public static bool operator <=(TickOn lhs, TickOn rhs) =>
//        lhs.CompareTo(rhs) <= 0;

//    public static bool operator >(TickOn lhs, TickOn rhs) =>
//        lhs.CompareTo(rhs) > 0;

//    public static bool operator >=(TickOn lhs, TickOn rhs) =>
//        lhs.CompareTo(rhs) >= 0;
//}