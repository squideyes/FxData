//// ********************************************************
//// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
////
//// This file is part of SquidEyes.FxData
////
//// The use of this source code is licensed under the terms
//// of the MIT License (https://opensource.org/licenses/MIT)
//// ********************************************************

//using SquidEyes.FxData.Models;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace SquidEyes.FxData.Helpers;

//public class JsonStringUnitsConverter : JsonConverter<Units>
//{
//    public override Units Read(ref Utf8JsonReader reader,
//        Type _, JsonSerializerOptions options)
//    {
//        return Units.From(int.Parse(reader.GetString()!));
//    }

//    public override void Write(Utf8JsonWriter writer,
//        Units value, JsonSerializerOptions options)
//    {
//        writer.WriteStringValue(value.ToString());
//    }
//}