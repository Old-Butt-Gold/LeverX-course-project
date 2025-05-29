using EER.API.CustomAttributes;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EER.API.Filters;

public class AddHeaderFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var attributes = context.ActionDescriptor.EndpointMetadata
            .OfType<AddHeaderAttribute>()
            .ToList();

        if (attributes.Count != 0)
        {
            foreach (var attribute in attributes)
            {
                ApplyHeader(context.HttpContext.Response.Headers, attribute);
            }
        }

        await next();
    }

    private static void ApplyHeader(IHeaderDictionary headers, AddHeaderAttribute attribute)
    {
        if (headers.ContainsKey(attribute.Name))
        {
            if (attribute.Overwrite)
            {
                headers[attribute.Name] = attribute.Value;
            }
            else
            {
                headers[attribute.Name] += $", {attribute.Value}";
            }
        }
        else
        {
            headers.Append(attribute.Name, attribute.Value);
        }
    }
}
