namespace Invensa.Api.OpenApi;

using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConfigureSwaggerOptions" /> class.
    /// </summary>
    /// <param name="provider">
    ///     The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger
    ///     documents.
    /// </param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder("API's for Invensa :3");
        var info = new OpenApiInfo
        {
            Title = "Invensa.Api",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated) text.Append(" This API version has been deprecated.");

        if (description.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');

            if (policy.HasLinks)
            {
                text.AppendLine();

                foreach (var link in policy.Links)
                {
                    if (link.Type != "text/html") continue;

                    text.AppendLine();

                    if (link.Title.HasValue) text.Append(link.Title.Value).Append(": ");

                    text.Append(link.LinkTarget.OriginalString);
                }
            }
        }

        info.Description = text.ToString();

        return info;
    }
}