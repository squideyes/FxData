// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.FxData.Models;

public class Brick
{
    internal Brick(DataPoint open, DataPoint close)
    {
        Open = open;
        Close = close;
    }

    public DataPoint Open { get; }
    public DataPoint Close { get; }

    public Trend Trend => (Open.Rate < Close.Rate) ? Trend.Up : Trend.Down;

    public override string ToString() =>
        $"{Open.Rate} to {Close.Rate} ({Open.TickOn} to {Close.TickOn})";

    public string ToCsvString() => $"{Open.ToCsvString()},{Close.ToCsvString()}";
}