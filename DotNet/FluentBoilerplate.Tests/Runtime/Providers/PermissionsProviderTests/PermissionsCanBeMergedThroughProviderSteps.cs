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

namespace FluentBoilerplate.Tests.Runtime.Providers.PermissionsProviderTests
{
    [Binding]
    public class PermissionsCanBeMergedThroughProviderSteps
    {
        private readonly TestContext testContext;

        public PermissionsCanBeMergedThroughProviderSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [When(@"I merge the permissions provider with a set of no permissions")]
        public void WhenIMergeThePermissionsProviderWithASetOfNoPermissions()
        {
            this.testContext.Permissions.Should().NotBeNull("because we need to merge the permissions");
            this.testContext.Permissions = this.testContext.Permissions.Merge(requiredRights: TestPermissions.NoRights);
        }

        [When(@"I merge the permissions provider with a set of required rights")]
        public void WhenIMergeThePermissionsProviderWithASetOfRequiredRights()
        {
            this.testContext.Permissions.Should().NotBeNull("because we need to merge the permissions");
            this.testContext.Permissions = this.testContext.Permissions.Merge(requiredRights: TestPermissions.BasicRights);
        }

        [Then(@"the permissions provider should have no permissions")]
        public void ThenThePermissionsProviderShouldHaveNoPermissions()
        {
            this.testContext.Permissions.Should().NotBeNull("because we need to verify the permissions");
            var isEmpty = this.testContext.Permissions.HasNoRequirements && this.testContext.Permissions.HasNoRestrictions;
            isEmpty.Should().BeTrue("because no permissions should be present");
        }

        [Then(@"the permissions provider should have merged the required rights")]
        public void ThenThePermissionsProviderShouldHaveMergedTheRequiredRights()
        {
            this.testContext.Permissions.Should().NotBeNull("because we need to verify the permissions");            
            this.testContext.Permissions.HasRequiredRights.Should().BeTrue("because required rights were merged");
            //TODO: Right now we're not actually verifying the merge, just checking that it has some required rights.
            //      Rework this when we have time.
        }
    }
}
