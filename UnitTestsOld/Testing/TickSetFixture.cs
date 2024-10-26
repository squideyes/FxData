using SquidEyes.FxData.Models;
using System;
using System.Collections.Generic;

namespace SquidEyes.UnitTests.Testing;

public class TickSetFixture : IDisposable
{
    public TickSetFixture()
    {
        void AddTickSet(int day, DataKind dataKind)
        {
            TickSets.Add((day, dataKind), 
                TestHelper.GetTickSet(day, dataKind));
        }

        TickSets = new Dictionary<(int, DataKind), TickSet>();

        for (var day = 4; day <= 8; day++)
        {
            AddTickSet(day, DataKind.CSV);
            AddTickSet(day, DataKind.STS);
        }
    }

    public Dictionary<(int, DataKind), TickSet> TickSets { get; private set; }

    public void Dispose()
    {
        TickSets.Clear();

        TickSets = null!;
    }
}
