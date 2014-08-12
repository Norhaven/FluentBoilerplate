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

using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Runtime.Providers.Logging;
using System;
using TechTalk.SpecFlow;

namespace FluentBoilerplate.Tests.Runtime.ExceptionHandlerTests
{
    [Binding]
    public class ExceptionHandlersAreStoredTogetherSteps
    {
        private readonly TestContext testContext;

        public ExceptionHandlersAreStoredTogetherSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [Given(@"I have created an exception handler for ArgumentException")]
        public void GivenIHaveCreatedAnExceptionHandlerForArgumentException()
        {
            var handler = new ExceptionHandler<ArgumentException>(LogProvider.Default, _ => { });
            this.testContext.NonSpecificExceptionHandler = (ExceptionHandler<Exception>)handler;
            this.testContext.SpecificExceptionHandler = handler;
        }

        [Given(@"I have made the exception handler specific")]
        public void GivenIHaveMadeTheExceptionHandlerSpecific()
        {
            this.testContext.NonSpecificExceptionHandler = null;
        }

        [Given(@"I have made the exception handler non-specific")]
        public void GivenIHaveMadeTheExceptionHandlerNon_Specific()
        {
            this.testContext.SpecificExceptionHandler = null;
        }

        [When(@"an ArgumentException reaches the exception handler")]
        public void WhenAnArgumentExceptionReachesTheExceptionHandler()
        {
            if (this.testContext.SpecificExceptionHandler != null)
                this.testContext.SpecificExceptionHandler.Handle(new ArgumentException());
            else
                this.testContext.NonSpecificExceptionHandler.Handle(new ArgumentException());
        }
    }
}
