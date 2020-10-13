using LessPaper.Shared.Rest.Models.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    public class CreateDirectoryDtoSwaggerExample : IExamplesProvider<CreateDirectoryDto>
    {
        public CreateDirectoryDto GetExamples()
        {
            return new CreateDirectoryDto()
            {
                SubDirectoryName = "TestDirectory"
            };
        }
    }
}