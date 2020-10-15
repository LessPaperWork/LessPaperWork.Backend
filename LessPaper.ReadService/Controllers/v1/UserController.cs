using System;
using System.Threading.Tasks;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Rest.Exceptions;
using LessPaper.Shared.Rest.Models.Dtos;
using LessPaper.Shared.Rest.Models.DtoSwaggerExamples;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.ReadService.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IGuardApi guardApi;

        public UserController(ILogger<UserController> logger, IGuardApi guardApi)
        {
            this.logger = logger;
            this.guardApi = guardApi;
        }

        [HttpGet("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserInformation(
            [FromRoute] [SwaggerParameterExample("test@swagger.de")] string email)
        {
            if (!ValidationHelper.IsValidEmailAddress(email))
                return BadRequest();

            var userInformation = await guardApi.GetUserInformation(email);
            if (userInformation == null)
                return BadRequest();

            return new OkObjectResult(new ExtendedUserInformationDto(userInformation));
        }
    }
}
