using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.Queueing.Models.RabbitMq;
using Xunit;

namespace LessPaper.Shared.Queueing.UnitTest.RabbitMq
{
    public class RabbitMqBuilderTest
    {
        [Fact]
        public void  SubscribeSpecific()
        {
            var settingsMock = RabbitMqSettingsTest.GetSettingsMock();
            var senderBuilder = new RabbitMqBuilder(settingsMock.Object);

            senderBuilder.SubscribeSpecific("QueueName", async (DummyClass x) => { });
            senderBuilder.SubscribeSpecific("QueueName", async (DummyClass1 x) => { });

            //TODO Implement good tests
        }


        [Fact]
        public void Subscribe()
        {
            var settingsMock = RabbitMqSettingsTest.GetSettingsMock();
            var senderBuilder = new RabbitMqBuilder(settingsMock.Object);

            senderBuilder.Subscribe(async (DummyClass x) => { });
            senderBuilder.Subscribe(async (DummyClass1 x) => { });

            //TODO Implement good tests
        }
    }
}
