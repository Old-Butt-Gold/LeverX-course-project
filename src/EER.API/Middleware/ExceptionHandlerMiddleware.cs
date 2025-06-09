using System.Net.Mime;
using System.Security.Claims;
using System.Xml.Serialization;
using EER.API.ProblemDetailsXml;
using EER.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

namespace EER.API.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IHostEnvironment _env;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger,
        ProblemDetailsFactory problemDetailsFactory, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _problemDetailsFactory = problemDetailsFactory;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ConflictException => StatusCodes.Status409Conflict,
            ValidationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            OperationCanceledException => StatusCodes.Status499ClientClosedRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var userId = context.User.FindFirst(ClaimTypes.Sid)?.Value ?? "Anonymous";

        _logger.LogError(
            "Exception: {ExceptionType} | User: {UserId} | Path: {Path} | Method: {Method} | StatusCode: {StatusCode}",
            exception.GetType().Name, userId, context.Request.Path, context.Request.Method, statusCode);

        object problemDetails;

        if (exception is ValidationException validationException)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in validationException.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            problemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                context, modelState, statusCode: statusCode, detail: "Validation failed: ", instance: context.Request.Path);
        }
        else
        {
            problemDetails = _problemDetailsFactory.CreateProblemDetails(
                context, statusCode: statusCode,
                detail: _env.IsDevelopment() || statusCode != 500
                    ? exception.Message : "An error occurred",
                instance: context.Request.Path);
        }

        var acceptHeader = context.Request.Headers[HeaderNames.Accept].ToString();
        if (acceptHeader.Contains(MediaTypeNames.Application.Xml, StringComparison.OrdinalIgnoreCase))
        {
            await SerializeXmlResponse(context, problemDetails);
        }
        else
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
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
