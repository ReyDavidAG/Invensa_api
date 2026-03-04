namespace Invensa.Application.DTOs.Auth;

public record RegisterRequest(
    string Email,
    string Name,
    string Password,
    bool IsAdmin = false
);
