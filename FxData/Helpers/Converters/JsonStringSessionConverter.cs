// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Fundamentals;
using SquidEyes.FxData.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SquidEyes.FxData.Helpers;

public class JsonStringSessionConverter : JsonConverter<Session>
{
    public override Session Read(ref Utf8JsonReader reader,
        Type _, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;

        var tradeDate = new TradeDate(DateOnly.Parse(value[..10]));
        var market = value[12..^1].ToEnumValue<Market>();

        return Session.From(tradeDate, market);
    }

    public override void Write(Utf8JsonWriter writer,
        Session value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"{value.TradeDate} ({value.Market})");
    }
}