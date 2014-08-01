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
using TechTalk.SpecFlow;
using FluentAssertions;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime.Contexts;

namespace FluentBoilerplate.Tests.PublicContract.BoilerplateTests
{
    [Binding]
    public class BoilerplateCreationSteps
    {
        private readonly TestContext testContext;

        public BoilerplateCreationSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        private static bool TryUseType<T>(IContext boilerplate, Action useType)
        {
            try
            {
                boilerplate.Open<T>().AndDo((context, value) => { useType(); });
            }
            catch (OperationWasNotSuccessfulException)
            {
                return true;
            }
            return false;
        }

        [Given(@"I have asked for a new boilerplate")]
        public void GivenIHaveAskedForANewBoilerplate()
        {
            this.testContext.Boilerplate = null;
        }

        [Given(@"I have no identity")]
        public void GivenIHaveNoIdentity()
        {
            this.testContext.Identity = null;
        }

        [When(@"I create the boilerplate")]
        public void WhenICreateTheBoilerplate()
        {
            this.testContext.Boilerplate = Boilerplate.New(this.testContext.Identity, this.testContext.Access);
        }
        
        [Then(@"I should receive an instance of a boilerplate context with the default identity and no additional type access")]
        public void ThenIShouldReceiveAnInstanceOfABoilerplateContextWithTheDefaultIdentityAndNoAdditionalTypeAccess()
        {
            this.testContext.Boilerplate.Identity.Should().NotBeNull("because an identity should be present");
            this.testContext.Boilerplate.Identity.ShouldBeEquivalentTo(Identity.Default, "because we did not specify an identity");

            var caughtException = TryUseType<int>(this.testContext.Boilerplate, () => { throw new ArgumentException("Should not have tried to use this type"); });

            caughtException.Should().BeTrue("because not having a type access provider will fail the call to Open()");
        }
        
        [Then(@"I should receive an instance of a boilerplate context with the custom identity and no additional type access")]
        public void ThenIShouldReceiveAnInstanceOfABoilerplateContextWithTheCustomIdentityAndNoAdditionalTypeAccess()
        {
            this.testContext.Boilerplate.Identity.Equals(Identity.Default).Should().BeFalse("because a custom identity was used");

            var caughtException = TryUseType<int>(this.testContext.Boilerplate, () => { throw new ArgumentException("Should not have tried to use this type"); });

            caughtException.Should().BeTrue("because the default type access provider was used");
        }

        [Then(@"I should receive an instance of a boilerplate context with the default identity and the custom type access")]
        public void ThenIShouldReceiveAnInstanceOfABoilerplateContextWithTheDefaultIdentityAndTheCustomTypeAccess()
        {
            this.testContext.Boilerplate.Identity.ShouldBeEquivalentTo(Identity.Default, "because we did not specify an identity");

            var typeUsed = false;
            var caughtException = TryUseType<int>(this.testContext.Boilerplate, () => { typeUsed = true; });

            caughtException.Should().BeFalse("because the custom type access provider was used");
            typeUsed.Should().BeTrue("because the type was useable");
        }

        [Then(@"I should receive an instance of a boilerplate context with the custom identity and the custom type access")]
        public void ThenIShouldReceiveAnInstanceOfABoilerplateContextWithTheCustomIdentityAndTheCustomTypeAccess()
        {
            this.testContext.Boilerplate.Identity.Equals(Identity.Default).Should().BeFalse("because a custom identity was used");

            var usedType = false;
            var caughtException = TryUseType<int>(this.testContext.Boilerplate, () => { usedType = true; });

            caughtException.Should().BeFalse("because the custom type access provider was used");
            usedType.Should().BeTrue("because the custom type access provider was used");
        }    
    }
}
