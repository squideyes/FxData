using Xunit;
using FluentAssertions;
using SquidEyes.FxData.Models;

namespace SquidEyes.UnitTests;

public class UnitsTests
{
    [Theory]
    [InlineData(1000)]
    [InlineData(1000000)]
    public void ConstructorWithGoodArg(int units) => _ = Units.From(units);

    ////////////////////////////

    [Fact]
    public void ConstructorWithoutArg()
    {
        var units = new Units();

        units.Should().Be(Units.From(Units.Minimum));
        units.Value.Should().Be(Units.Minimum);
        //units.IsEmpty.Should().Be(false);
    }

    ////////////////////////////

    //[Fact]
    //public void ToStringWithBadDigitsThrowsError()
    //{
    //    FluentActions.Invoking(() => new Units(Units.MIN_VALUE).ToString(4))
    //        .Should().Throw<ArgumentOutOfRangeException>();
    //}

    ////////////////////////////

    //[Fact]
    //public void IsRateWithBadDigitsThrowsError()
    //{
    //    FluentActions.Invoking(() => Units.IsRate(1.2345f, 4))
    //        .Should().Throw<ArgumentOutOfRangeException>();
    //}

    ////////////////////////////

    //[Fact]
    //public void SetToDefault()
    //{
    //    Units units = default;

    //    units.Value.Should().Be(0);
    //    units.IsEmpty.Should().Be(true);
    //}

    ////////////////////////////

    //[Theory]
    //[InlineData(0)]
    //[InlineData(1000000)]
    //public void IntConstructorWithBadArg(int value)
    //{
    //    FluentActions.Invoking(() => _ = new Units(value))
    //        .Should().Throw<ArgumentOutOfRangeException>();
    //}

    ////////////////////////////

    //[Theory]
    //[InlineData(0.0f, 5)]
    //[InlineData(10.0f, 5)]
    //[InlineData(0.00001f, 0)]
    //[InlineData(0.001f, 0)]
    //public void FloatConstructorWithBadArgs(float value, int digits)
    //{
    //    FluentActions.Invoking(() => _ = new Units(value, digits))
    //        .Should().Throw<ArgumentOutOfRangeException>();
    //}

    ////////////////////////////

    //[Theory]
    //[InlineData(0.0, 5, false)]
    //[InlineData(0.00001f, 5, true)]
    //[InlineData(9.99999f, 5, true)]
    //[InlineData(10.0, 5, false)]
    //[InlineData(0.0, 3, false)]
    //[InlineData(0.001f, 3, true)]
    //[InlineData(999.999f, 3, true)]
    //[InlineData(1000.0, 3, false)]
    //public void IsRateWithMixedArgs(float value, int digits, bool result) =>
    //    Units.IsRate(value, digits).Should().Be(result);

    ////////////////////////////

    //[Fact]
    //public void RateNotEqualToNullRate() =>
    //      new Units(1).Equals(null).Should().BeFalse();

    ////////////////////////////

    //[Fact]
    //public void GetHashCodeReturnsExpectedResult()
    //{
    //    new Units(1).GetHashCode().Should().Be(new Units(1).GetHashCode());

    //    new Units(1).GetHashCode().Should().NotBe(new Units(2).GetHashCode());
    //}

    ////////////////////////////

    //[Theory]
    //[InlineData(true)]
    //[InlineData(false)]
    //public void GenericEquals(bool result) => new Units(1)
    //    .Equals(result ? new Units(1) : new Units(2)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(true)]
    //[InlineData(false)]
    //public void ObjectEqualsWithGoodRates(bool result) => new Units(1)
    //    .Equals(result ? new Units(1) : new Units(2)).Should().Be(result);

    ////////////////////////////

    //[Fact]
    //public void ObjectEqualsWithNullRate() =>
    //    new Units(1).Equals(null).Should().BeFalse();

    ////////////////////////////

    //[Theory]
    //[InlineData(true)]
    //[InlineData(false)]
    //public void EqualsOperator(bool result) => (new Units(1)
    //    == (result ? new Units(1) : new Units(2))).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(true)]
    //[InlineData(false)]
    //public void NotEqualsOperator(bool result) => (new Units(1)
    //    != (result ? new Units(2) : new Units(1))).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(1, 2, -1)]
    //[InlineData(2, 2, 0)]
    //[InlineData(3, 2, 1)]
    //public void CompareToWithMixedArgs(int v1, int v2, int result) =>
    //    new Units(v1).CompareTo(new Units(v2)).Should().Be(result);

    ////////////////////////////

    //[Fact]
    //public void AddOperator() =>
    //    (new Units(1) + new Units(2)).Should().Be(new Units(3));

    ////////////////////////////

    //[Fact]
    //public void SubtractOperator() =>
    //    (new Units(3) - new Units(2)).Should().Be(new Units(1));

    ////////////////////////////

    //[Theory]
    //[InlineData(3, 3, false)]
    //[InlineData(2, 3, true)]
    //[InlineData(1, 3, true)]
    //public void LessThanOperator(int v1, int v2, bool result) =>
    //    (new Units(v1) < new Units(v2)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(1, 1, false)]
    //[InlineData(2, 1, true)]
    //[InlineData(3, 1, true)]
    //public void GreaterThanOperator(int v1, int v2, bool result) =>
    //    (new Units(v1) > new Units(v2)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(2, 4, true)]
    //[InlineData(2, 3, true)]
    //[InlineData(2, 2, true)]
    //[InlineData(2, 1, false)]
    //public void LessThanOrEqualToOperator(int v1, int v2, bool result) =>
    //    (new Units(v1) <= new Units(v2)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(4, 2, true)]
    //[InlineData(3, 2, true)]
    //[InlineData(2, 2, true)]
    //[InlineData(1, 2, false)]
    //public void GreaterThanOrEqualToOperator(int v1, int v2, bool result) =>
    //    (new Units(v1) >= new Units(v2)).Should().Be(result);

    ////////////////////////////

    //[Theory]
    //[InlineData(Units.MIN_VALUE)]
    //[InlineData(Units.MAX_VALUE)]
    //public void IntToRateToOperatorWithGoodArg(int value) =>
    //    ((Units)value).Value.Should().Be(value);

    ////////////////////////////

    //[Theory]
    //[InlineData(Units.MAX_VALUE + 1)]
    //[InlineData(Units.MIN_VALUE - 1)]
    //public void IntToRateToOperatorWithBadArg(int value)
    //{
    //    FluentActions.Invoking(() => _ = (Units)value)
    //        .Should().Throw<ArgumentOutOfRangeException>();
    //}
}
