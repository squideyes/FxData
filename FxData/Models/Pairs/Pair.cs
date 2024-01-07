// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Fundamentals;

namespace SquidEyes.FxData.Models;

public class Pair : IEquatable<Pair>
{
    private readonly string format;

    public Pair(Symbol symbol, int digits)
    {
        Symbol = symbol.MustBe().EnumValue();
        Digits = digits.MustBe().True(v => v.In(3, 5));

        format = "N" + digits;

        Factor = (int)MathF.Pow(10, digits);
        OnePip = MathF.Pow(10, -(digits - 1));
        OneTick = MathF.Pow(10, -digits);
        MaxValue = Round((OneTick * 1000000.0f) - OneTick);
        Base = symbol.ToString()[0..3].ToEnumValue<Currency>();
        Quote = symbol.ToString()[3..].ToEnumValue<Currency>();
    }

    public Symbol Symbol { get; }
    public int Digits { get; }
    public float Factor { get; }
    public Currency Base { get; }
    public Currency Quote { get; }
    public float OnePip { get; }
    public float OneTick { get; }
    public float MaxValue { get; }

    public float MinValue => OneTick;

    public bool Equals(Pair? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Symbol == other.Symbol;
    }

    public override bool Equals(object? other) => 
        Equals(other as Pair);

    public override int GetHashCode() => Symbol.GetHashCode();

    public bool IsRate(float value)
    {
        if (value != Round(value))
            return false;

        return value >= OneTick && value <= MaxValue;
    }

    public override string ToString() => Symbol.ToString();

    public string Format(float value) => value.ToString(format);

    public float Round(float value) => MathF.Round(value, Digits);

    public static bool operator ==(Pair lhs, Pair rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
                return true;

            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator !=(Pair lhs, Pair rhs) =>
        !(lhs == rhs);
}