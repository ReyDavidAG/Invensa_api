namespace Invensa.Api.Validation;

using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

public static class ValidationExtension
{
    public static ValidationProblemDetails ToProblemDetails(this ValidationException ex)
    {
        var error = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest
        };

        foreach (var validationFailure in ex.Errors)
        {
            var propertyName = validationFailure.PropertyName[0].ToString().ToLower() +
                               validationFailure.PropertyName[1..];
            var containsKey = error.Errors.ContainsKey(propertyName);

            if (containsKey)
            {
                error.Errors[propertyName] = error.Errors[propertyName]
                    .Concat(new[] { validationFailure.ErrorMessage }).ToArray();

                continue;
            }

            error.Errors.Add(new KeyValuePair<string, string[]>(propertyName,
                new[] { validationFailure.ErrorMessage }));
        }

        return error;
    }

    public static ValidationProblemDetails ToProblemDetails(
        this Domain.Exceptions.ValidationException ex)
    {
        var error = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Revise los errores de validación",
            Title = "Ha ocurrido un problema de validación"
        };

        if (ex?.Errors == null) return error;

        foreach (var validationFailure in ex.Errors)
        {
            var propertyName = validationFailure.Key[0].ToString().ToLower() +
                               validationFailure.Key[1..];
            var containsKey = error.Errors.ContainsKey(propertyName);

            if (containsKey)
            {
                error.Errors[propertyName] = error.Errors[propertyName]
                    .Concat(validationFailure.Value).ToArray();

                continue;
            }

            error.Errors.Add(new KeyValuePair<string, string[]>(propertyName,
                validationFailure.Value.ToArray()));
        }

        return error;
    }

    public static ValidationProblemDetails ToProblemDetails(
        this UnAuthorizedException ex)
    {
        var error = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status401Unauthorized,
            Detail = ex.Message,
            Title = ex.Message
        };

        return error;
    }

    public static ValidationProblemDetails ToProblemDetails(
        this BadRequestException ex)
    {
        var error = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Detail = ex.Message,
            Title = ex.Message
        };

        if (ex?.Errors == null) return error;

        foreach (var validationFailure in ex.Errors)
        {
            var propertyName = validationFailure.Key[0].ToString().ToLower() +
                               validationFailure.Key[1..];
            var containsKey = error.Errors.ContainsKey(propertyName);

            if (containsKey)
            {
                error.Errors[propertyName] = error.Errors[propertyName]
                    .Concat(validationFailure.Value).ToArray();

                continue;
            }

            error.Errors.Add(new KeyValuePair<string, string[]>(propertyName,
                validationFailure.Value.ToArray()));
        }

        return error;
    }
}