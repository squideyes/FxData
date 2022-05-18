// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.FxData.Context;
using SquidEyes.FxData.Models;

namespace SquidEyes.FxData.Models;

public class Brick
{
    internal Brick(Tick tick)
    {
        OpenOn = tick.TickOn;
        Open = tick.Mid;
        CloseOn = tick.TickOn;
        Close = tick.Mid;
        IsClosed = false;
        IsVirtual = false;
    }

    internal Brick(TickOn openOn, Rate open, TickOn closeOn, Rate close, bool isClosed, bool isVirtual)
    {
        OpenOn = openOn;
        Open = open;
        CloseOn = closeOn;
        Close = close;
        IsClosed = isClosed;
        IsVirtual = isVirtual;
    }

    public TickOn OpenOn { get; }
    public Rate Open { get; }
    public TickOn CloseOn { get; }
    public Rate Close { get; }
    public bool IsClosed { get; }
    public bool IsVirtual { get; }

    public Trend Trend
    {
        get
        {
            if (Open < Close)
                return Trend.Up;
            else if (Open > Close)
                return Trend.Down;
            else
                return Trend.None;
        }
    }

    public string ToCsvString(Pair pair = null!)
    {
        string Formatted(Rate value) => pair.Format(value.AsFloat(pair.Digits));

        if (pair == null!)
            return $"{OpenOn},{Open},{CloseOn},{Close}";
        else
            return $"{OpenOn},{Formatted(Open)},{CloseOn},{Formatted(Close)}";
    }

    public override string ToString() =>
        $"{OpenOn},{Open},{CloseOn},{Close},{(IsClosed ? "CLOSED" : "OPEN")},{(IsVirtual ? "VIRTUAL" : "REAL")}";
}