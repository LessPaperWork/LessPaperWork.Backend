using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    public class GetObjectsPermissionDtoSwaggerExample : IExamplesProvider<GetObjectsPermissionDto>
    {
        /// <inheritdoc />
        public GetObjectsPermissionDto GetExamples()
        {
            return new GetObjectsPermissionDto("0146613259928c4bd8a58dfd0fca344e47", new []{""});
        }
    }
}
