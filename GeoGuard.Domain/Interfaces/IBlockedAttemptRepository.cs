using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;

namespace GeoGuard.Domain.Interfaces;

public interface IBlockedAttemptRepository
{
    Task<Result> AddLogAsync(BlockedAttemptLog attemptLog);
    Task<Result<PagedResult<BlockedAttemptLog>>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
}
