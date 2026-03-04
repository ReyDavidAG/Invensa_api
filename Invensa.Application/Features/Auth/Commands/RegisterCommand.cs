using Invensa.Application.DTOs.Auth;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Email,
    string Name,
    string Password,
    bool IsAdmin = false
) : IRequest<Result<AuthResponse>>;
