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

using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Contexts;
using System;
using TechTalk.SpecFlow;
using System.Collections.Immutable;
using FluentAssertions;
using FluentBoilerplate.Runtime.Providers;

namespace FluentBoilerplate.Tests.PublicContract.Providers.TypeAccessProviderTests
{
    [Binding]
    public class TryAccessSteps
    {
        private readonly IImmutableSet<IRight> rights = new IRight[] { new Right(0, "Test Right") }.ToImmutableHashSet();
        private readonly TestContext testContext;
        
        public TryAccessSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [Given(@"I want to access a custom type")]
        public void GivenIWantToAccessACustomType()
        {   
            this.testContext.Identity = null;
            this.testContext.Access = null;
            this.testContext.ResultExpected = false;
        }

        [Given(@"I want to access a custom type and get a result")]
        public void GivenIWantToAccessACustomTypeAndGetAResult()
        {
            this.testContext.Access = null;
            this.testContext.Identity = null;
            this.testContext.ResultExpected = true;
            this.testContext.Response = null;
        }

        [Given(@"I have a provider with required permissions")]
        public void GivenIHaveAProviderWithRequiredPermissions()
        {            
            var permissions = new Permissions(requiredRights: this.rights);
            var permissionsProvider = new PermissionsProvider(permissions);
            this.testContext.Access = new TestTypeAccessProvider(permissionsProvider, new[] { typeof(int) });
        }

        [Given(@"I have an identity with no permissions")]
        public void GivenIHaveAnIdentityWithNoPermissions()
        {
            this.testContext.Identity = Identity.Default;
        }

        [Given(@"I have a provider with no permissions")]
        public void GivenIHaveAProviderWithNoPermissions()
        {
            this.testContext.Access = new TestTypeAccessProvider(PermissionsProvider.Empty, new[] { typeof(int) });
        }

        [Given(@"I have an identity with permissions")]
        public void GivenIHaveAnIdentityWithPermissions()
        {
            this.testContext.Identity = new Identity(permittedRights: this.rights);
        }

        [When(@"I try to access the type")]
        public void WhenITryToAccessTheType()
        {
            if (this.testContext.ResultExpected)
                this.testContext.Response = this.testContext.Access.TryAccess<int, object>(this.testContext.Identity, value => value);
            else
                this.testContext.Response = this.testContext.Access.TryAccess<int>(this.testContext.Identity, value => { });
        }
        
        [Then(@"I should be able to access the type")]
        public void ThenIShouldBeAbleToAccessTheType()
        {
            this.testContext.Response.Should().NotBeNull("because we should always have a response");
            this.testContext.Response.IsSuccess.Should().BeTrue("because we accessed the type");
        }

        [Given(@"I have no provider")]
        public void GivenIHaveNoProvider()
        {
            this.testContext.Access = new TestTypeAccessProvider(null, new[] { typeof(int) });
        }

        [Then(@"I should fail to access the type")]
        public void ThenIShouldFailToAccessTheType()
        {
            this.testContext.Response.Should().NotBeNull("because we should always have a response");
            this.testContext.Response.IsSuccess.Should().BeFalse("because we could not access the type");
        }       
    }
}
