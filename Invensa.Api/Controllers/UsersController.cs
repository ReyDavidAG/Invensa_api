using Invensa.Application.Features.Users.Queries;
using Invensa.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Invensa.Api.Controllers;

[Authorize(Policy = "Admin")]
[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, "Users", typeof(IEnumerable<User>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request.", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetUsersByParameters([FromQuery] GetUsersByParametersQuery query,
        CancellationToken ct)
    {
        var result = await Mediator.Send(query, ct);

        return result.ToOk(users => users);
    }

    [HttpGet("self-user")]
    [SwaggerResponse(StatusCodes.Status200OK, "User", typeof(User))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request.", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetSelfUser(
    CancellationToken ct)
    {
        var result = await Mediator.Send(new GetSelfUserQuery(), ct);

        return result.ToOk(user => user);
    }
}
