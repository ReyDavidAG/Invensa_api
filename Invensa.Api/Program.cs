using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Invensa.Api.Hubs;          // DashboardHub
using Invensa.Api.OpenApi;       // ConfigureSwaggerOptions, SwaggerDefaultValues
using Invensa.Api.Services;      // UserService, DashboardNotifier
using Invensa.Application;       // AddApplication()
using Invensa.Domain.Interfaces; // IDashboardNotifier
using Invensa.Infrastructure;    // AddInfrastructureServices()
using Invensa.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración para funcionar como Servicio de Windows
builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "Invensa.Api";
});

builder.Host.UseDefaultServiceProvider(o => { o.ValidateOnBuild = true; o.ValidateScopes = true; });
// 1. Controllers
builder.Services.AddControllers();

// 1.5 SignalR (Real-Time WebSockets)
builder.Services.AddSignalR();

// 2. Api Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// 3. Swagger Configuration (Para que funcione en el navegador)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Description = "JWT Bearer. Ej: Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, new[] { "Bearer" } } });
    options.OperationFilter<SwaggerDefaultValues>();

    var xml = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

// 4. CORS
const string CorsPolicy = "FrontDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, p =>
        p.WithOrigins("http://localhost:5173", "https://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());  // Necesario para SignalR WebSockets
});

// 5. JWT Authentication Local Configuration
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] 
    ?? throw new InvalidOperationException("JWT SecretKey no configurada");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] 
    ?? throw new InvalidOperationException("JWT Issuer no configurado");
var jwtAudience = builder.Configuration["Jwt:Audience"] 
    ?? throw new InvalidOperationException("JWT Audience no configurado");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.NameIdentifier
    };

    // SignalR no puede enviar headers en WebSocket,
    // así que el token JWT se envía como query string "access_token"
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// 6. Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("isAdmin", "true");
    });
    
    options.AddPolicy("UserActive", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

// 7. Dependency Injection (Capas Externas)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDashboardNotifier, DashboardNotifier>();  // SignalR Notifier
builder.Services.AddInfrastructureServices(builder.Configuration); // Aquí están Repo, JWT Service, Password Hasher, etc.
builder.Services.AddApplication();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 8. Swagger en el Navegador
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            $"Invensa.Api {description.GroupName.ToUpperInvariant()}");
    }
    options.RoutePrefix = string.Empty; // Hace que Swagger sea la página de inicio al abrir la URL base
});

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);       
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR Hub endpoint
app.MapHub<DashboardHub>("/hubs/dashboard");

app.Run();