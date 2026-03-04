namespace Invensa.Application.DTOs.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfo User
);

public record UserInfo(
    Guid Id,
    string Email,
    string Name,
    bool IsAdmin,
    bool Active
);
