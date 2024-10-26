﻿//namespace SquidEyes.FxData.Models;

//public readonly struct Rate : IEquatable<Rate>, IComparable<Rate>
//{
//    public const int MinInt32 = 1;
//    public const int MaxInt32 = 999999;

//    private const double Factor5 = 100000.0;
//    private const double Factor3 = 1000.0;

//    private Rate(int value) => Value = value;

//    private Rate(int value, int digits)
//    {
//        if (value < MinInt32 || value > MaxInt32)
//            throw new ArgumentOutOfRangeException(nameof(value));

//        Value = digits == 5 ? value : value * -1;
//    }

//    public int Value { get; }

//    public int Digits
//    {
//        get
//        {
//            if (Value > 0)
//                return 5;
//            else if (Value < 0)
//                return 3;
//            else
//                return 0;
//        }
//    }

//    public double AsDouble()
//    {
//        if (Digits == 5)
//        {
//            var shifted = Value / Factor5;
//            var rounded = shifted >= 0 ? shifted + 0.5e-5 : shifted - 0.5e-5;
//            return ((long)(rounded * Factor5)) / Factor5;
//        }
//        else if (Digits == 3)
//        {
//            var shifted = Value * -1 / Factor3;
//            var rounded = shifted >= 0 ? shifted + 0.5e-3 : shifted - 0.5e-3;
//            return ((long)(rounded * Factor3)) / Factor3;
//        }
//        else
//        {
//            throw new InvalidOperationException();
//        }
//    }

//    public override string ToString()
//    {
//        var asDouble = AsDouble();

//        return Digits switch
//        {
//            5 => asDouble.ToString("N5"),
//            3 => asDouble.ToString("N3"),
//            _ => throw new InvalidOperationException()
//        };
//    }

//    public bool Equals(Rate other) => Value == other.Value;

//    public override bool Equals(object? other) =>
//        other is Rate rate && Equals(rate);

//    public override int GetHashCode() => Value.GetHashCode();

//    public int CompareTo(Rate other)
//    {
//        return Digits switch
//        {
//            5 => Value.CompareTo(other.Value),
//            3 => other.Value.CompareTo(Value),
//            _ => throw new InvalidOperationException()
//        };
//    }

//    public static Rate Empty => new();

//    internal static Rate Create(int value) => new(value);

//    internal static Rate Create(double value, int digits)
//    {
//        var asInt32 = digits switch
//        {
//            5 => (int)(value * Factor5),
//            3 => (int)(value * Factor3),
//            _ => throw new InvalidOperationException()
//        };

//        return new Rate(asInt32, digits);
//    }

//    internal static Rate Parse(string input, int digits) =>
//        Create(double.Parse(input), digits);

//    public static bool operator ==(Rate left, Rate right) =>
//        left.Equals(right);

//    public static bool operator !=(Rate left, Rate right) =>
//        !(left == right);

//    public static bool operator >(Rate left, Rate right) =>
//        left.CompareTo(right) > 0;

//    public static bool operator >=(Rate left, Rate right) =>
//        left.CompareTo(right) >= 0;

//    public static bool operator <(Rate left, Rate right) =>
//        left.CompareTo(right) < 0;

//    public static bool operator <=(Rate left, Rate right) =>
//        left.CompareTo(right) <= 0;
//}
