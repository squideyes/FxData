// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

//using SquidEyes.FxData.Context;
//using System.Text;
//using SquidEyes.Basics;
//using static SquidEyes.FxData.Models.Trend;

//namespace SquidEyes.FxData.Models;

//public class Brick
//{
//    internal Brick(TickOn openOn, Rate open, TickOn closeOn, Rate close)
//    {
//        OpenOn = openOn;
//        Open = open;
//        CloseOn = closeOn;
//        Close = close;
//    }

//    public TickOn OpenOn { get; }
//    public Rate Open { get; }
//    public TickOn CloseOn { get; set; }
//    public Rate Close { get; set; }

//    public override string ToString() =>
//        $"{Open} on {OpenOn} to {Close} on {CloseOn}";

//    public string ToCsvString(Pair pair)
//    {
//        ArgumentNullException.ThrowIfNull(pair);

//        var sb = new StringBuilder();

//        sb.Append(OpenOn);
//        sb.AppendDelimited(Open.ToString(pair.Digits));
//        sb.AppendDelimited(CloseOn);
//        sb.AppendDelimited(Close.ToString(pair.Digits));

//        return sb.ToString();
//    }

//    public Trend Trend => Open < Close ? Rising : Falling;
//}