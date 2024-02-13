using SquidEyes.FxData.Helpers;

namespace SquidEyes.FxData.Models;

public readonly struct Rate : IEquatable<Rate>, IComparable<Rate>
{
    private float Value { get; }

    private Rate(float value)
    {
        Value = value;
    }

    public readonly float AsFloat() => Value;

    public override readonly string ToString() => Value.ToString();

    public readonly string ToString(int digits)
    {
        return digits switch
        {
            5 => Value.ToString("N5"),
            3 => Value.ToString("N3"),
            _ => throw new ArgumentOutOfRangeException(nameof(Value))
        };
    }

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

    public static Rate From(float value, int digits)
    {
        if (!IsRateValue(value, digits))
            throw new ArgumentOutOfRangeException(nameof(value));

        return new Rate(value);
    }

    public static Rate Parse(string value, int digits) =>
        From(float.Parse(value), digits);

    public static bool TryParse(string value, int digits, out Rate rate) =>
        Try.GetValue(() => From(float.Parse(value), digits), out rate);

    public static bool IsRateValue(float value, int digits)
    {
        bool IsRounded() => value == MathF.Round(value, digits);

        return digits switch
        {
            5 => value >= 0.00001f && value <= 9.99999f && IsRounded(),
            3 => value >= 0.001f && value <= 999.999f && IsRounded(),
            _ => throw new ArgumentOutOfRangeException(nameof(digits))
        };
    }

    public static bool operator ==(Rate left, Rate right) =>
        left.Equals(right);

    public static bool operator !=(Rate left, Rate right) =>
        !(left == right);

    public static bool operator <(Rate left, Rate right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(Rate left, Rate right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(Rate left, Rate right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(Rate left, Rate right) =>
        left.CompareTo(right) >= 0;
}
