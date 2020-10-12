using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Rest.Models.Dtos;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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


        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterNewUser([FromBody] UserCreationDto request)
        {
            var successful = await guardApi.RegisterNewUser(
                request.Email, 
                request.HashedPassword, 
                request.Salt, 
                request.UserId,
                request.PublicKey, 
                request.EncryptedPrivateKey);

            if (!successful)
                return BadRequest();

            return Ok();
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
