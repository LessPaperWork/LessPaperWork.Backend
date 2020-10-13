using System;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class SwaggerParameterxAttribute : Attribute
    {
        public SwaggerParameterxAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string DataType { get; set; }
        public string ParameterType { get; set; }
        public string Description { get; private set; }
        public bool Required { get; set; } = false;
    }

}