// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.FxData.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SquidEyes.FxData.Helpers;

public class JsonStringRateConverter : JsonConverter<Rate>
{
    private readonly Pair pair;

    public JsonStringRateConverter(Pair pair)
    {
        this.pair = pair ?? throw new ArgumentNullException(nameof(pair));
    }

    public override Rate Read(ref Utf8JsonReader reader,
        Type _, JsonSerializerOptions options)
    {
        return Rate.Parse(reader.GetString()!, pair.Digits);
    }

    public override void Write(Utf8JsonWriter writer,
        Rate value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.AsDouble().ToString());
    }
}