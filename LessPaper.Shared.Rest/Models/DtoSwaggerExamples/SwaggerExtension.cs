using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    public static class SwaggerExtension
    {
        /// <summary>
        /// Register all swagger examples within the LessPaper.Shared.Rest library
        /// </summary>
        /// <param name="collection"></param>
        public static void RegisterSwaggerSharedDtoExamples(this IServiceCollection collection)
        {
            collection.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
        }
    }
}
