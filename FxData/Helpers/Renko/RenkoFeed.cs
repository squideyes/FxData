using SquidEyes.Basics;
using SquidEyes.FxData.Context;
using SquidEyes.FxData.Models;
using System.Collections;
using System.Text;

namespace SquidEyes.FxData.Helpers
{
    public class RenkoFeed : IEnumerable<Brick>
    {
        private readonly SlidingBuffer<Brick> bricks;

        private readonly Session session;
        private readonly Rate ticksPerBrick;
        private readonly bool raiseOpenBricks;

        private DataPoint firstDataPoint = null!;

        public event EventHandler<BrickArgs>? OnBrick;

        public RenkoFeed(Session session,
            Rate ticksPerBrick, bool raiseOpenBricks, int bufferSize = 100)
        {
            this.session = session ??
                throw new ArgumentNullException(nameof(session));

            this.ticksPerBrick = ticksPerBrick
                .Validated(nameof(ticksPerBrick), v => v.IsTicksPerBrick());

            this.raiseOpenBricks = raiseOpenBricks;

            bricks = new SlidingBuffer<Brick>(bufferSize, true);
        }

        public int Count => bricks.Count;

        public Brick this[int index] => bricks[index];

        public void HandleTick(Tick tick)
        {
            if (tick == default)
                throw new ArgumentNullException(nameof(tick));

            if (tick.TickOn.TradeDate != session.TradeDate)
                throw new ArgumentOutOfRangeException(nameof(tick));

            var dataPoint = new DataPoint(tick.TickOn, tick.Mid);

            Rate GetRate(Rate source, out Rate target) => target = source;

            void AddClosedBricks()
            {
                if (bricks.Count == 0)
                    return;

                var last = bricks[0];

                while (tick.Mid > GetRate(last.High.Rate + ticksPerBrick, out Rate rate))
                {
                    last = AddAndRaiseClosedBrick(new Brick(last.High,
                        new DataPoint(tick.TickOn, rate)), tick);
                }

                while (tick.Mid < GetRate(last.Low.Rate - ticksPerBrick, out Rate rate))
                {
                    last = AddAndRaiseClosedBrick(new Brick(last.Low,
                        new DataPoint(tick.TickOn, rate)), tick);
                }
            }

            if (firstDataPoint == default)
            {
                firstDataPoint = dataPoint;
            }
            else if (Count == 0)
            {
                if (tick.Mid > GetRate(firstDataPoint.Rate + ticksPerBrick, out Rate rate))
                {
                    AddAndRaiseClosedBrick(new Brick(firstDataPoint,
                        new DataPoint(tick.TickOn, rate)), tick);
                }
                else if (tick.Mid < GetRate(firstDataPoint.Rate - ticksPerBrick, out rate))
                {
                    AddAndRaiseClosedBrick(new Brick(
                        firstDataPoint, new DataPoint(tick.TickOn, rate)), tick);
                }

                AddClosedBricks();
            }
            else
            {
                AddClosedBricks();
            }

            RaiseOpenBrick(dataPoint, tick);
        }

        public string GetPattern(bool inSession)
        {
            var sb = new StringBuilder();

            for (int i = Count - 1; i >= 1; i--)
            {
                if (inSession && !session.InSession(this[i].Open.TickOn))
                    continue;

                sb.Append(this[i].Trend == Trend.Up ? 'U' : 'D');
            }

            return sb.ToString();
        }

        private void RaiseOpenBrick(DataPoint dataPoint, Tick tick)
        {
            if (!raiseOpenBricks)
                return;

            DataPoint open;

            if (bricks.Count == 0)
                open = firstDataPoint;
            else
                open = bricks[0].Close;

            var brick = new Brick(open, dataPoint);

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