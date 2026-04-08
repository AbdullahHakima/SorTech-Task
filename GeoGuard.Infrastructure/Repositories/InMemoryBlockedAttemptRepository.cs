using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.Interfaces;
using System.Collections.Concurrent;

namespace GeoGuard.Infrastructure.Repositories;

public class InMemoryBlockedAttemptRepository : IBlockedAttemptRepository
{
    private static readonly ConcurrentQueue<BlockedAttemptLog> _logs = new();

    public async Task<Result> AddLogAsync(BlockedAttemptLog attemptLog)
    {
        _logs.Enqueue(attemptLog);
        return Result.Success();
    }

    public async Task<Result<PagedResult<BlockedAttemptLog>>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
    {
        pageSize = pageSize > 50 ? Math.Clamp(pageSize, 20, 50) : pageSize;
        var query = _logs.AsQueryable().OrderByDescending(q => q.TimeStamp);
        int totalCount = query.Count();
        if (totalCount == 0)
            return Result<PagedResult<BlockedAttemptLog>>.Success(
                                    new PagedResult<BlockedAttemptLog>(
                                        totalCount, 0, pageNumber, []));

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var attemptLogs = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return Result<PagedResult<BlockedAttemptLog>>.Success(
                                    new PagedResult<BlockedAttemptLog>(
                                        totalCount, totalPages,
                                        pageNumber, attemptLogs));
    }
}
