using SquidEyes.FxData.Models;
using System;
using System.Collections.Generic;

namespace SquidEyes.UnitTests.Testing;

public class TickSetFixture : IDisposable
{
    public TickSetFixture()
    {
        void AddTickSet(int day)
        {
            TickSets.Add(day, TestHelper.GetTickSet(day));
        }

        TickSets = new Dictionary<int, TickSet>();

        //for (var day = 4; day <= 8; day++)
        //    AddTickSet(day);
    }

    public Dictionary<int, TickSet> TickSets { get; private set; }

    public void Dispose()
    {
        TickSets.Clear();

        TickSets = null!;

        GC.SuppressFinalize(this);
    }
}
