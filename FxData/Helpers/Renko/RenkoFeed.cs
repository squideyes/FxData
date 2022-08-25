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
using System.Collections;
using System.Text;

namespace SquidEyes.FxData.Helpers
{
    public class RenkoFeed : IEnumerable<Brick>
    {
        private readonly SlidingBuffer<Brick> bricks;

        private readonly Session session;
        private readonly BidOrAsk bidOrAsk;
        private readonly Rate1 ticksPerBrick;
        private readonly bool raiseOpenBricks;

        private Point firstPoint = null!;

        public event EventHandler<BrickArgs>? OnBrick;

        public RenkoFeed(Session session, BidOrAsk bidOrAsk,
            Rate1 ticksPerBrick, bool raiseOpenBricks = false, int bufferSize = 100)
        {
            this.session = session ??
                throw new ArgumentNullException(nameof(session));

            this.bidOrAsk = bidOrAsk.Validated(nameof(bidOrAsk), v => v.IsEnumValue());

            this.ticksPerBrick = ticksPerBrick
                .Validated(nameof(ticksPerBrick), v => v.IsTicksPerBrick());

            this.raiseOpenBricks = raiseOpenBricks;

            bricks = new SlidingBuffer<Brick>(bufferSize, true);
        }

        public int Count => bricks.Count;

        public Brick this[int index] => bricks[index];

        public void HandleTick(Tick tick)
        {
            if (tick.IsDefaultValue())
                throw new ArgumentNullException(nameof(tick));

            if (!session.InSession(tick.TickOn.AsDateTime()))
                throw new ArgumentOutOfRangeException(nameof(tick));

            var rate = bidOrAsk == BidOrAsk.Bid ? tick.Bid : tick.Ask;

            var point = new Point(tick.TickOn, rate);

            Rate1 GetRate(Rate1 source, out Rate1 target) => target = source;

            void AddClosedBricks()
            {
                if (bricks.Count == 0)
                    return;

                var last = bricks[0];

                while (rate > GetRate(last.High.Rate + ticksPerBrick, out Rate1 closeAt))
                {
                    last = AddAndRaiseClosedBrick(
                        new Brick(last.High, new Point(tick.TickOn, closeAt)), tick);
                }

                while (rate < GetRate(last.Low.Rate - ticksPerBrick, out Rate1 closeAt))
                {
                    last = AddAndRaiseClosedBrick(
                        new Brick(last.Low, new Point(tick.TickOn, closeAt)), tick);
                }
            }

            if (firstPoint.IsDefaultValue())
            {
                firstPoint = point;
            }
            else if (Count == 0)
            {
                if (rate > GetRate(firstPoint.Rate + ticksPerBrick, out Rate1 closeAt))
                {
                    AddAndRaiseClosedBrick(
                        new Brick(firstPoint, new Point(tick.TickOn, closeAt)), tick);
                }
                else if (rate < GetRate(firstPoint.Rate - ticksPerBrick, out closeAt))
                {
                    AddAndRaiseClosedBrick(
                        new Brick(firstPoint, new Point(tick.TickOn, closeAt)), tick);
                }

                AddClosedBricks();
            }
            else
            {
                AddClosedBricks();
            }

            RaiseOpenBrick(point, tick);
        }

        public string GetPattern(bool inSession)
        {
            var sb = new StringBuilder();

            for (int i = Count - 1; i >= 1; i--)
            {
                if (inSession && !session.InSession(this[i].Open.TickOn.AsDateTime()))
                    continue;

                sb.Append(this[i].Trend == Trend.Up ? 'U' : 'D');
            }

            return sb.ToString();
        }

        private void RaiseOpenBrick(Point point, Tick tick)
        {
            if (!raiseOpenBricks)
                return;

            Point open;

            if (bricks.Count == 0)
                open = firstPoint;
            else
                open = bricks[0].Close;

            var brick = new Brick(open, point);

            OnBrick?.Invoke(this, new BrickArgs(brick, tick, false));
        }

        private Brick AddAndRaiseClosedBrick(Brick brick, Tick tick)
        {
            bricks.Add(brick);

            OnBrick?.Invoke(this, new BrickArgs(brick, tick, true));

            return brick;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<Brick> GetEnumerator() => bricks.GetEnumerator();
    }
}