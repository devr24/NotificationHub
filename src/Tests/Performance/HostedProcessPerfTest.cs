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

namespace CloudCore.NotificationHub.Tests.Performance
{
    [IsPerformance]
    public class HostedProcessPerfTest : IDisposable
    {
        public HostedProcessPerfTest()
        {
            // Tear up...
        }

        /// <summary>Describe performance test purpose.</summary>
        [Fact, LogExecutionTime]
        public async Task Performance_HostedProcess_TestName()
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
