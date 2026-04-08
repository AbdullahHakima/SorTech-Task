using GeoGuard.Api.Extensions;
using GeoGuard.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GeoGuard.Api.Controllers
{
    /// <summary>
    /// Audits and retrieves logs for blocked IP connection attempts.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IBlockedAttemptRepository _attemptRepository;

        public LogsController(IBlockedAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }

        /// <summary>
        /// Retrieves paginated logs of caller IPs that were blocked.
        /// </summary>
        /// <param name="page">The page number to retrieve (default: 1).</param>
        /// <param name="pageSize">The number of items per page (default: 20).</param>
        /// <returns>A paginated list of blocked attempts.</returns>
        /// <response code="200">Successfully retrieved the blocked attempts logs.</response>
        /// <response code="400">If the pagination parameters are invalid.</response>
        [HttpGet("blocked-attempts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AttemptList([FromQuery] int page = 1, int pageSize = 20)
                    => (await _attemptRepository.GetAllAsync(page, pageSize)).ToActionResult(this);

    }
}
