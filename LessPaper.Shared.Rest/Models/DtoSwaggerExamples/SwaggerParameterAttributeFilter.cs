using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Security.Policy;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    public class SwaggerParameterAttributeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Determine SwaggerParameterAttributes of current method
            var attributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<SwaggerParameterExampleValue>()
                .ToList();

            // Ensure at least one attribute exists
            if (attributes == null || !attributes.Any())
                return;

            //Convert OpenApiParameters and the corresponding parameter type to dictionary
            var openApiParameters = operation
                .Parameters
                .ToDictionary(x => x.Name.ToLower(), k => k);

            var methodParameterTypes = context
                .MethodInfo
                .GetParameters()
                .ToDictionary(x => x.Name.ToLower(), x => x.ParameterType);

            //Loop each attribute
            foreach (var attribute in attributes)
            {
                var targetParameterName = attribute.ParameterName.ToLower();

                // Resolve the real method parameters from given attribute parameter name
                if (!openApiParameters.TryGetValue(targetParameterName, out var openApiParameter) ||
                    !methodParameterTypes.TryGetValue(targetParameterName, out var parameterType))
                    throw new KeyNotFoundException(
                        $"Swagger example attribute for parameter {attribute.ParameterName} of method {context.MethodInfo.Name} was" +
                        $" defined but the method does not contain any parameter with this name. Check spelling.");
                
                // Generate example value of correct type
                if (parameterType == typeof(string))
                    openApiParameter.Example = new OpenApiString(attribute.ParameterValue.ToString());
                else if (parameterType == typeof(int))
                    openApiParameter.Example = new OpenApiInteger((int)attribute.ParameterValue);
                else if (parameterType == typeof(double))
                    openApiParameter.Example = new OpenApiDouble((double)attribute.ParameterValue);
                else if (parameterType == typeof(long))
                    openApiParameter.Example = new OpenApiLong((long)attribute.ParameterValue);
                else if (parameterType == typeof(bool))
                    openApiParameter.Example = new OpenApiBoolean((bool)attribute.ParameterValue);
                else if (parameterType == typeof(byte))
                    openApiParameter.Example = new OpenApiByte((byte)attribute.ParameterValue);
                else
                    throw new NotImplementedException($"Generation of swagger example for parameter of type {parameterType.Name} " +
                                                      $"not implemented yet. Error occured for method with name {context.MethodInfo.Name} " +
                                                      $"and parameter with name {openApiParameter.Name}");

            }


        }
    }

}