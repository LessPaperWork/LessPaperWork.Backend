using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LessPaper.APIGateway.Helper;
using LessPaper.APIGateway.Models;
using LessPaper.APIGateway.Models.Request;
using LessPaper.APIGateway.Options;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.GuardApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LessPaper.APIGateway.Controllers.v1
{
    [Route("v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IOptions<AppSettings> config;
        private readonly IGuardApi guardApi;

        public UserController(IOptions<AppSettings> config, IGuardApi guardApi)
        {
            this.config = config;
            this.guardApi = guardApi;
        }

        private bool IsValidEmailAndPassword(string email, string password)
        {
            return !string.IsNullOrEmpty(password) && 
                   !string.IsNullOrEmpty(email) && 
                   password.Length >= config.Value.ValidationRules.MinimumPasswordLength && 
                   ValidationHelper.IsValidEmailAddress(email);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest registrationRequest)
        {
            // Validate user input
            if (!IsValidEmailAndPassword(registrationRequest.Email, registrationRequest.Password))
                return BadRequest();

            // Generate user entry
            var emailAddress = registrationRequest.Email;
            var salt = CryptoHelper.GetSalt(10);
            var hashedPassword = CryptoHelper.Sha256FromString(registrationRequest.Password, salt);

            //TODO Add user type
            var userId = IdGenerator.NewId(IdType.Undefined);

            // Call api to register a new user
            try
            {
                var registrationSuccessful = await guardApi.RegisterNewUser(emailAddress, hashedPassword, salt, userId);
                if (!registrationSuccessful)
                    return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            // Validate user input
            if (!IsValidEmailAndPassword(loginRequest.Email, loginRequest.Password))
                return BadRequest();

            try
            {
                // Receive user data
                var userData = await guardApi.GetUserCredentials(loginRequest.Email);
                if (userData == null)
                    return BadRequest();

                // Recalculate the password and compare with given password hash
                var hashedPassword = CryptoHelper.Sha256FromString(loginRequest.Password, userData.Salt);
                if (hashedPassword != userData.PasswordHash)
                    return BadRequest();

                // Todo generate token

                return Ok();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] AuthToken oldAuthToken)
        {
            var newAuthToken = new AuthToken();


            // Todo implement token refresh
            await Task.Delay(1);

            return Ok(newAuthToken);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UserInfo()
        {
            //Todo implement request for user information
            await Task.Delay(1);
            return Ok();
        }
    }
}
