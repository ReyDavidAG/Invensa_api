namespace Invensa.Infrastructure.Helpers;

public static class Auth0Helpers
{
    public static string GetUserIdFromSub(string sub)
    {
        var index = sub.IndexOf('|');
        var id = sub.Substring(index + 1);

        return id;

        //return sub.Split('|').Last();
    }
}