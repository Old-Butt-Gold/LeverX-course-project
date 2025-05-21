using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EER.API.SwaggerSchemaFilters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var enumValues = Enum.GetValues(context.Type);
            var underlyingType = Enum.GetUnderlyingType(context.Type);

            schema.Description = "Possible values:";
            schema.Enum.Clear();

            foreach (var value in enumValues)
            {
                var name = value.ToString();
                var numericValue = Convert.ChangeType(value, underlyingType);
                schema.Description += $"\n- {name} ({numericValue})";
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}
