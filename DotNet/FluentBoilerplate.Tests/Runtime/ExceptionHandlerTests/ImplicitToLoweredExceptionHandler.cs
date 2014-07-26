/*
   Copyright 2014 Chris Hannon

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

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

            var wrapper = new ExceptionHandler<ArgumentException>(log.Object, handler);

            ExceptionHandler<Exception> lowered = wrapper;

            lowered.Handle(exception);

            handled.Should().BeTrue("because the exception handler should have handled it");
            log.Verify();
        }
    }
}
