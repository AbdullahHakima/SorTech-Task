using GeoGuard.Api.Extensions;
using GeoGuard.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GeoGuard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IBlockedAttemptRepository _attemptRepository;

        public LogsController(IBlockedAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }
        [HttpGet("blocked-attempts")]
        public async Task<IActionResult> AttemptList([FromQuery] int page = 1, int pageSize = 20)
                    => (await _attemptRepository.GetAllAsync(page, pageSize)).ToActionResult(this);

    }
}
