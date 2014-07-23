using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Messages.Developer;
using FluentBoilerplate.Runtime.Extensions;
using Moq;
using FluentBoilerplate.Runtime;
using FluentAssertions;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Providers;

namespace FluentBoilerplate.Tests.Runtime.ExceptionHandlerTests
{
    [TestClass]
    public class ImplicitToLoweredExceptionHandler
    {
        [TestMethod]
        public void BaseExceptionDoesNotChangeHandlingBehavior()
        {
            var exception = new ArgumentException("Error message");
            var sectionName = "Test Section";
            var message = LogErrors.ActionResultedInException.WithValues(exception.GetType(), sectionName);
            var log = new Mock<ILogProvider>(MockBehavior.Strict).AllowErrorsWithMessage(message);
            var handled = false;
            Action<Exception> handler = ex => { handled = true; };

            var wrapper = new ExceptionHandler<ArgumentException>(log.Object, sectionName, handler);

            ExceptionHandler<Exception> lowered = wrapper;

            lowered.Handle(exception);

            handled.Should().BeTrue("because the exception handler should have handled it");
            log.Verify();
        }
    }
}
