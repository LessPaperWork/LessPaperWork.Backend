using System.Threading.Tasks;
using LessPaper.GuardService.Models.Api;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.Database.Manager;
using LessPaper.Shared.Rest.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<IActionResult> RegisterNewUser([FromBody] UserCreationDto request)
        {
            if (!IdGenerator.IsType(request.UserId, IdType.User) ||
                !ValidationHelper.IsValidEmailAddress(request.Email) ||
                string.IsNullOrWhiteSpace(request.HashedPassword) ||
                string.IsNullOrWhiteSpace(request.Salt) ||
                string.IsNullOrWhiteSpace(request.EncryptedPrivateKey) ||
                string.IsNullOrWhiteSpace(request.PublicKey))
            {
                return BadRequest();
            }

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

            var userInformation = await userManager.GetUserInformation(email);
            if (userInformation == null)
                return BadRequest();

            return new OkObjectResult(new ExtendedUserInformationDto(userInformation));
        }

    }
}
