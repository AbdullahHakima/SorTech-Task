using FluentAssertions;
using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.ValueObjects;
using GeoGuard.Infrastructure.Exceptions;
using GeoGuard.Infrastructure.Services;
using Moq;

namespace GeoGuard.Tests;

public class CountryManagementServiceTests
{
    private readonly Mock<IBlockedCountryRepository> _countryRepository = new();
    private readonly Mock<ICountryNameService> _countryNameService = new();

    [Fact]
    public async Task TemporalBlockCountryAsync_ShouldThrow_WhenDurationIsOutOfRange()
    {
        var service = CreateService();

        var action = () => service.TemporalBlockCountryAsync("US", 0);

        await action.Should().ThrowAsync<GeoGuardSeviceException>();
    }

    [Fact]
    public async Task BlockedListAsync_ShouldThrow_WhenBothCountryNameAndCountryCodeProvided()
    {
        var service = CreateService();

        var action = () => service.BlockedListAsync("Canada", "CA");

        await action.Should().ThrowAsync<GeoGuardSeviceException>();
    }

    [Fact]
    public async Task BlockCountryAsync_ShouldAddCountryToRepository()
    {
        var service = CreateService();
        _countryNameService.Setup(x => x.GetName(It.IsAny<CountryCode>())).ReturnsAsync("United States");
        _countryRepository
            .Setup(x => x.AddAsync(It.IsAny<BlockedCountry>()))
            .ReturnsAsync((BlockedCountry country) => Result<BlockedCountry>.Success(country));

        var result = await service.BlockCountryAsync("US");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CountryCode.Value.Should().Be("US");
    }

    [Fact]
    public async Task BlockedListAsync_ShouldClampPageSizeTo50_WhenRequestedSizeIsGreaterThan50()
    {
        var service = CreateService();
        _countryRepository
            .Setup(x => x.GetAllAsync(null, null, 1, 50))
            .ReturnsAsync(Result<PagedResult<BlockedCountry>>.Success(new PagedResult<BlockedCountry>(0, 0, 1, [])));

        var result = await service.BlockedListAsync(null, null, 1, 500);

        result.IsSuccess.Should().BeTrue();
        _countryRepository.Verify(x => x.GetAllAsync(null, null, 1, 50), Times.Once);
    }

    private CountryManagementService CreateService() =>
        new(_countryRepository.Object, _countryNameService.Object);
}