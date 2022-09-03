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

//public readonly struct Rate1 : IEquatable<Rate1>, IComparable<Rate1>
//{
//    public const int Minimum = 1;
//    public const int Maximum = 999999;

//    public Rate1() => Value = Minimum;

//    private Rate1(int value) => Value = value;

//    private int Value { get; }

//    public override string ToString() => Value.ToString();

//    public int AsInt32() => Value;

//    public string ToString(int digits)
//    {
//        return digits switch
//        {
//            5 => ToFloat(digits).ToString("N5"),
//            3 => ToFloat(digits).ToString("N3"),
//            _ => throw new ArgumentOutOfRangeException(nameof(digits))
//        };
//    }

//    public float ToFloat(int digits) =>
//        FastMath.Round(Value / GetFactor(digits), digits);

//    public bool Equals(Rate1 other) => Value == other.Value;

//    public override bool Equals(object? other) =>
//        other is Rate1 rate && Equals(rate);

//    public override int GetHashCode() => Value.GetHashCode();

//    public int CompareTo(Rate1 other) => Value.CompareTo(other.Value);

//    public static Rate1 From(int value)
//    {
//        if (value < Minimum || value > Maximum)
//            throw new ArgumentOutOfRangeException(nameof(value));

//        return new Rate1(value);
//    }

//    public static Rate1 From(float value, int digits) =>
//        From((int)FastMath.Round(value * GetFactor(digits)));

//    public static Rate1 Parse(string value, int digits) =>
//        From(float.Parse(value), digits);

//    public static bool TryParse(string value, int digits, out Rate1 rate) =>
//        Try.GetValue(() => Parse(value, digits), out rate);

//    public static bool IsRate(float value, int digits)
//    {
//        bool IsRounded() => value.Equals(FastMath.Round(value, digits));

//        return digits switch
//        {
//            5 => value is >= 0.00001f and <= 9.99999f && IsRounded(),
//            3 => value is >= 0.001f and <= 999.999f && IsRounded(),
//            _ => throw new ArgumentOutOfRangeException(nameof(digits))
//        };
//    }

//    private static float GetFactor(int digits)
//    {
//        return digits switch
//        {
//            5 => 100000.0f,
//            3 => 1000.0f,
//            _ => throw new ArgumentOutOfRangeException(nameof(digits))
//        };
//    }

//    public static Rate1 operator +(Rate1 lhs, Rate1 rhs) =>
//        new(lhs.Value + rhs.Value);

//    public static Rate1 operator -(Rate1 lhs, Rate1 rhs) =>
//        new(lhs.Value - rhs.Value);

//    public static Rate1 operator %(Rate1 lhs, Rate1 rhs) =>
//        new(lhs.Value % rhs.Value);

//    public static bool operator ==(Rate1 lhs, Rate1 rhs) =>
//        lhs.Equals(rhs);

//    public static bool operator !=(Rate1 lhs, Rate1 rhs) =>
//        !(lhs == rhs);

//    public static bool operator <(Rate1 lhs, Rate1 rhs) =>
//        lhs.CompareTo(rhs) < 0;

//    public static bool operator <=(Rate1 lhs, Rate1 rhs) =>
//        lhs.CompareTo(rhs) <= 0;

//    public static bool operator >(Rate1 lhs, Rate1 rhs) =>
//        lhs.CompareTo(rhs) > 0;

//    public static bool operator >=(Rate1 lhs, Rate1 rhs) =>
//        lhs.CompareTo(rhs) >= 0;
//}