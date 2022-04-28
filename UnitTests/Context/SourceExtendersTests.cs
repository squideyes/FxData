using System;
using FluentAssertions;
using SquidEyes.FxData.Models;
using Xunit;

namespace SquidEyes.UnitTests.Context;

public class SourceExtendersTests
{
    [Theory]
    [InlineData(Source.Dukascopy, "DC")]
    [InlineData(Source.ForexCom, "FC")]
    [InlineData(Source.HistData, "HD")]
    [InlineData(Source.OandaCorp, "OC")]
    [InlineData(Source.SquidEyes, "SE")]
    public void ToCodeReturnsExpectedValue(Source source, string expected) =>
        source.ToCode().Should().Be(expected);
    
    [Theory]
    [InlineData("DC", Source.Dukascopy)]
    [InlineData("FC", Source.ForexCom)]
    [InlineData("HD", Source.HistData)]
    [InlineData("OC", Source.OandaCorp)]
    [InlineData("SE", Source.SquidEyes)]
    public void ToSourceReturnsExpectedValue(string source, Source expected) =>
        source.ToSource().Should().Be(expected);

    [Fact]
    public void ToSourceThrowsErrorOnBadArg()
    {
        FluentActions.Invoking(() => "XX".ToSource())
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToCodeThrowsErrorOnBadArg()
    {
        FluentActions.Invoking(() => ((Source)0).ToCode())
            .Should().Throw<ArgumentOutOfRangeException>();
    }    
}