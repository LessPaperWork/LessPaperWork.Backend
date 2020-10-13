using System;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class SwaggerParameterExampleValue : Attribute
    {
        public SwaggerParameterExampleValue(string parameterName, string parameterValue)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }

        public string ParameterName { get; }
        public object ParameterValue { get; }
    }
    
}