using System;
using System.Threading;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.Queuing;
using LessPaper.Shared.Queueing.Models.RabbitMq;
using MassTransit;
using Moq;
using Xunit;

namespace LessPaper.Shared.Queueing.UnitTest.RabbitMq
{
    public class RabbitMqSenderTest
    {
   


        [Fact]
        public async void SentToSpecific()
        {
            var settingsMock = RabbitMqSettingsTest.GetSettingsMock();

            var queueBuilderMock = new Mock<IQueueBuilder>();
            var busControllerMock = new Mock<IBusControl>();
            var sendEndpointMock = new Mock<ISendEndpoint>();
        
            sendEndpointMock
                .Setup(x => x.Send(It.IsAny<DummyClass>(), It.IsAny<CancellationToken>()))
                .Returns(async () => {});
            
            busControllerMock
                .Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
                .Returns(Task.FromResult(sendEndpointMock.Object));
            
            var sender = new RabbitMqSender(busControllerMock.Object, queueBuilderMock.Object, settingsMock.Object);
            await sender.SendToSpecific("MyQueue", new DummyClass());

            // Verify that the MassTransit calls are executed
            busControllerMock.Verify(mock => mock.GetSendEndpoint(It.IsAny<Uri>()), Times.Once());
            sendEndpointMock.Verify(mock => mock.Send(It.IsAny<DummyClass>(), It.IsAny<CancellationToken>()), Times.Once());

            // Verify that the send endpoint is fresh
            await sender.SendToSpecific("MyQueue", new DummyClass());
            busControllerMock.Verify(mock => mock.GetSendEndpoint(It.IsAny<Uri>()), Times.Exactly(2));
            sendEndpointMock.Verify(mock => mock.Send(It.IsAny<DummyClass>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async void Sent()
        {
            var settingsMock = RabbitMqSettingsTest.GetSettingsMock();
            var queueBuilderMock = new Mock<IQueueBuilder>();
            var busControllerMock = new Mock<IBusControl>();
            var sendEndpointMock = new Mock<ISendEndpoint>();
            
            busControllerMock
                .Setup(x => x.Publish(It.IsAny<DummyClass>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(sendEndpointMock.Object));

            var sender = new RabbitMqSender(busControllerMock.Object, queueBuilderMock.Object, settingsMock.Object);
            await sender.Send(new DummyClass());

            // Verify that the MassTransit calls are executed
            busControllerMock.Verify(x => x.Publish(It.IsAny<DummyClass>(), It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async void Stop()
        {
            var settingsMock = RabbitMqSettingsTest.GetSettingsMock();
            var queueBuilderMock = new Mock<IQueueBuilder>();
            var busControllerMock = new Mock<IBusControl>();
            var sendEndpointMock = new Mock<ISendEndpoint>();

            busControllerMock
                .Setup(x => x.StopAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(sendEndpointMock.Object));

            var sender = new RabbitMqSender(busControllerMock.Object, queueBuilderMock.Object, settingsMock.Object);
            await sender.Stop();

            // Verify that the MassTransit calls are executed
            busControllerMock.Verify(x => x.StopAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
