// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.FxData.Context;

public static class SideExtenders
{
    public static bool IsBuy(this Side value) => value == Side.Buy;

    public static bool IsSell(this Side value) => value == Side.Sell;

    public static Side Flipped(this Side value) =>
        value.IsBuy() ? Side.Sell : Side.Buy;
}