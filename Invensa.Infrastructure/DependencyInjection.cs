using Invensa.Domain.Custom;
using Invensa.Domain.Interfaces;
using Invensa.Infrastructure.Data;
using Invensa.Infrastructure.Repositories;
using Invensa.Infrastructure.Services;
using Invensa.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Invensa.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("InvensaConn");
        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("Falta ConnectionStrings:InvensaConn");

        services.AddDbContext<InvensaDbContext>(options =>
            options.UseSqlServer(conn, builder => builder.EnableRetryOnFailure(4, TimeSpan.FromSeconds(5), null)));
        services.AddHttpClient();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<IImageStorage, FileSystemImageStorage>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Servicios de Autenticación Local
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.Configure<ImageStorageOptions>(configuration.GetSection("ImageStorage"));
    }
}
