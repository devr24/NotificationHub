using System;
using Xunit;
using Cloud.Core;
using Cloud.Core.Testing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CloudCore.NotificationHub.Tests.Unit
{
    [IsUnit]
    public class HostedProcessUnitTest : IDisposable
    {
        public HostedProcessUnitTest()
        {
            // Tear up...
        }

        /// <summary>Sample unit test for HostedProcess.</summary>
        [Fact]
        public void Test_HostedProcess_StartStop()
        {
            //// Arrange - setup classes.
            //var loggerMock = new Mock<ILogger<EmailProcessor>>();
            //var telemLoggerMock = new Mock<ITelemetryLogger>();
            //var messengerMock = new Mock<IReactiveMessenger>();

            //// Act - create class passing dependencies.
            //var process = new EmailProcessor(loggerMock.Object, telemLoggerMock.Object, messengerMock.Object);

            //// Asset - all properties should be as expected.
            //process.Logger.Should().Be(loggerMock.Object);
            //process.TelemetryLogger.Should().Be(telemLoggerMock.Object);
            //process.Messenger.Should().Be(messengerMock.Object);
        }

        public void Dispose()
        {
            // Tear down...
        }
    }
}
