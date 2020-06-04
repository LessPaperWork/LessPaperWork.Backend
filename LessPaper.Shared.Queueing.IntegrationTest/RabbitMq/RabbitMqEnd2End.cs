using System;
using System.Threading;
using LessPaper.Shared.Queueing.Models.RabbitMq;
using Xunit;

namespace LessPaper.Shared.Queueing.IntegrationTest.RabbitMq
{
    public class RabbitMqEnd2End
    {
        [Fact]
        public async void SendReceiveSingle()
        {
            var rabbitMqSettings = new RabbitMqSettings(
                "localhost",
                "guest",
                "guest",
                "LessPaper.Shared.Queueing.IntegrationTest.1");

            var successfulCalled = new AutoResetEvent(false);

            var exampleText = "Some data";

            var builder = new RabbitMqBuilder(rabbitMqSettings);
            var finalized = await builder
                .Subscribe(async delegate (DummyClass dto)
                {
                    if (dto.DummyProperty == exampleText)
                        successfulCalled.Set();
                })
                .Start();


            var myObj = new DummyClass()
            {
                DummyProperty = exampleText
            };

            await finalized.Send(myObj);

            // Wait for receiving
            Assert.True(successfulCalled.WaitOne(2000));

            await finalized.Stop();
        }


        [Fact]
        public async void SendReceive_ThrowRetry()
        {
            var rabbitMqSettings = new RabbitMqSettings(
                "localhost",
                "guest",
                "guest",
                "LessPaper.Shared.Queueing.IntegrationTest.2");

            var counter = 0;
            var lockObj = new object();
            var successfulCalled = new AutoResetEvent(false);
            
            var exampleText = "Some data";

            var builder = new RabbitMqBuilder(rabbitMqSettings);
            var finalized = await builder
                .Subscribe(async delegate (DummyClass dto)
                {
                    if (dto.DummyProperty == exampleText)
                    {
                        lock (lockObj)
                        {
                            counter += 1;
                            switch (counter)
                            {
                                case 2:
                                    successfulCalled.Set();
                                    break;
                                case 1:
                                    throw new Exception("Test exception");
                            }
                        }
                    }
                    else
                    {
                        Assert.True(false, "Wrong data received");
                    }

                })
                .Start();


            var myObj = new DummyClass()
            {
                DummyProperty = exampleText
            };

            await finalized.Send(myObj);

            // Wait for receiving
            Assert.True(successfulCalled.WaitOne(TimeSpan.FromSeconds(5)));
     

            await finalized.Stop();
        }
    }
}
