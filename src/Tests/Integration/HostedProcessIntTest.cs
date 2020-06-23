using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Cloud.Core.Testing;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CloudCore.NotificationHub.Tests.Integration
{
    [IsIntegration]
    public class HostedProcessIntTest : IDisposable
    {
        public HostedProcessIntTest()
        {
            // Tear up...
        }

        /// <summary>Describe integration test purpose.</summary>
        [Fact]
        public async Task Integration_HostedProcess_TestName()
        {
            // Arrange

            // Act

            // Assert
        }

        public void Dispose()
        {
            // Tear down...
        }
    }
}
