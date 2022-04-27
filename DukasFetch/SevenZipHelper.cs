// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SevenZip;
using SevenZip.Compression.LZMA;

namespace SquidEyes.FxData.DukasFetch;

public static class SevenZipHelper
{
    private static readonly int dictionary = 1 << 23;

    private static readonly bool eos = false;

    private static readonly CoderPropID[] propIds =
    {
        CoderPropID.DictionarySize,
        CoderPropID.PosStateBits,
        CoderPropID.LitContextBits,
        CoderPropID.LitPosBits,
        CoderPropID.Algorithm,
        CoderPropID.NumFastBytes,
        CoderPropID.MatchFinder,
        CoderPropID.EndMarker
    };

    private static readonly object[] properties =
    {
        dictionary,
        2,
        3,
        0,
        2,
        128,
        "bt4",
        eos
    };

    public static byte[] Compress(byte[] inputBytes)
    {
        var inStream = new MemoryStream(inputBytes);

        var outStream = new MemoryStream();

        var encoder = new Encoder();

        encoder.SetCoderProperties(propIds, properties);
        
        encoder.WriteCoderProperties(outStream);
        
        long fileSize = inStream.Length;
        
        for (int i = 0; i < 8; i++)
            outStream.WriteByte((byte)(fileSize >> (8 * i)));
        
        encoder.Code(inStream, outStream, -1, -1, null);
        
        return outStream.ToArray();
    }

    public static byte[] Decompress(byte[] inputBytes)
    {
        var newInStream = new MemoryStream(inputBytes);

        var decoder = new Decoder();

        newInStream.Seek(0, 0);

        var newOutStream = new MemoryStream();

        var properties = new byte[5];

        if (newInStream.Read(properties, 0, 5) != 5)
            throw new Exception("Input .lzma is too short");
        
        long outSize = 0;
        
        for (int i = 0; i < 8; i++)
        {
            int v = newInStream.ReadByte();
        
            if (v < 0)
                throw (new Exception("Can't Read 1"));
            
            outSize |= ((long)(byte)v) << (8 * i);
        }
        
        decoder.SetDecoderProperties(properties);

        var compressedSize = newInStream.Length - newInStream.Position;
        
        decoder.Code(newInStream, newOutStream, compressedSize, outSize, null);

        return newOutStream.ToArray();
    }
}