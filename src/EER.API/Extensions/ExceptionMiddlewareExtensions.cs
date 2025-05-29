using System.Net.Mime;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature is null)
                    return;

                var statusCode = contextFeature.Error switch
                {
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    OperationCanceledException => StatusCodes.Status499ClientClosedRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = statusCode;

                var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var pd = problemDetailsFactory.CreateProblemDetails(
                    context,
                    statusCode: statusCode,
                    detail: contextFeature.Error.Message,
                    instance: context.Request.Path
                );

                var acceptHeader = context.Request.Headers[HeaderNames.Accept].ToString();
                if (acceptHeader.Contains(MediaTypeNames.Application.Xml, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.ContentType = MediaTypeNames.Application.Xml;

                    var xmlPd = new ProblemDetailsXml
                    {
                        Type = pd.Type,
                        Title = pd.Title,
                        Status = pd.Status,
                        Detail = pd.Detail,
                        Instance = pd.Instance,
                        Extensions = pd.Extensions.Select(e => new ExtensionEntry
                        {
                            Key = e.Key,
                            Value = e.Value?.ToString()
                        })
                            .ToList(),
                    };

                    await using var memoryStream = new MemoryStream();
                    var xmlSerializer = new XmlSerializer(typeof(ProblemDetailsXml));
                    xmlSerializer.Serialize(memoryStream, xmlPd);
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(context.Response.Body);
                }
                else
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsJsonAsync(pd);
                }
            });
        });
    }

    [XmlRoot("ProblemDetails")]
    public class ProblemDetailsXml
    {
        public ProblemDetailsXml() { }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        public List<ExtensionEntry>? Extensions { get; set; }
    }

    public class ExtensionEntry
    {
        [XmlAttribute("key")]
        public string? Key { get; set; }

        [XmlText]
        public string? Value { get; set; }
    }
}
