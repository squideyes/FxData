//using SquidEyes.FxData.Helpers;

//namespace SquidEyes.FxData.Models;

//public struct Rate : IEquatable<Rate>, IComparable<Rate>
//{
//    private const int MinInt32 = 1;
//    private const int MaxInt32 = 999999;
//    private const float FiveZeros = 100000.0f;
//    private const float ThreeZeros = 1000.0f;

//    public Rate()
//    {
//        Value = MinInt32;
//    }

//    private int Value { get; }

//    private Rate(int value)
//    {
//        Value = value;
//    }

//    public float AsFloat() => Value > 0 ?
//        Value / FiveZeros : Value * -1 / ThreeZeros;

//    public int AsInt32() => Value > 0 ? Value : Value * -1;

//    public override string ToString() => Value > 0 ?
//        AsFloat().ToString("N5") : AsFloat().ToString("N3");

//    public bool Equals(Rate other) => Value == other.Value;

//    public override bool Equals(object? other) =>
//        other is Rate rate && Equals(rate);

//    public override int GetHashCode() => Value.GetHashCode();

//    public int CompareTo(Rate other)
//    {
//        if (Value > 0 && other.Value > 0)
//            return Value.CompareTo(other.Value);
//        else if (Value < 0 && other.Value < 0)
//            return other.Value.CompareTo(Value);
//        else
//            throw new ArgumentOutOfRangeException(nameof(other));
//    }

//    public static Rate From(int value, int digits)
//    {
//        if (value < MinInt32 || value > MaxInt32)
//            throw new ArgumentOutOfRangeException(nameof(value));

//        return digits switch
//        {
//            5 => new Rate(value),
//            3 => new Rate(value * -1),
//            _ => throw new ArgumentOutOfRangeException(nameof(digits))
//        };
//    }

//    public static Rate From(float value, int digits) =>
//        From(value * (digits == 5 ? FiveZeros : ThreeZeros), digits);

//    public static Rate Parse(string value, int digits) =>
//        From(int.Parse(value), digits);

//    public static bool TryParse(string value, int digits, out Rate rate) =>
//        Try.GetValue(() => From(float.Parse(value), digits), out rate);

//    public static bool IsRate(float value, int digits)
//    {
//        bool IsRounded() => value == FastMath.Round(value, digits);

//        return digits switch
//        {
//            5 => value >= 0.00001f && value <= 9.99999f && IsRounded(),
//            3 => value >= 0.001f && value <= 999.999f && IsRounded(),
//            _ => throw new ArgumentOutOfRangeException(nameof(digits))
//        };
//    }

//    public static bool operator ==(Rate lhs, Rate rhs) =>
//        lhs.Equals(rhs);

//    public static bool operator !=(Rate lhs, Rate rhs) =>
//        !(lhs == rhs);

//    public static Rate operator +(Rate lhs, Rate rhs)
//    {
//        if (lhs.Value > 0 && rhs.Value > 0)
//            return From(lhs.Value + rhs.Value, 5);
//        else if (lhs.Value < 0 && rhs.Value < 0)
//            return From((lhs.Value * -1) + (rhs.Value * -1), 3);
//        else
//            throw new ArgumentOutOfRangeException(nameof(rhs));
//    }

//    public static Rate operator -(Rate lhs, Rate rhs)
//    {
//        if (lhs.Value > 0 && rhs.Value > 0)
//            return From(lhs.Value - rhs.Value, 5);
//        else if (lhs.Value < 0 && rhs.Value < 0)
//            return From((lhs.Value * -1) - (rhs.Value * -1), 3);
//        else
//            throw new ArgumentOutOfRangeException(nameof(rhs));
//    }

//    public static bool operator <(Rate lhs, Rate rhs) =>
//        lhs.CompareTo(rhs) < 0;

//    public static bool operator <=(Rate lhs, Rate rhs) =>
//        lhs.CompareTo(rhs) <= 0;

//    public static bool operator >(Rate lhs, Rate rhs) =>
//        lhs.CompareTo(rhs) > 0;

//    public static bool operator >=(Rate lhs, Rate rhs) =>
//        lhs.CompareTo(rhs) >= 0;
//}
