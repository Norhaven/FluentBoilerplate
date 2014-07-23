using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    internal static class MockILogExtensions
    {
        public static Mock<ILogProvider> AllowEveryError(this Mock<ILogProvider> mock)
        {
            mock.Setup(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()));
            return mock;
        }

        public static Mock<ILogProvider> AllowErrorsWithMessage(this Mock<ILogProvider> mock, string message)
        {
            mock.Setup(x => x.Error(It.Is<string>(p => p == message), It.IsAny<Exception>()));
            return mock;
        }
    }
}
