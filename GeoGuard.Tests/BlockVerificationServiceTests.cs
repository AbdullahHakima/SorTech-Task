using FluentAssertions;
using GeoGuard.Domain.Common;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.Services;
using GeoGuard.Domain.ValueObjects;
using Moq;
using System.Net;

namespace GeoGuard.Tests;

public class BlockVerificationServiceTests
{
    private readonly Mock<IBlockedCountryRepository> _countryRepository = new();
    private readonly Mock<IBlockedAttemptRepository> _attemptRepository = new();
    private readonly Mock<IIpLookupService> _ipLookupService = new();

    [Fact]
    public async Task VerifyAccessAsync_ShouldReturnNotFound_WhenIpLookupFails()
    {
        var service = CreateService();
        var ip = IPAddress.Parse("8.8.8.8");
        var userAgent = new UserAgent("agent");

        _ipLookupService
            .Setup(x => x.GetCountryCodeAsync(ip))
            .ReturnsAsync(Result<string>.NotFound("not found"));

        var result = await service.VerifyAccessAsync(ip, userAgent);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        _attemptRepository.Verify(x => x.AddLogAsync(It.IsAny<BlockedAttemptLog>()), Times.Never);
    }

    [Fact]
    public async Task VerifyAccessAsync_ShouldReturnBlocked_WhenCountryIsActivelyBlocked()
    {
        var service = CreateService();
        var ip = IPAddress.Parse("1.1.1.1");
        var userAgent = new UserAgent("agent");
        var countryCode = new CountryCode("US");
        var blockedCountry = BlockedCountry.Create("United States", countryCode, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5));

        _ipLookupService.Setup(x => x.GetCountryCodeAsync(ip)).ReturnsAsync(Result<string>.Success("US"));
        _countryRepository.Setup(x => x.GetByCodeAsync(countryCode)).ReturnsAsync(Result<BlockedCountry>.Success(blockedCountry));
        _attemptRepository.Setup(x => x.AddLogAsync(It.IsAny<BlockedAttemptLog>())).ReturnsAsync(Result.Success());

        var result = await service.VerifyAccessAsync(ip, userAgent);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _attemptRepository.Verify(x => x.AddLogAsync(It.Is<BlockedAttemptLog>(a => a.IsBlocked)), Times.Once);
    }

    [Fact]
    public async Task VerifyAccessAsync_ShouldReturnAllowed_WhenCountryBlockIsExpired()
    {
        var service = CreateService();
        var ip = IPAddress.Parse("9.9.9.9");
        var userAgent = new UserAgent("agent");
        var countryCode = new CountryCode("CA");
        var blockedCountry = BlockedCountry.Create("Canada", countryCode, DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddMinutes(-10));

        _ipLookupService.Setup(x => x.GetCountryCodeAsync(ip)).ReturnsAsync(Result<string>.Success("CA"));
        _countryRepository.Setup(x => x.GetByCodeAsync(countryCode)).ReturnsAsync(Result<BlockedCountry>.Success(blockedCountry));
        _attemptRepository.Setup(x => x.AddLogAsync(It.IsAny<BlockedAttemptLog>())).ReturnsAsync(Result.Success());

        var result = await service.VerifyAccessAsync(ip, userAgent);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
        _attemptRepository.Verify(x => x.AddLogAsync(It.Is<BlockedAttemptLog>(a => !a.IsBlocked)), Times.Once);
    }

    private BlockVerificationService CreateService() =>
        new(_countryRepository.Object, _attemptRepository.Object, _ipLookupService.Object);
}