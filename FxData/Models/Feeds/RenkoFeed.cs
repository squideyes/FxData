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

namespace SquidEyes.Trading.FxData;

public class RenkoFeed
{
    private readonly Rate brickTicks;
    private readonly BidOrAsk bidOrAsk;

    private bool firstTick = true;
    private Brick lastBrick = null!;

    private TickOn openOn;
    private Rate open;
    private TickOn closeOn;
    private Rate close;

    public event EventHandler<BrickArgs>? OnBrick;

    public RenkoFeed(Session session, Rate brickTicks, BidOrAsk bidOrAsk)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));

        if (brickTicks < 5)
            throw new ArgumentOutOfRangeException(nameof(brickTicks));

        if (!bidOrAsk.IsEnumValue())
            throw new ArgumentOutOfRangeException(nameof(bidOrAsk));

        this.brickTicks = brickTicks;
        this.bidOrAsk = bidOrAsk;
    }

    public Session Session { get; }

    private void BrickClosed(Tick tick, TickOn closeOn, Rate limit, Brick brick)
    {
        OnBrick?.Invoke(this, new BrickArgs(tick, brick));

        openOn = closeOn;
        open = limit;
        lastBrick = brick;
    }

    private void Rising(Tick tick)
    {
        Rate limit;

        var firstTime = true;

        while (close > (limit = open + brickTicks))
        {
            if (firstTime)
                firstTime = false;

            BrickClosed(tick, closeOn, limit, 
                new Brick(openOn, open, closeOn, limit));
        }
    }

    private void Falling(Tick tick)
    {
        Rate limit;

        var firstTime = true;

        while (close < (limit = open - brickTicks))
        {
            if (firstTime)
                firstTime = false;

            BrickClosed(tick, closeOn, limit, 
                new Brick(openOn, open, closeOn, limit));
        }
    }

    public void HandleTick(Tick tick)
    {
        if (!Session.InSession(tick.TickOn))
        {
            throw new InvalidOperationException(
                $"{tick.TickOn} is not within the \"{Session}\" session");
        }

        var rate = tick.ToRate(bidOrAsk);

        if (firstTick)
        {
            firstTick = false;

            openOn = closeOn = tick.TickOn;
            open = close = rate;
        }
        else
        {
            closeOn = tick.TickOn;
            close = rate;

            if (close > open)
            {
                if (lastBrick == null! || (lastBrick.Trend == Trend.Rising))
                {
                    Rising(tick);
                }
                else
                {
                    var limit = lastBrick!.Open + brickTicks;

                    if (close > limit)
                    {
                        BrickClosed(tick, closeOn, limit,
                            new Brick(openOn, lastBrick.Open, closeOn, limit));

                        Rising(tick);
                    }
                }
            }
            else if (close < open)
            {
                if (lastBrick == null! || (lastBrick.Trend == Trend.Falling))
                {
                    Falling(tick);
                }
                else
                {
                    var limit = lastBrick!.Open - brickTicks;

                    if (close < limit)
                    {
                        BrickClosed(tick, closeOn, limit,
                            new Brick(openOn, lastBrick.Open, closeOn, limit));

                        Falling(tick);
                    }
                }
            }
        }
    }
}