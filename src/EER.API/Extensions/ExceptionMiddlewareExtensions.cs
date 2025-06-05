using System.Net.Mime;
using System.Xml.Serialization;
using EER.API.ProblemDetailsXml;
using EER.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace EER.API.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                logger.LogError("Exception handler triggered for request {Path}", context.Request.Path);

                if (contextFeature is null)
                {
                    logger.LogWarning("Exception handler called but no exception found");
                    return;
                }

                var statusCode = contextFeature.Error switch
                {
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    ConflictException => StatusCodes.Status409Conflict,
                    ValidationException => StatusCodes.Status400BadRequest,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    OperationCanceledException => StatusCodes.Status499ClientClosedRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = statusCode;

                var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                object pd;

                if (contextFeature.Error is ValidationException validationException)
                {
                    var modelStateDict = new ModelStateDictionary();
                    foreach (var error in validationException.Errors)
                    {
                        modelStateDict.AddModelError(error.PropertyName, error.ErrorMessage);
                    }

                    pd = problemDetailsFactory.CreateValidationProblemDetails(
                        context,
                        modelStateDict,
                        statusCode: statusCode,
                        detail: "Validation failed: ",
                        instance: context.Request.Path
                    );
                }
                else
                {
                    pd = problemDetailsFactory.CreateProblemDetails(
                        context,
                        statusCode: statusCode,
                        detail: contextFeature.Error.Message,
                        instance: context.Request.Path
                    );
                }

                var acceptHeader = context.Request.Headers[HeaderNames.Accept].ToString();
                if (acceptHeader.Contains(MediaTypeNames.Application.Xml, StringComparison.OrdinalIgnoreCase))
                {
                    await SerializeXmlResponse(context, pd);
                }
                else
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsJsonAsync(pd);
                }
            });
        });
    }

    private static async Task SerializeXmlResponse(HttpContext context, object problemDetails)
    {
        context.Response.ContentType = MediaTypeNames.Application.Xml;
        var xmlPd = new ProblemDetailsXml.ProblemDetailsXml();

        if (problemDetails is ProblemDetails basePd)
        {
            xmlPd.Type = basePd.Type;
            xmlPd.Title = basePd.Title;
            xmlPd.Status = basePd.Status;
            xmlPd.Detail = basePd.Detail;
            xmlPd.Instance = basePd.Instance;

            xmlPd.Extensions = basePd.Extensions
                .Select(e => new ExtensionEntry { Key = e.Key, Value = e.Value?.ToString() })
                .ToList();
        }

        if (problemDetails is ValidationProblemDetails validationPd && validationPd.Errors.Any())
        {
            xmlPd.ValidationErrors = validationPd.Errors
                .Select(e => new ValidationErrorEntry
                {
                    Field = e.Key,
                    Messages = e.Value
                })
                .ToList();
        }

        await using var memoryStream = new MemoryStream();
        var xmlSerializer = new XmlSerializer(typeof(ProblemDetailsXml.ProblemDetailsXml));
        xmlSerializer.Serialize(memoryStream, xmlPd);
        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(context.Response.Body);
    }
}
