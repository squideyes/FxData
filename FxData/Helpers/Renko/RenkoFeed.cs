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
using System.Collections;

namespace SquidEyes.FxData.Helpers;

public class RenkoFeed : IEnumerable<Brick>
{
    private readonly SlidingBuffer<Brick> bricks;
    private TickOn? openOn = null;
    private Rate lastRate = default;

    private readonly Session session;
    private readonly Rate ticksPerBrick;
    private readonly Rate halfBrick;
    private readonly Rate oneAndOneHalfBricks;

    public event EventHandler<BrickArgs>? OnBrick;

    public RenkoFeed(Session session, Rate ticksPerBrick, int bufferSize = 100)
    {
        this.session = session ??
            throw new ArgumentNullException(nameof(session));

        this.ticksPerBrick = ticksPerBrick
            .Validated(nameof(ticksPerBrick), v => v.IsTicksPerBrick());

        halfBrick = ticksPerBrick.Value / 2;

        oneAndOneHalfBricks = (int)(ticksPerBrick.Value * 1.5);

        bricks = new SlidingBuffer<Brick>(bufferSize, true);
    }

    public int Count => bricks.Count;

    public Brick this[int index] => bricks[index];

    public void HandleTick(Tick tick)
    {
        if (tick == default)
            throw new ArgumentOutOfRangeException(nameof(tick));

        if (!session.InSession(tick.TickOn))
            throw new ArgumentOutOfRangeException(nameof(tick));

        if (lastRate == default)
        {
            openOn = tick.TickOn;

            lastRate = tick.Mid - (tick.Mid % ticksPerBrick) + halfBrick;
        }

        while (tick.Mid >= lastRate + oneAndOneHalfBricks)
        {
            lastRate += ticksPerBrick;

            var brick = new Brick(
                new DataPoint(openOn!.Value, lastRate - halfBrick),
                new DataPoint(tick.TickOn, lastRate + halfBrick));

            AddAndRaiseBrick(brick, tick);

            openOn = tick.TickOn;
        }

        while (tick.Mid <= lastRate - oneAndOneHalfBricks)
        {
            lastRate -= ticksPerBrick;

            var brick = new Brick(
                new DataPoint(openOn!.Value, lastRate + halfBrick),
                new DataPoint(tick.TickOn, lastRate - halfBrick));

            AddAndRaiseBrick(brick, tick);

            openOn = tick.TickOn;
        }
    }

    public string GetPattern(int maxTrends = int.MaxValue)
    {
        if (maxTrends < 1)
            throw new ArgumentOutOfRangeException(nameof(maxTrends));

        var sb = new StringBuilder();

        if (maxTrends > Count)
            maxTrends = Count;

        for (int i = maxTrends - 1; i >= 0; i--)
            sb.Append(this[i].Trend == Trend.Up ? 'U' : 'D');

        return sb.ToString();
    }

    private void AddAndRaiseBrick(Brick brick, Tick tick)
    {
        bricks.Add(brick);

        OnBrick?.Invoke(this, new BrickArgs(brick, tick));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<Brick> GetEnumerator() => bricks.GetEnumerator();
}