// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.FxData.Models;

namespace SquidEyes.FxData.Helpers
{
    public class BrickArgs
    {
        internal BrickArgs(Brick brick, Tick tick)
        {
            Brick = brick;
            Tick = tick;
        }

        public Brick Brick { get; }
        public Tick Tick { get; }
    }
}