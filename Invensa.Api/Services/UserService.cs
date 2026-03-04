namespace Invensa.Api.Services;

using Domain.Custom;
using Domain.Exceptions;
using Infrastructure.Services.Interfaces;
using LanguageExt.Common;
using System.Security.Claims;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<UserService> _logger;

    public UserService(IHttpContextAccessor contextAccessor, ILogger<UserService> logger)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    public Result<UserAuthentication> GetUser()
    {
        try
        {
            _logger.LogInformation("Trying to get the claims principal");

            var user = _contextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("Claims principal are empty or not authenticated.");
                return new Result<UserAuthentication>(new UnAuthorizedException("User is not authenticated."));
            }

            _logger.LogInformation("Claims principal obtained");

            // En nuestro JWT local usamos ClaimTypes.NameIdentifier para el ID del usuario
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogInformation("NameIdentifier claim does not exist");
                return new Result<UserAuthentication>(new UnAuthorizedException("El token no contiene el identificador de usuario."));
            }

            _logger.LogInformation("User identifier claim exists: {UserId}", userIdClaim);

            // Verificamos si es administrador mediante el claim 'isAdmin' que definimos en JwtTokenService
            var isAdminClaim = user.FindFirst("isAdmin")?.Value;
            var isAdmin = isAdminClaim != null && isAdminClaim.Equals("true", StringComparison.OrdinalIgnoreCase);

            var userAuthentication = new UserAuthentication
            {
                UserId = userIdClaim,
                IsAdmin = isAdmin,
            };

            _logger.LogInformation("User authenticated. IsAdmin: {IsAdmin}", userAuthentication.IsAdmin);

            return new Result<UserAuthentication>(userAuthentication);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error verifying user: {error}", ex.Message);
            return new Result<UserAuthentication>(new InfrastructureException("OcurriÃ³ un error tratando de verificar al usuario"));
        }
    }
}
