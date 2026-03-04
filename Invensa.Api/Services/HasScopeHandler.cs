namespace Invensa.Api.Services;

using Microsoft.AspNetCore.Authorization;

public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
    private readonly ILogger<HasScopeHandler> _logger;

    public HasScopeHandler(ILogger<HasScopeHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        _logger.LogInformation("Checking if 'permissions' claim exist");
        if (!context.User.HasClaim(c => c.Type == "permissions" && c.Issuer == requirement.Issuer))
        {
            _logger.LogInformation("'permissions' claim doesn't exist");
            return Task.CompletedTask;
        }

        _logger.LogInformation("'permissions' exists. Ensuring if 'permissions' has {requirement}", requirement.Scope);

        var hasPermissions = context!.User.HasClaim(c => c.Type == "permissions" && c.Issuer == requirement.Issuer && c.Value == requirement.Scope);

        _logger.LogInformation("Has permissions: {permissions}", hasPermissions);

        if (hasPermissions)
        {
            _logger.LogInformation("'permissions' is correct");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogInformation("'permissions' does not have expected value");
        }

        return Task.CompletedTask;
    }
}