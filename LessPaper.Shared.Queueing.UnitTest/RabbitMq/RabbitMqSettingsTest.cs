using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.Queueing.Interfaces.RabbitMq;
using LessPaper.Shared.Queueing.Models.RabbitMq;
using Moq;
using Xunit;

namespace LessPaper.Shared.Queueing.UnitTest.RabbitMq
{
    public class RabbitMqSettingsTest
    {
       

        [Fact]
        public void PropertyGetterSetter()
        {
            var settings = new RabbitMqSettings()
            {
                Host = "localhost",
                Username = "user",
                Password = "pw",
                ServerIdentity = "UniqueServerId"
            };

            Assert.Equal("localhost", settings.Host);
            Assert.Equal("user", settings.Username);
            Assert.Equal("pw", settings.Password);
            Assert.Equal("UniqueServerId", settings.ServerIdentity);
        }

        public static Mock<IRabbitMqSettings> GetSettingsMock()
        {
            var settingsMock = new Mock<IRabbitMqSettings>();
            settingsMock.SetupGet(settings1 => settings1.Host).Returns("localhost");
            settingsMock.SetupGet(settings1 => settings1.Username).Returns("admin");
            settingsMock.SetupGet(settings1 => settings1.Password).Returns("masterkey");
            settingsMock.SetupGet(settings1 => settings1.ServerIdentity).Returns("MyUniqueServer");

            return settingsMock;
        }
    }
}
