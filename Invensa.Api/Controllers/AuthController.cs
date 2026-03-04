using Invensa.Application.DTOs.Auth;
using Invensa.Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Invensa.Api.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [SwaggerResponse(StatusCodes.Status200OK, "Login exitoso", typeof(AuthResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Credenciales inválidas", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken ct)
    {
        var result = await Mediator.Send(request, ct);
        return result.ToOk(response => response);
    }

    [HttpPost("register")]
    [SwaggerResponse(StatusCodes.Status200OK, "Registro exitoso", typeof(AuthResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Datos inválidos", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(request.Email, request.Name, request.Password, request.IsAdmin);
        var result = await Mediator.Send(command, ct);
        return result.ToOk(response => response);
    }

    [HttpPost("refresh")]
    [SwaggerResponse(StatusCodes.Status200OK, "Token renovado", typeof(AuthResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Token inválido", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await Mediator.Send(command, ct);
        return result.ToOk(response => response);
    }
}
