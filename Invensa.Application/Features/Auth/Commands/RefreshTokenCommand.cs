using Invensa.Application.DTOs.Auth;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
