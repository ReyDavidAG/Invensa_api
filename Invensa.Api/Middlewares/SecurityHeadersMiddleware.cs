namespace Invensa.Api.Middlewares;

public class SecurityHeadersMiddleware
{
    private readonly IEnumerable<string> _allowedExternalApis;
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _allowedExternalApis = configuration.GetSection("AllowedExternalApis").Get<IEnumerable<string>>() ?? [];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var csp = string.Join("; ",
            // Base rule to allow content only from your domain
            "default-src 'self'",

            // Allow styles unsafe-inline to allow TailwindCSS
            "style-src 'self' 'unsafe-inline'",

            // Allow images from your domain and those embedded with 'data:'
            "img-src 'self' data:",

            // Allow connection to your domain and external apis used in client
            $"connect-src 'self' {string.Join(" ", _allowedExternalApis)}"
        );

        context.Response.Headers.Append("Content-Security-Policy", csp);
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Permissions-Policy",
            "accelerometer=(), ambient-light-sensor=(), autoplay=(), battery=(), camera=(), cross-origin-isolated=(), display-capture=(), document-domain=(), encrypted-media=(), encrypted-media=(), execution-while-not-rendered=(), execution-while-out-of-viewport=(), fullscreen=(), geolocation=(), gyroscope=(), keyboard-map=(), magnetometer=(), microphone=(), midi=(), navigation-override=(), payment=(), picture-in-picture=(), publickey-credentials-get=(), screen-wake-lock=(), sync-xhr=(), usb=(), web-share=(), xr-spatial-tracking=()");

        await _next(context);
    }
}
