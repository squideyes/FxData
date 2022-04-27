// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.FxData.Models;

public static class MiscExtenders
{
    public static Rate ToRate(this Tick tick, BidOrAsk midOrAsk)
    {
        return midOrAsk switch
        {
            BidOrAsk.Bid => tick.Bid,
            BidOrAsk.Ask => tick.Ask,
            _=> throw new ArgumentOutOfRangeException(nameof(midOrAsk))
        };
    }
}