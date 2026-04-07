using GeoGuard.Domain.Common;
using GeoGuard.Domain.Controllers.BlockedCountries.DTOs;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace GeoGuard.Infrastructure.Repositories;

public class InMemoryBlockedCountryRepository : IBlockedCountryRepository
{
    private static readonly ConcurrentDictionary<string, BlockedCountry> _context = new();

    public async Task<Result<BlockedCountry>> AddAsync(AddCountryToBlockedListRequest request)
    {
        // used casue the ConcurrentDictionary is actually sync not async to it case lack of using async 
        await Task.CompletedTask;
        var countryCode = new CountryCode(request.CountryCodeString);
        var blockedCountry = BlockedCountry.Create(request.CountryName, countryCode, DateTime.UtcNow,request.ExpirationDate);
        bool isAdded = _context.TryAdd(countryCode.Value, blockedCountry);
        if (!isAdded)
            return Result<BlockedCountry>.BadRequest("The Country Already blocked.");
        return Result<BlockedCountry>.Success(blockedCountry);
    }

    public async Task<Result<PagedResult<BlockedCountry>>> GetAllAsync(GetBlockedListRequest request)
    {
        await Task.CompletedTask;
        var query =  _context.Values.AsQueryable();
        if (!string.IsNullOrEmpty(request.CountryCode))
        { 
            var code = new CountryCode(request.CountryCode);
            query = query.Where(q => q.CountryCode == code);
        }
        if (!string.IsNullOrEmpty(request.CountryName))
            query = query.Where(q => q.Name == request.CountryName);
        int totalCount = query.Count();
        if (totalCount == 0)
            return Result<PagedResult<BlockedCountry>>.Success(new PagedResult<BlockedCountry>(
                                                         totalCount, 0,
                                                         request.PageNumber, []));

        int totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var countries =  query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

        return Result<PagedResult<BlockedCountry>>.Success(
                                new PagedResult<BlockedCountry>(
                                 totalCount,totalPages,
                                 request.PageNumber,countries));

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

    public async Task<Result<BlockedCountry>> RemoveAsync(string CountryCodeString)
    {
        var countryCode = new CountryCode(CountryCodeString);
        await Task.CompletedTask;
        bool isRemoved = _context.Remove(countryCode.Value, out var removedCountry);
        if (!isRemoved)
            return Result<BlockedCountry>.NotFound($"No country with code:{countryCode.Value} was found in the blocked list.");
        return Result<BlockedCountry>.Success(removedCountry!);

    }
}
