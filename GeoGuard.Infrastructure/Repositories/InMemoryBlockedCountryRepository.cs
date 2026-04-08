using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace GeoGuard.Infrastructure.Repositories;

public class InMemoryBlockedCountryRepository : IBlockedCountryRepository
{
    private static readonly ConcurrentDictionary<string, BlockedCountry> _context = new();
    private readonly ICountryNameService _countryNameService;

    public InMemoryBlockedCountryRepository(ICountryNameService countryNameService)
    {
        _countryNameService = countryNameService;
    }

    public async Task<Result<BlockedCountry>> AddAsync(BlockedCountry blockedCountry)
    {
        // used casue the ConcurrentDictionary is actually sync not async to it case lack of using async 
        await Task.CompletedTask;
        bool isAdded = _context.TryAdd(blockedCountry.CountryCode.Value, blockedCountry);
        if (!isAdded)
            return Result<BlockedCountry>.Conflict($"{blockedCountry.CountryCode} is Already blocked.");
        return Result<BlockedCountry>.Success(blockedCountry);
    }

    public async Task<Result<PagedResult<BlockedCountry>>> GetAllAsync(string? countryName, CountryCode? countryCode,
                                                                       int pageNumber = 1, int pageSize = 20)
    {
        await Task.CompletedTask;
        var query =  _context.Values.AsQueryable();
        if (countryCode is not null)
            query = query.Where(q => q.CountryCode == countryCode);
        if (!string.IsNullOrEmpty(countryName))
            query = query.Where(q => q.Name == countryName);
        int totalCount = query.Count();
        if (totalCount == 0)
            return Result<PagedResult<BlockedCountry>>.Success(new PagedResult<BlockedCountry>(
                                                         totalCount, 0,
                                                         pageNumber, []));

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var countries =  query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return Result<PagedResult<BlockedCountry>>.Success(
                                new PagedResult<BlockedCountry>(
                                 totalCount,totalPages,
                                 pageNumber,countries));

    }

    public async Task<Result<BlockedCountry>> GetByCodeAsync(CountryCode code)
    {
        await Task.CompletedTask;
        var blockedCountry = _context.GetValueOrDefault(code.Value);
        if (blockedCountry is null)
            return Result<BlockedCountry>.NotFound(
                $"No Country with code:{code.Value} in blocked list was found.");
        return Result<BlockedCountry>.Success(blockedCountry);
    }

    public async Task<Result<BlockedCountry>> RemoveAsync(CountryCode countryCode)
    {
        await Task.CompletedTask;
        bool isRemoved = _context.Remove(countryCode.Value, out var removedCountry);
        if (!isRemoved)
            return Result<BlockedCountry>.NotFound($"No country with code:{countryCode} was found in the blocked list.");
        return Result<BlockedCountry>.Success(removedCountry!);

    }
}
