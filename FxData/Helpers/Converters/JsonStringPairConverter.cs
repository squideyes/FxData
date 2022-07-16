// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.FxData.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SquidEyes.FxData.Helpers;

public class JsonStringPairConverter : JsonConverter<Pair>
{
    public override Pair Read(ref Utf8JsonReader reader,
        Type _, JsonSerializerOptions options)
    {
        return Known.Pairs[reader.GetString()!.ToEnumValue<Symbol>()];
    }

    public override void Write(Utf8JsonWriter writer,
        Pair value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}