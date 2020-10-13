using Microsoft.OpenApi.Models;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Linq;
    public class SwaggerParameterAttributeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<SwaggerParameterxAttribute>();

            foreach (var attribute in attributes)
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = attribute.Name,
                    Description = attribute.Description,
                    Required = attribute.Required,
                });              
        }
    }

}