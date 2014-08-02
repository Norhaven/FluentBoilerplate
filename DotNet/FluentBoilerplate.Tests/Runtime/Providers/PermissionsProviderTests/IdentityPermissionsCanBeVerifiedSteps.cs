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
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime.Contexts;

namespace FluentBoilerplate.Tests.Runtime.Providers.PermissionsProviderTests
{
    [Binding]
    public class IdentityPermissionsCanBeVerifiedSteps
    {
        private readonly TestContext testContext;

        public IdentityPermissionsCanBeVerifiedSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [Given(@"I have a permissions provider with no permissions")]
        public void GivenIHaveAPermissionsProviderWithNoPermissions()
        {
            this.testContext.Permissions = PermissionsProvider.Empty;
        }

        [Given(@"I have a permissions provider with required permissions")]
        public void GivenIHaveAPermissionsProviderWithRequiredPermissions()
        {
            var permissions = new Permissions(requiredRights: TestPermissions.BasicRights);
            this.testContext.Permissions = new PermissionsProvider(permissions);
        }

        [When(@"I verify identity permissions through the provider")]
        public void WhenIVerifyIdentityPermissionsThroughTheProvider()
        {
            this.testContext.Permissions.Should().NotBeNull("because we need to verify identity permissions");
            this.testContext.IdentityHasPermission = this.testContext.Permissions.HasPermission(this.testContext.Identity);            
        }

        [Then(@"the identity should have permission")]
        public void ThenTheIdentityShouldHavePermission()
        {
            this.testContext.IdentityHasPermission.Should().BeTrue("because the identity should have permission");
        }

        [Then(@"the identity should not have permission")]
        public void ThenTheIdentityShouldNotHavePermission()
        {
            this.testContext.IdentityHasPermission.Should().BeFalse("because the identity should not have permission");
        }
    }
}
