using GeoGuard.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace GeoGuard.Api.Extensions;

public static class ResultExtension
{
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);
        return result.StatusCode switch
        {
            404 => controller.NotFound(new { error = result.Error }),
            400 => controller.BadRequest(new { error = result.Error }),
            409 => controller.Conflict(new { error = result.Error }),
            _ => controller.StatusCode(500, new { error = result.Error })
        };
    }

    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok();
        return result.StatusCode switch
        {
            404 => controller.NotFound(new { error = result.Error }),
            400 => controller.BadRequest(new { error = result.Error }),
            409 => controller.Conflict(new { error = result.Error }),
            _ => controller.StatusCode(500, new { error = result.Error })
        };
    }
}
