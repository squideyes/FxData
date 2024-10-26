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

//public class JsonStringTickOnConverter : JsonConverter<TickOn>
//{
//    private readonly Session session;

//    public JsonStringTickOnConverter(Session session)
//    {
//        this.session = session ??
//            throw new ArgumentNullException(nameof(session));
//    }

//    public override TickOn Read(ref Utf8JsonReader reader,
//        Type _, JsonSerializerOptions options)
//    {
//        return TickOn.Parse(reader.GetString()!, session);
//    }

//    public override void Write(Utf8JsonWriter writer,
//        TickOn value, JsonSerializerOptions options)
//    {
//        writer.WriteStringValue(value.ToString());
//    }
//}