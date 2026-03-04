using Invensa.Application.DTOs.Auth;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
