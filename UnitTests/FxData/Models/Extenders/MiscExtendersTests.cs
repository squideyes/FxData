using System;
using Xunit;
using FluentAssertions;
using SquidEyes.FxData.Context;
using SquidEyes.FxData.Models;

namespace SquidEyes.UnitTests.FxData;

public class MiscExtendersTests
{
    [Fact]
    public void ToRateWithBadMidOrAskThrowsException()
    {
        var session = new Session(Known.MinTradeDate, Market.Combined);

        var tick = new Tick(session.MinTickOn, 1, 2);

        FluentActions.Invoking(() => tick.ToRate(0))
            .Should().Throw<ArgumentOutOfRangeException>();
    }
}