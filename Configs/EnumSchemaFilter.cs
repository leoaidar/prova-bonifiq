using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ProvaPub.Configs
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum = context.Type.GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Select(field => new OpenApiString(field.GetCustomAttribute<DisplayAttribute>()?.Name ?? field.Name))
                    .ToList<IOpenApiAny>();
            }
        }
    }
}
