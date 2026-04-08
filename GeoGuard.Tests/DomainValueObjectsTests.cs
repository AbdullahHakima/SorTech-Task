using FluentAssertions;
using GeoGuard.Domain.Exceptions;
using GeoGuard.Domain.ValueObjects;

namespace GeoGuard.Tests;

public class DomainValueObjectsTests
{
    [Theory]
    [InlineData("us", "US")]
    [InlineData("UsA", "USA")]
    public void CountryCode_ShouldNormalizeToUppercase(string input, string expected)
    {
        var code = new CountryCode(input);

        code.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1A")]
    [InlineData("ABCD")]
    public void CountryCode_ShouldThrowForInvalidInput(string input)
    {
        var action = () => new CountryCode(input);

        action.Should().Throw<GeoGuardDomainException>();
    }

    [Fact]
    public void UserAgent_ShouldFallbackToUnknown_WhenInputIsEmpty()
    {
        var userAgent = new UserAgent(string.Empty);

        userAgent.Value.Should().Be("UNKNOWN");
    }

    [Fact]
    public void UserAgent_ShouldTrimTo500Characters_WhenInputIsTooLong()
    {
        var longAgent = new string('a', 600);

        var userAgent = new UserAgent(longAgent);

        userAgent.Value.Should().HaveLength(500);
    }
}