using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Runtime;
using Moq;
using FluentAssertions;
using FluentBoilerplate.Messages.Developer;
using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Providers;

namespace FluentBoilerplate.Tests.Runtime.ExceptionHandlerTests
{
    [TestClass]
    public class Handle
    {
        [TestMethod]
        public void ExceptionIsHandled()
        {
            var log = new Mock<ILogProvider>(MockBehavior.Strict).AllowEveryError();
            var sectionName = "Test Section";
            var handled = false;
            Action<Exception> handler = ex => { handled = true; };
            var wrapper = new ExceptionHandler<Exception>(log.Object, sectionName, handler);
            var exception = new Exception();
            wrapper.Handle(exception);

            handled.Should().BeTrue("because the handler should handle the exception");
            log.Verify();
        }

        [TestMethod]
        public void ExceptionIsLogged()
        {
            var exception = new Exception("Error message");
            var sectionName = "Test Section";
            var message = LogErrors.ActionResultedInException.WithValues(exception.GetType(), sectionName);
            var log = new Mock<ILogProvider>(MockBehavior.Strict).AllowErrorsWithMessage(message);            
            Action<Exception> handler = ex => { };
            var wrapper = new ExceptionHandler<Exception>(log.Object, sectionName, handler);
            
            wrapper.Handle(exception);

            log.Verify();
        }
    }
}
