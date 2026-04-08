using GeoGuard.Api.Extensions;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GeoGuard.Api.Controllers;

/// <summary>
/// Provides endpoints for querying IP geolocation details and verifying caller access.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class IpLookupController : ControllerBase
{
    private readonly IIpLookupService _lookupService;
    private readonly IBlockVerificationService _verificationService;

    public IpLookupController(IIpLookupService lookupService, IBlockVerificationService verificationService)
    {
        _lookupService = lookupService;
        _verificationService = verificationService;
    }

    /// <summary>
    /// Looks up geographical details for a given IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to look up. If omitted, the caller's IP is used.</param>
    /// <returns>Geographical details of the parsed IP address.</returns>
    /// <response code="200">Returns IP geolocation details.</response>
    /// <response code="400">If the provided IP address is in an invalid format.</response>
    [HttpGet("lookup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
    {
        IPAddress ip;
        if (string.IsNullOrEmpty(ipAddress))
            ip = HttpContext.Connection.RemoteIpAddress!;
        else if (!IPAddress.TryParse(ipAddress, out ip!))
            return BadRequest("Invalid ip address format.");
        return (await _lookupService.GetIpDetailsAsync(ip)).ToActionResult(this);
    }

    /// <summary>
    /// Validates the caller's IP against the blocked countries list and logs the attempt if blocked.
    /// </summary>
    /// <param name="testIp">Allows overriding the IP for local testing (currently ignored).</param>
    /// <returns>A boolean flag stating whether the IP is blocked or allowed access.</returns>
    /// <response code="200">Returns access verification result.</response>
    [HttpGet("check-block")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckBlock([FromQuery] string? testIp)
    {
        //IPAddress? ip = null;
        var ip = HttpContext.Connection.RemoteIpAddress!;
        //if (!string.IsNullOrEmpty(testIp))
        //    IPAddress.TryParse(testIp, out ip!); // for loacl testing
        var userAgent = new UserAgent(Request.Headers.UserAgent.ToString());
        return (await _verificationService.VerifyAccessAsync(ip, userAgent)).ToActionResult(this);
    }
}
