namespace Invensa.Domain.Custom;

public class Auth0Configuration
{
    public string Audience { get; set; } = null!;

    public string AudienceBackend { get; set; } = null!;

    public string Authority { get; set; } = null!;

    public string ClientId { get; set; } = null!;

    public string ClientSecrets { get; set; } = null!;

    public string Connection { get; set; } = null!;

    public string ConnectionId { get; set; } = null!;

    public string Domain { get; set; } = null!;

    public string GrantType { get; set; } = null!;

    public bool IsPaid { get; set; }

    public Auth0Roles Roles { get; set; } = null!;
}

public class Auth0Roles
{
    public string Active { get; set; } = null!;
    public string Admin { get; set; } = null!;

}