using GeoGuard.Api.Extensions;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GeoGuard.Api.Controllers;

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

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
    {
        IPAddress ip;
        if (string.IsNullOrEmpty(ipAddress))
            ip = HttpContext.Connection.RemoteIpAddress!;
        else if (!IPAddress.TryParse(ipAddress, out ip!))
            return BadRequest("Invalid ip address format.");
        return (await _lookupService.GetIpDetailsAsync(ip)).ToActionResult(this);
    }
    [HttpGet("check-block")]
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
