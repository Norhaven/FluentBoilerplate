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

using FluentBoilerplate.Runtime.Contexts;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Runtime.Providers.Logging;
using System;
using TechTalk.SpecFlow;
using FluentAssertions;

namespace FluentBoilerplate.Tests.Runtime.Contexts.ImmutableErrorContextTests
{
    [Binding]
    public class RunWithErrorHandlingSteps
    {
        private readonly TestContext testContext;

        public RunWithErrorHandlingSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [Given(@"I have created an error context")]
        public void GivenIHaveCreatedAnErrorContext()
        {
            this.testContext.Errors = new ImmutableErrorContext(LogProvider.Default, TryCatchBlockProvider.Empty, ExceptionHandlerProvider.Empty);
        }

        [Given(@"I have not added any exception handlers")]
        public void GivenIHaveNotAddedAnyExceptionHandlers()
        {
            this.testContext.Errors.HasHandlers.Should().BeFalse("because we need an empty exception handler");
        }

        [Given(@"I have added an exception handler for ArgumentException")]
        public void GivenIHaveAddedAnExceptionHandlerForArgumentException()
        {
            this.testContext.Errors = this.testContext.Errors.RegisterExceptionHandler<ArgumentException>(ex => { });
        }

        [Given(@"I perform an action that throws ArgumentException")]
        public void GivenIPerformAnActionThatThrowsArgumentException()
        {
            this.testContext.CustomAction = () => { throw new ArgumentException(); };
        }

        [Given(@"I perform an action that throws IndexOutOfRangeException")]
        public void GivenIPerformAnActionThatThrowsIndexOutOfRangeException()
        {
            this.testContext.CustomAction = () => { throw new IndexOutOfRangeException(); };
        }
        [Given(@"I perform an action that does not throw exceptions")]
        public void GivenIPerformAnActionThatDoesNotThrowExceptions()
        {
            this.testContext.CustomAction = () => { };
        }

        [When(@"I perform the action through the error context")]
        public void WhenIPerformTheActionThroughTheErrorContext()
        {
            this.testContext.CustomAction.Should().NotBeNull("because we are trying to perform an action");

            try
            {
                this.testContext.Errors.DoInContext(_ => this.testContext.CustomAction());
            }
            catch(Exception ex)
            {
                this.testContext.UnhandledException = ex;
            }
        }

        [Then(@"I should perform the action successfully")]
        public void ThenIShouldPerformTheActionSuccessfully()
        {
            this.testContext.UnhandledException.Should().BeNull("because no exception should have been unhandled");
        }

        [Then(@"the exception should have been handled")]
        public void ThenTheExceptionShouldHaveBeenHandled()
        {
            this.testContext.UnhandledException.Should().BeNull("because no exception should have been unhandled");
        }

        [Then(@"the exception should not have been handled")]
        public void ThenTheExceptionShouldNotHaveBeenHandled()
        {
            this.testContext.UnhandledException.Should().NotBeNull("because an unhandled exception should have been thrown");
        }
    }
}
