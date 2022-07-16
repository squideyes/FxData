// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.FxData.Models;

namespace SquidEyes.FxData.DukasFetch;

public class Settings
{
    public string? ConnString { get; set; }
    public List<Symbol>? Symbols { get; set; }
    public int MinYear { get; set; }
    public bool Replace { get; set; } = true;
}