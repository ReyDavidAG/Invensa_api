namespace Invensa.Api.Filters;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateModelStateFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        var existsErrors = context.ModelState.Values.Any(x => x.Errors.Count > 0);

        if (!existsErrors) return;

        const string propertyValidationErrorsName = "errors";
        var problemDetails = new ProblemDetails
        {
            Title = "Validation problems.",
            Detail = "Exists some validation problems in the request.",
            Instance = context.HttpContext.Request.GetDisplayUrl()
        };
        var validationErrors = context.ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
        var propertiesErrors = new Dictionary<string, List<string>>();

        foreach (var validationError in validationErrors)
        {
            var keyName = validationError.Key[0].ToString().ToLower() + validationError.Key[1..];
            keyName = keyName.Replace("_", string.Empty);
            var errorMessages = validationError.Value.Errors.Select(x => x.ErrorMessage).ToList();
            propertiesErrors.Add(keyName, errorMessages);
        }

        problemDetails.Extensions.Add(propertyValidationErrorsName, propertiesErrors);

        context.Result = new BadRequestObjectResult(problemDetails);
    }
}