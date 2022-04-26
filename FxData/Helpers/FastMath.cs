// ********************************************************
// Copyright (C) 2021 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

namespace SquidEyes.FxData.Shared.Helpers;

public static class FastMath
{
    private static readonly float[] adjustments = GetAdjustments();

    public static float Round(float value) =>
        MathF.Floor(value + 0.5f);

    public static float Round(float value, int digits)
    {
        var adjustment = adjustments[digits];

        return MathF.Floor(value * adjustment + 0.5f) / adjustments[digits];
    }

    private static float[] GetAdjustments()
    {
        var result = new float[15];

        for (var i = 0; i < result.Length; i++)
            result[i] = MathF.Pow(10, i);

        return result;
    }
}