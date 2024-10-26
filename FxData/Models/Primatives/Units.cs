//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//using SquidEyes.Fundamentals;

//namespace SquidEyes.FxData.Models;

//public struct Units : IEquatable<Units>, IComparable<Units>
//{
//    public const int Minimum = 1000;
//    public const int Step = 1000;
//    public const int Maximum = 10000000;

//    public Units() => Value = Minimum;

//    private Units(int value) => Value = value;

//    internal int Value { get; }

//    public override string ToString() => Value.ToString();

//    public int GetSignedUnits(Side side) => 
//        side.IsBuy() ? Value : Value * -1;

//    public override bool Equals(object? other) =>
//        other is Units units && Equals(units);

//    public bool Equals(Units other) => Value.Equals(other.Value);

//    public override int GetHashCode() => Value.GetHashCode();

//    public int CompareTo(Units other) => Value.CompareTo(other.Value);

//    public static Units From(int value)
//    {
//        if (!IsValue(value))
//            throw new ArgumentOutOfRangeException(nameof(value));

//        return new Units(value);
//    }

//    public static Units Parse(string value) => From(int.Parse(value));

//    public static bool IsValue(int value) =>
//        value.IsBetween(Minimum, Maximum) && value % Step == 0;

//    public static bool operator ==(Units left, Units right) =>
//        left.Equals(right);

//    public static bool operator !=(Units left, Units right) =>
//        !left.Equals(right);

//    public static bool operator <(Units left, Units right) =>
//        left.CompareTo(right) < 0;

//    public static bool operator <=(Units left, Units right) =>
//        left.CompareTo(right) <= 0;

//    public static bool operator >(Units left, Units right) =>
//        left.CompareTo(right) > 0;

//    public static bool operator >=(Units left, Units right) =>
//        left.CompareTo(right) >= 0;

//    public static implicit operator int(Units value) => value.Value;
//}