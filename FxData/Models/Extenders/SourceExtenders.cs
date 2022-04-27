// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.FxData.Models;

public static class SourceExtenders
{
    public static string ToCode(this Source value)
    {
        return value switch
        {
            Source.SquidEyes => "SE",
            Source.Dukascopy => "DC",
            Source.HistData => "HD",
            Source.ForexCom => "FC",
            Source.OandaCorp => "OC",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }

    public static Source ToSource(this string value)
    {
        return value switch
        {
            "SE" => Source.SquidEyes,
            "DC" => Source.Dukascopy,
            "HD" => Source.HistData,
            "FC" => Source.ForexCom,
            "OC" => Source.OandaCorp,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }
}