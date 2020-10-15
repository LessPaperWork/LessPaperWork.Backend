using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    public class SwaggerParameterExampleFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Extract method parameters
            var parameters = context
                .MethodInfo
                .GetParameters()
                .ToList();

            // Check if SwaggerSingleParameterAttribute exists on any property
            if (parameters
                .Select(x => Attribute.GetCustomAttribute(x, typeof(SwaggerParameterExample)))
                .All(x => x == null))
                return;

            //Convert OpenApiParameters and the corresponding parameter type to dictionary
            //In case of an duplicate key exception check for matching path/query-parameter
            var openApiParameters = operation
                .Parameters
                .ToDictionary(x => x.Name.ToLower(), k => k);

            //Loop each parameter of the method
            foreach (var parameter in parameters)
            {
                var attribute = parameter
                    .GetCustomAttributes(typeof(SwaggerParameterExample), true)
                    .Cast<SwaggerParameterExample>()
                    .FirstOrDefault();

                if (attribute == null)
                    continue;

                var parameterType = parameter.ParameterType;
                var targetParameterName = parameter.Name.ToLower();

                // Resolve the real method parameters from given attribute parameter name
                if (!openApiParameters.TryGetValue(targetParameterName, out var openApiParameter))
                    throw new KeyNotFoundException(
                        $"OpenAPI definition could found {targetParameterName} of method {context.MethodInfo.Name}.");

                openApiParameter.Required = attribute.IsRequired;

                if (attribute.ParameterValue.GetType() != parameterType)
                {
                    throw new InvalidCastException($"Generation of swagger example failed. Example data of type {attribute.ParameterValue.GetType().Name} does not match target type {parameterType.Name}. " +
                                                   $"Error occured for method with name {context.MethodInfo.Name} " +
                                                   $"and parameter with name {openApiParameter.Name}.");
                }

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

