using System.Reflection.Metadata;
using SquidEyes.FxData.Context;
using SquidEyes.FxData.Models;
using Xunit;
using FluentAssertions;
using SquidEyes.Basics;

namespace SquidEyes.UnitTests.FxData.Models;

public class BrickTests
{
    [Fact]
    public void ToStringReturnsExpectedValue()
    {
        GetBrick().ToString().Should().Be(
            "1 on 01/04/2016 03:00:00.000 to 2 on 01/04/2016 16:59:59.999");
    }

    [Fact]
    public void ToCsvStringReturnsExpectedValue()
    {
        GetBrick().ToCsvString(Known.Pairs[Symbol.EURUSD]).Should().Be(
            "01/04/2016 03:00:00.000,0.00001,01/04/2016 16:59:59.999,0.00002");
    }

    [Theory]
    [InlineData(1, 2, Trend.Rising)]
    [InlineData(2, 1, Trend.Falling)]
    public void TrendReturnsExpectedValue(int open, int close, Trend expected) =>
        GetBrick(open, close).Trend.Should().Be(expected);

    private static Brick GetBrick(int bid = 1, int ask = 2)
    {
        return new Session(new TradeDate(2016, 1, 4), Market.Combined)
            .AsFunc(s => new Brick(s.MinTickOn, bid, s.MaxTickOn, ask));
    }
}