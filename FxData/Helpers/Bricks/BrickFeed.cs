// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using SquidEyes.Basics;
using SquidEyes.FxData.Context;
using SquidEyes.FxData.Models;

namespace SquidEyes.FxData.Helpers;

public class BrickFeed
{
    private class Data
    {
        public Data(TickOn openOn, Rate open, TickOn closeOn, Rate close)
        {
            OpenOn = openOn;
            Open = open;
            CloseOn = closeOn;
            Close = close;
        }

        public TickOn OpenOn { get; }
        public Rate Open { get; }
        public TickOn CloseOn { get; }
        public Rate Close { get; }
    }

    private readonly Rate ticksPerBrick;

    private TickOn? openOn = null;
    private Rate lastRate = default;
    private bool lastBrickWasClosed = false;

    public event EventHandler<BrickArgs>? OnBrick;

    public BrickFeed(Rate ticksPerBrick)
    {
        this.ticksPerBrick = ticksPerBrick.Value
            .Validated(nameof(ticksPerBrick), v => v.Between(5, 99));
    }

    public void HandleTick(Tick tick)
    {
        if (lastRate == default)
        {
            openOn = tick.TickOn;
            lastRate = tick.Mid;

            OnBrick?.Invoke(this, new BrickArgs(new Brick(tick), tick, true));

            return;
        }

        Data GetData(Rate closeRate) =>
            new(openOn!.Value, lastRate, tick.TickOn, closeRate);

        var datas = new List<Data>();

        while (tick.Mid > lastRate + ticksPerBrick)
        {
            datas.Add(GetData(lastRate + ticksPerBrick));

            lastRate += ticksPerBrick;

            openOn = tick.TickOn;
        }

        while (tick.Mid < lastRate - ticksPerBrick)
        {
            datas.Add(GetData(lastRate - ticksPerBrick));

            lastRate -= ticksPerBrick;

            openOn = tick.TickOn;
        }

        void RaiseBrick(Data data, bool isFirstTickOfBar, bool isClosed, bool isVirtual)
        {
            if (isClosed)
                lastBrickWasClosed = true;

            var brick = new Brick(data.OpenOn, data.Open, data.CloseOn, data.Close, true, isVirtual);

            OnBrick?.Invoke(this, new BrickArgs(brick, tick, isFirstTickOfBar));
        }

        for (int i = 0; i < datas.Count; i++)
            RaiseBrick(datas[i], false, true, i < datas.Count - 1);

        if (tick.Mid < lastRate || tick.Mid > lastRate || datas.Count == 0)
        {
            var brick = new Brick(openOn!.Value, lastRate, tick.TickOn, tick.Mid, false, false);

            OnBrick?.Invoke(this, new BrickArgs(brick, tick, lastBrickWasClosed));

            lastBrickWasClosed = false;
        }
    }
}