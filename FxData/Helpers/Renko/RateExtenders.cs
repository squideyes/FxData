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

namespace SquidEyes.FxData.Helpers;

public static class RateExtenders
{
    public static bool IsTicksPerBrick(this Rate value) => 
        value.Value.Between(1, 200);
}