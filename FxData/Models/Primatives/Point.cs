// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.FxData.Context;

namespace SquidEyes.FxData.Models;

public class Point
{
    internal Point(TickOn tickOn, Rate rate)
    {
        TickOn = tickOn;
        Rate = rate;
    }

    public TickOn TickOn { get; }
    public Rate Rate { get; }

    public string ToCsvString() => $"{TickOn},{Rate}";
}