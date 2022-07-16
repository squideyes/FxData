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
    internal Brick(Point open, Point close)
    {
        Open = open;
        Close = close;
    }

    public Point Open { get; }

    public Point Close { get; }

    public Point High => Trend == Trend.Up ? Close : Open;

    public Point Low => Trend == Trend.Up ? Open : Close;

    public Rate TicksPerBrick =>
        Rate.From(Math.Abs((int)Open.Rate - (int)Close.Rate));

    public Trend Trend => 
        (Open.Rate < Close.Rate) ? Trend.Up : Trend.Down;

    public string ToCsvString() => 
        $"{Open.ToCsvString()},{Close.ToCsvString()}";

    public override string ToString() =>
        $"{Open.Rate} to {Close.Rate} ({Open.TickOn} to {Close.TickOn})";
}