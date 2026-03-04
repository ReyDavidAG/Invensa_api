namespace Invensa.Infrastructure.Services.Interfaces;

using Domain.Custom;
using LanguageExt.Common;

public interface IUserService
{
    Result<UserAuthentication> GetUser();
}