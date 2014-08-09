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

using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Contexts;
using System;
using TechTalk.SpecFlow;
using FluentAssertions;

namespace FluentBoilerplate.Tests.Runtime.Contexts.InitialBoilerplateContextTests
{
    [Binding]
    public class RunWithinContextSteps
    {
        private readonly TestContext testContext;

        public RunWithinContextSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [When(@"I have not added a contract")]
        public void GivenIHaveNotAddedAContract()
        {
            //TODO: Think of a way to ensure this is the case without polluting the public API
        }
        
        [When(@"I perform the action through the boilerplate context")]
        public void WhenIPerformTheActionThroughTheBoilerplateContext()
        {
            if (this.testContext.UnhandledException != null)
                return;

            try
            {
                this.testContext.Boilerplate.Do(context => this.testContext.CustomAction());
            }
            catch (Exception ex)
            {
                this.testContext.UnhandledException = ex;
            }
        }

        [When(@"I have added a contract that has requirements")]
        public void GivenIHaveAddedAContractThatHasRequirements()
        {
            this.testContext.Boilerplate =
                this.testContext.Boilerplate
                    .BeginContract()
                        .Require(() => this.testContext.ContractRequirementsShouldPass)
                    .EndContract();                                 
        }

        [When(@"all contract requirements pass")]
        public void WhenAllContractRequirementsPass()
        {
            this.testContext.ContractRequirementsShouldPass = true;
        }

        [When(@"a contract requirement fails")]
        public void WhenAContractRequirementFails()
        {
            this.testContext.ContractRequirementsShouldPass = false;
        }

        [Then(@"a ContractViolationException should be thrown")]
        public void ThenAContractViolationExceptionShouldBeThrown()
        {
            this.testContext.UnhandledException.Should().NotBeNull("because an unhandled exception is expected");
            this.testContext.UnhandledException.Should().BeOfType<ContractViolationException>("because that is the exception we're expecting");
        }

        [When(@"I have added a contract that handles ArgumentException")]
        public void WhenIHaveAddedAContractThatHandlesArgumentException()
        {
            this.testContext.Boilerplate =
                this.testContext.Boilerplate
                    .BeginContract()
                        .Handles<ArgumentException>(ex => { })
                    .EndContract();
        }

        [When(@"I have added a contract that handles ArgumentException and then Exception")]
        public void WhenIHaveAddedAContractThatHandlesArgumentExceptionAndThenException()
        {
            this.testContext.Boilerplate =
                this.testContext.Boilerplate
                    .BeginContract()
                        .Handles<ArgumentException>(ex => { })
                    .EndContract();
        }

        [Then(@"the ArgumentException exception handler should handle the exception")]
        public void ThenTheArgumentExceptionExceptionHandlerShouldHandleTheException()
        {
            this.testContext.ArgumentExceptionHandlerShouldBeRun = true;
        }

        [When(@"I have added a contract that handles Exception and then ArgumentException")]
        public void WhenIHaveAddedAContractThatHandlesExceptionAndThenArgumentException()
        {
            this.testContext.Boilerplate =
                this.testContext.Boilerplate
                    .BeginContract()
                        .Handles<Exception>(ex => this.testContext.ExceptionHandlerShouldBeRun.FailTestIfFalse())
                        .Handles<ArgumentException>(ex => this.testContext.ArgumentExceptionHandlerShouldBeRun.FailTestIfFalse())
                    .EndContract();
        }

        [Then(@"the Exception exception handler should handle the exception")]
        public void ThenTheExceptionExceptionHandlerShouldHandleTheException()
        {
            this.testContext.ExceptionHandlerShouldBeRun = true;
        }

        [When(@"I have added a contract that fails validation")]
        public void WhenIHaveAddedAContractThatFailsValidation()
        {
            this.testContext.Boilerplate =
                this.testContext.Boilerplate
                    .BeginContract()
                        .RequireValidInstanceOf<ValidateableType>(new ValidateableType { NotNullText = null })
                    .EndContract();
        }

        [When(@"I have added a contract that passes validation")]
        public void WhenIHaveAddedAContractThatPassesValidation()
        {
            this.testContext.Boilerplate =
                this.testContext.Boilerplate
                    .BeginContract()
                        .RequireValidInstanceOf<ValidateableType>(new ValidateableType { NotNullText = String.Empty })
                    .EndContract();
        }

    }
}
