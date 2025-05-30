using EER.API.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EER.API.Filters;

public class RequiredHeaderFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var attributes = context.ActionDescriptor.EndpointMetadata
            .OfType<RequiredHeaderAttribute>();

        foreach (var attr in attributes)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(attr.HeaderName, out var headerValue))
            {
                context.Result = new BadRequestObjectResult($"Missing required header: {attr.HeaderName}");
                return Task.CompletedTask;
            }

            if (attr.AllowedValues?.Length > 0)
            {
                var comparison = attr.IgnoreCase
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal;

                var isValid = attr.AllowedValues
                    .Any(v => v.Equals(headerValue.ToString(), comparison));

                if (!isValid)
                {
                    context.Result = new BadRequestObjectResult(
                        $"Invalid value for header {attr.HeaderName}. Allowed: {string.Join(", ", attr.AllowedValues)}");
                    return Task.CompletedTask;
                }
            }
        }

        return next();
    }
}
