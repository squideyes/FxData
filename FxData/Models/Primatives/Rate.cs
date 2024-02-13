using SquidEyes.FxData.Helpers;

namespace SquidEyes.FxData.Models;

public struct Rate : IEquatable<Rate>, IComparable<Rate>
{
    public const int MinInt32 = 1;
    public const int MaxInt32 = 999999;

    private static readonly float MinF5 = 0.00001f;
    private static readonly float MaxF5 = 9.99999f;
    private static readonly float MinF3 = 0.001f;
    private static readonly float MaxF3 = 999.999f;
    private static readonly float Factor5 = 100000.0f;
    private static readonly float Factor3 = 1000.0f;

    public Rate()
    {
        Value = MinInt32;
    }

    private int Value { get; }

    private Rate(int value)
    {
        Value = value;
    }

    public readonly int Digits => Value > 0 ? 5 : 3;

    public readonly float AsFloat() => GetFloat(Value, Digits);

    public readonly int AsInt32() => Value > 0 ? Value : Value * -1;

    public override readonly string ToString() =>
        AsFloat().ToString(Value > 0 ? "N5" : "N3");

    public readonly bool Equals(Rate other) => Value == other.Value;

    public override readonly bool Equals(object? other) =>
        other is Rate rate && Equals(rate);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public readonly int CompareTo(Rate other)
    {
        if (Value > 0 && other.Value > 0)
            return Value.CompareTo(other.Value);
        else if (Value < 0 && other.Value < 0)
            return other.Value.CompareTo(Value);
        else
            throw new ArgumentOutOfRangeException(nameof(other));
    }

    private static float GetFactor(int digits) =>
        digits == 5 ? Factor5 : Factor3;

    private static float GetFloat(int value, int digits) =>
        FastMath.Round(value / GetFactor(digits), digits);

    public static Rate From(int value, int digits)
    {
        if (value < MinInt32 || value > MaxInt32)
            throw new ArgumentOutOfRangeException(nameof(value));

        return digits switch
        {
            5 => new Rate(value),
            3 => new Rate(value * -1),
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };
    }

    public static Rate From(float value, int digits)
    {
        var factor = digits switch
        {
            5 => Factor5,
            3 => Factor3,
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };

        return From((int)FastMath.Round(value * factor, digits), digits);
    }

    public static Rate Parse(string value, int digits) =>
        From(float.Parse(value), digits);

    public static bool TryParse(string value, int digits, out Rate rate) =>
        Try.GetValue(() => From(float.Parse(value), digits), out rate);

    public static bool IsRateValue(float value, int digits)
    {
        bool IsRounded() => value == FastMath.Round(value, digits);

        return digits switch
        {
            5 => value >= MinF5 && value <= MaxF5 && IsRounded(),
            3 => value >= MinF3 && value <= MaxF3 && IsRounded(),
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };
    }

    public static bool operator ==(Rate lhs, Rate rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(Rate lhs, Rate rhs) =>
        !(lhs == rhs);

    public static Rate operator +(Rate lhs, Rate rhs)
    {
        if (lhs.Value > 0 && rhs.Value > 0)
            return From(lhs.Value + rhs.Value, 5);
        else if (lhs.Value < 0 && rhs.Value < 0)
            return From((lhs.Value * -1) + (rhs.Value * -1), 3);
        else
            throw new ArgumentOutOfRangeException(nameof(rhs));
    }

    public static Rate operator -(Rate lhs, Rate rhs)
    {
        if (lhs.Value > 0 && rhs.Value > 0)
            return From(lhs.Value - rhs.Value, 5);
        else if (lhs.Value < 0 && rhs.Value < 0)
            return From((lhs.Value * -1) - (rhs.Value * -1), 3);
        else
            throw new ArgumentOutOfRangeException(nameof(rhs));
    }

    public static bool operator <(Rate lhs, Rate rhs) =>
        lhs.CompareTo(rhs) < 0;

    public static bool operator <=(Rate lhs, Rate rhs) =>
        lhs.CompareTo(rhs) <= 0;

    public static bool operator >(Rate lhs, Rate rhs) =>
        lhs.CompareTo(rhs) > 0;

    public static bool operator >=(Rate lhs, Rate rhs) =>
        lhs.CompareTo(rhs) >= 0;
}
