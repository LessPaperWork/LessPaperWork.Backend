using System.ComponentModel;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.Database.Manager;
using LessPaper.Shared.Rest.Models.Dtos;
using LessPaper.Shared.Rest.Models.DtoSwaggerExamples;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.GuardService.Controllers.v1
{
    [Route("v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IDbUserManager userManager;

        public UserController(
            ILogger<UserController> logger,
            IDbUserManager userManager,
            IDbDirectoryManager directoryManager,
            IDbFileManager fileManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }


        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(UserCreationDto), typeof(UserCreationDtoSwaggerExample))]
        public async Task<IActionResult> RegisterNewUser([FromBody] UserCreationDto request)
        {
            logger.LogTrace($"Entering method {nameof(UserController) + "." + nameof(RegisterNewUser)}");
            
            if (!IdGenerator.IsType(request.UserId, IdType.User))
                return BadRequest(new MessageDto("User id is not of type user"));
            if (!ValidationHelper.IsValidEmailAddress(request.Email))
                return BadRequest(new MessageDto("Email address is not valid"));
            if (string.IsNullOrWhiteSpace(request.HashedPassword))
                return BadRequest(new MessageDto("Hashed password is null or whitespace"));
            if (string.IsNullOrWhiteSpace(request.Salt))
                return BadRequest(new MessageDto("Salt is null or whitespace"));
            if (string.IsNullOrWhiteSpace(request.EncryptedPrivateKey))
                return BadRequest(new MessageDto("EncryptedPrivateKey is null or whitespace"));
            if (string.IsNullOrWhiteSpace(request.PublicKey))
                return BadRequest(new MessageDto("PublicKey is null or whitespace"));
            
            var rootDirectoryId = IdGenerator.NewId(IdType.Directory);

            var successful = await userManager.InsertUser(
                request.UserId,
                rootDirectoryId,
                request.Email,
                request.HashedPassword,
                request.Salt,
                request.PublicKey,
                request.EncryptedPrivateKey);

            if (!successful)
                return BadRequest(new MessageDto("Database insertion failed"));

            return Ok(new MessageDto("User created"));
        }

        [HttpGet("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserInformation([FromRoute] string email)
        {
            logger.LogTrace($"Entering method {nameof(UserController) + "." + nameof(GetUserInformation)}");

            if (!ValidationHelper.IsValidEmailAddress(email))
                return BadRequest();

            var userInformation = await userManager.GetUserInformation(email);
            if (userInformation == null)
                return BadRequest();

            return new OkObjectResult(new ExtendedUserInformationDto(userInformation));
        }

    }
}
