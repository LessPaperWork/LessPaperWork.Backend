using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SwaggerParameterExample : Attribute
    {
        /// <summary>
        /// Example value attribute
        /// </summary>
        /// <param name="parameterValue">Value of property. Must match property type</param>
        /// <param name="isRequired">Indicates if the parameter is required or optional</param>
        public SwaggerParameterExample(string parameterValue, bool isRequired = true)
        {
            ParameterValue = parameterValue;
            IsRequired = isRequired;
        }

        /// <summary>
        /// Indicates if the parameter is required or optional
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Value of property
        /// </summary>
        public object ParameterValue { get; }
    }
}
