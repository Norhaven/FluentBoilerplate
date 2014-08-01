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
using NUnit.Framework;
using FluentBoilerplate.Runtime;
using Moq;
using FluentAssertions;
using FluentBoilerplate.Messages.Developer;
using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Providers;

namespace FluentBoilerplate.Tests.Runtime.ExceptionHandlerTests
{
    [TestFixture]
    public class Handle
    {
        [Test]
        public void ExceptionIsHandled()
        {
            var log = new Mock<ILogProvider>(MockBehavior.Strict).AllowEveryError();
            var handled = false;
            Action<Exception> handler = ex => { handled = true; };
            var wrapper = new ExceptionHandler<Exception>(log.Object, handler);
            var exception = new Exception();
            wrapper.Handle(exception);

            handled.Should().BeTrue("because the handler should handle the exception");
            log.Verify();
        }

        [Test]
        public void ExceptionIsLogged()
        {
            var exception = new Exception("Error message");
            var sectionName = "Test Section";
            var message = LogErrors.ActionResultedInException.WithValues(exception.GetType(), sectionName);
            var log = new Mock<ILogProvider>(MockBehavior.Strict).AllowErrorsWithMessage(message);            
            Action<Exception> handler = ex => { };
            var wrapper = new ExceptionHandler<Exception>(log.Object, handler);
            
            wrapper.Handle(exception);

            log.Verify();
        }
    }
}
