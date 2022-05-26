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
using System.Text;

namespace SquidEyes.FxData.Helpers;

public class BrickFeed
{
    private readonly SlidingBuffer<Brick> bricks;
    private readonly Rate ticksPerBrick;

    private TickOn? openOn = null;
    private Rate? lastRate = null;

    public event EventHandler<BrickArgs>? OnBrick;

    public BrickFeed(Rate ticksPerBrick, int bufferSize = 10)
    {
        this.ticksPerBrick = ticksPerBrick.Validated(
            nameof(ticksPerBrick), v => v.IsTicksPerBrick());

        bricks = new SlidingBuffer<Brick>(bufferSize, true);
    }

    public int Count => bricks.Count;

    public Brick this[int index] => bricks[index];

    public string GetPattern()
    {
        var sb = new StringBuilder();

        for (int i = Count - 1; i >= 1; i--)
            sb.Append(this[i].Trend == Trend.Up ? 'U' : 'D');

        return sb.ToString();
    }

    public void HandleTick(Tick tick)
    {
        void AddBrick(Rate closeRate)
        {
            var brick = new Brick(new DataPoint(openOn!.Value, lastRate.Value),
                new DataPoint(tick.TickOn, closeRate));

            bricks.Add(brick);

            OnBrick?.Invoke(this, new BrickArgs(brick, tick));
        }

        if (!lastRate.HasValue)
        {
            openOn = tick.TickOn;
            lastRate = tick.Mid;
        }
        else
        {
            Rate rate;

            while (tick.Mid > (rate = lastRate.Value + ticksPerBrick))
            {
                AddBrick(rate);

                lastRate = rate;
                openOn = tick.TickOn;
            }

            while (tick.Mid < (rate = lastRate.Value - ticksPerBrick))
            {
                AddBrick(rate);

                lastRate = rate;
                openOn = tick.TickOn;
            }
        }
    }
}