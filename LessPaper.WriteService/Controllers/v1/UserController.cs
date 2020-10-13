using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LessPaper.WriteService.Controllers.v1
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


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(UserCreationDto), typeof(UserCreationDtoSwaggerExample))]
        public async Task<IActionResult> RegisterNewUser([FromBody] UserCreationDto request)
        {
            logger.LogTrace($"Entering method {nameof(UserController) + "." + nameof(RegisterNewUser)}");

            try
            {
                var successful = await guardApi.RegisterNewUser(
                    request.Email,
                    request.HashedPassword,
                    request.Salt,
                    request.UserId,
                    request.PublicKey,
                    request.EncryptedPrivateKey);

                if (!successful)
                    return BadRequest(new MessageDto("Guard api operation was not successful"));
            }
            catch (InvalidStatusCodeException e)
            {
                logger.LogError("Received an invalid status code", e);
                return BadRequest(e.ServerMessage);
            }
            catch (Exception e)
            {
                logger.LogError("Unexpected exception occured during the call of the guard api", e);
                return new ObjectResult(new MessageDto("Unexpected exception occured during the call of the guard api"))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            logger.LogInformation("User created successfully");
            return new ObjectResult(new MessageDto("User created")) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserInformation([FromRoute] string email)
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
