using System;
using System.Threading.Tasks;
using LessPaper.APIGateway.Controllers.v1;
using LessPaper.APIGateway.Helper;
using LessPaper.APIGateway.Models;
using LessPaper.APIGateway.Models.Request;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.Interfaces.GuardApi.Response;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LessPaper.APIGateway.UnitTest.Controller
{
    public class UserControllerTest : BaseController
    {
        [Fact]
        public async Task Register_NotSuccessful()
        {
            var guardApiMock = new Mock<IGuardApi>();
            guardApiMock.Setup(mock =>
                    mock.RegisterNewUser(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>())
                )
                .ReturnsAsync(false);

            var controller = new UserController(AppSettings, guardApiMock.Object);

            //Invalid email
            var requestObj = await controller.Register(new UserRegistrationRequest
            {
                Email = "a@b.de",
                Password = "my_secure_password"
            });
            Assert.IsType<BadRequestResult>(requestObj);
        }


        [Fact]
        public async Task Register_Ok()
        {
            var guardApiMock = new Mock<IGuardApi>();
            guardApiMock.Setup(mock =>
                    mock.RegisterNewUser(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>())
                )
                .ReturnsAsync(true);

            var controller = new UserController(AppSettings, guardApiMock.Object);

            //Invalid email
            var badEmailRequestObj = await controller.Register(new UserRegistrationRequest
            {
                Email = "a@",
                Password = "my_secure_password"
            });
            Assert.IsType<BadRequestResult>(badEmailRequestObj);


            //Invalid password
            var badPasswordRequestObj = await controller.Register(new UserRegistrationRequest
            {
                Email = "a@b.de",
                Password = "1"
            });
            Assert.IsType<BadRequestResult>(badPasswordRequestObj);


            //Valid request
            var validRequestObj = await controller.Register(new UserRegistrationRequest
            {
                Email = "a@b.de",
                Password = "my_secure_password"
            });
            Assert.IsType<OkResult>(validRequestObj);
        }

        [Fact]
        public async Task Register_Throw()
        {
            var guardApiMock = new Mock<IGuardApi>();
            guardApiMock.Setup(mock =>
                    mock.RegisterNewUser(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>())
                ).Throws<InvalidOperationException>();

            var controller = new UserController(AppSettings, guardApiMock.Object);

            //Invalid email
            var requestObj = await controller.Register(new UserRegistrationRequest
            {
                Email = "a@b.de",
                Password = "my_secure_password"
            });
            Assert.IsType<BadRequestResult>(requestObj);
        }


        [Fact]
        public async Task RefreshToken()
        {
            var writeApiMock = new Mock<IGuardApi>();
            writeApiMock.Setup(mock =>
                    mock.RegisterNewUser(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>())
                )
                .ReturnsAsync(true);

            var controller = new UserController(AppSettings, writeApiMock.Object);

            var refreshedToken = await controller.RefreshToken(new AuthToken()
            {
                RefreshToken = "RefreshToken",
                Token = "Token"
            });

            Assert.IsType<OkObjectResult>(refreshedToken);
            //Todo implement refresh token test
        }

        [Fact]
        public async Task UserInfo()
        {
            var writeApiMock = new Mock<IGuardApi>();
            writeApiMock.Setup(mock =>
                    mock.RegisterNewUser(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>())
                )
                .ReturnsAsync(true);

            var controller = new UserController(AppSettings, writeApiMock.Object);

            var userInfo = await controller.UserInfo();

            Assert.IsType<OkResult>(userInfo);
            //Todo implement user info test
        }


        [Fact]
        public async Task Login_Ok()
        {
            var passwordHash = CryptoHelper.Sha256FromString("my_secure_password", "salt");

            var credentialsMock = new Mock<ICredentialsResponse>();
            credentialsMock.SetupGet(x => x.PasswordHash).Returns(passwordHash);
            credentialsMock.SetupGet(x => x.Salt).Returns("salt");


            var guardApiMock = new Mock<IGuardApi>();
            guardApiMock.Setup(mock =>
                    mock.GetUserCredentials(
                        It.IsAny<string>())
                )
                .ReturnsAsync(credentialsMock.Object);

            var controller = new UserController(AppSettings, guardApiMock.Object);

            //Invalid email
            var validEmailRequestObj = await controller.Login(new UserLoginRequest
            {
                Email = "a@b.de",
                Password = "my_secure_password"
            });

            Assert.IsType<OkResult>(validEmailRequestObj);
            //Todo implement login test
        }


        [Fact]
        public async Task Login_Wrong_Credentials()
        {
            var credentialsMock = new Mock<ICredentialsResponse>();
            credentialsMock.SetupGet(x => x.PasswordHash).Returns("wrong hash");
            credentialsMock.SetupGet(x => x.Salt).Returns("salt");
            
            var guardApiMock = new Mock<IGuardApi>();
            guardApiMock.Setup(mock =>
                    mock.GetUserCredentials(
                        It.IsAny<string>())
                )
                .ReturnsAsync(credentialsMock.Object);

            var controller = new UserController(AppSettings, guardApiMock.Object);

            //Invalid email
            var validEmailRequestObj = await controller.Login(new UserLoginRequest
            {
                Email = "a@b.de",
                Password = "my_secure_password"
            });

            Assert.IsType<BadRequestResult>(validEmailRequestObj);
        }


        [Fact]
        public async Task Login_Throw()
        {
            var guardApiMock = new Mock<IGuardApi>();
            guardApiMock.Setup(mock =>
                    mock.GetUserCredentials(
                        It.IsAny<string>())
                )
                .Throws<InvalidOperationException>();

            var controller = new UserController(AppSettings, guardApiMock.Object);

            //Invalid email
            var validEmailRequestObj = await controller.Login(new UserLoginRequest
            {
                Email = "a@b.de",
                Password = "my_secure_password"
            });

            Assert.IsType<BadRequestResult>(validEmailRequestObj);
        }
    }
}
