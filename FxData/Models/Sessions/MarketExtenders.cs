//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//namespace SquidEyes.FxData.Models;

//public static class MarketExtenders
//{
//    public static string ToCode(this Market value)
//    {
//        return value switch
//        {
//            Market.NewYork => "NYC",
//            Market.London => "LDN",
//            Market.Combined => "CMB",
//            _ => throw new ArgumentOutOfRangeException(nameof(value))
//        };
//    }

//    public static Market ToMarket(this string value)
//    {
//        return value switch
//        {
//            "NYC" => Market.NewYork,
//            "LDN" => Market.London,
//            "CMB" => Market.Combined,
//            _ => throw new ArgumentOutOfRangeException(nameof(value))
//        };
//    }
//}