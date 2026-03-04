namespace Invensa.Domain.Custom;

using System.Text.Json.Serialization;

public class Auth0User
{
    [JsonPropertyName("connection")] public string? Connection { get; set; }

    [JsonPropertyName("email")] public string? Email { get; set; }

    [JsonPropertyName("password")] public string? Password { get; set; }

    [JsonPropertyName("blocked")] public bool? Blocked { get; set; }

}