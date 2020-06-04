using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.Queueing.Interfaces.RabbitMq;
using LessPaper.Shared.Queueing.Models.RabbitMq;
using MassTransit;
using Moq;
using Xunit;

namespace LessPaper.Shared.Queueing.UnitTest.RabbitMq
{
    public class MassTransitConsumerProxyTest
    {
        [Fact]
        public async void Consume()
        {
            var called = false;
            var proxy = new MassTransitConsumerProxy<DummyClass>(async delegate (DummyClass data)
                {
                    called = true;
                });

            var settingsMock = new Mock<ConsumeContext<DummyClass>>();
            await proxy.Consume(settingsMock.Object);
            
            Assert.True(called);
        }

    }
}
