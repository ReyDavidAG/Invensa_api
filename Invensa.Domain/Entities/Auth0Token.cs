namespace Invensa.Domain.Entities;

public class Auth0Token
{
    public int Id { get; set; }
    public string? Token { get; set; }
    public DateTime ExpirationDate { get; set; } 

}