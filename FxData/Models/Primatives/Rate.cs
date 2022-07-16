// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.FxData.Helpers;

namespace SquidEyes.FxData.Models;

public readonly struct Rate : IEquatable<Rate>, IComparable<Rate>
{
    public const int Minimum = 1;
    public const int Maximum = 999999;

    public Rate() => Value = Minimum;

    private Rate(int value) => Value = value;

    internal int Value { get; }

    public bool IsEmpty => Value.IsDefaultValue();

    public override string ToString() => Value.ToString();

    public string ToString(int digits)
    {
        return digits switch
        {
            5 => AsFloat(digits).ToString("N5"),
            3 => AsFloat(digits).ToString("N3"),
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };
    }

    public float AsFloat(int digits) =>
        FastMath.Round(Value / GetFactor(digits), digits);

    public bool Equals(Rate other) => Value == other.Value;

    public override bool Equals(object? other) =>
        other is Rate rate && Equals(rate);

    public override int GetHashCode() => Value.GetHashCode();

    public int CompareTo(Rate other) => Value.CompareTo(other.Value);

    public static Rate From(int value)
    {
        if (value < Minimum || value > Maximum)
            throw new ArgumentOutOfRangeException(nameof(value));

        return new Rate(value);
    }

    public static Rate From(float value, int digits) =>
        From((int)FastMath.Round(value * GetFactor(digits)));

    public static Rate Parse(string value, int digits) =>
        From(float.Parse(value), digits);

    public static bool IsRate(float value, int digits)
    {
        bool IsRounded() => value.Equals(FastMath.Round(value, digits));

        return digits switch
        {
            5 => value is >= 0.00001f and <= 9.99999f && IsRounded(),
            3 => value is >= 0.001f and <= 999.999f && IsRounded(),
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };
    }

    private static float GetFactor(int digits)
    {
        return digits switch
        {
            5 => 100000.0f,
            3 => 1000.0f,
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };
    }

    public static Rate operator +(Rate lhs, Rate rhs) =>
        new(lhs.Value + rhs.Value);

    public static Rate operator -(Rate lhs, Rate rhs) =>
        new(lhs.Value - rhs.Value);

    public static Rate operator %(Rate lhs, Rate rhs) =>
        new(lhs.Value % rhs.Value);

    public static bool operator ==(Rate lhs, Rate rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(Rate lhs, Rate rhs) =>
        !(lhs == rhs);

    public static bool operator <(Rate lhs, Rate rhs) =>
        lhs.CompareTo(rhs) < 0;

    public static bool operator <=(Rate lhs, Rate rhs) =>
        lhs.CompareTo(rhs) <= 0;

    public static bool operator >(Rate lhs, Rate rhs) =>
        lhs.CompareTo(rhs) > 0;

    public static bool operator >=(Rate lhs, Rate rhs) =>
        lhs.CompareTo(rhs) >= 0;

    public static implicit operator int(Rate rate) => rate.Value;
}