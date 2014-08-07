﻿/*
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

namespace FluentBoilerplate.Tests.Runtime.Providers.PermissionsProviderTests
{
    [Binding]
    public class IdentityPermissionsCanBeVerifiedWithActiveDirectorySteps
    {
        private readonly TestContext testContext;

        public IdentityPermissionsCanBeVerifiedWithActiveDirectorySteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [Given(@"I have created a Windows user named ""(.*)""")]
        public void GivenIHaveCreatedAWindowsUserNamed(string userName)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have created an Active Directory group named ""(.*)""")]
        public void GivenIHaveCreatedAnActiveDirectoryGroupNamed(string groupName)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have added the Windows user to the Active Directory group")]
        public void GivenIHaveAddedTheWindowsUserToTheActiveDirectoryGroup()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I am using the application directory as the Active Directory context")]
        public void GivenIAmUsingTheApplicationDirectoryAsTheActiveDirectoryContext()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I require access to an action by group ""(.*)""")]
        public void GivenIRequireAccessToAnActionByGroup(string groupName)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have not added the Windows user to the Active Directory group")]
        public void GivenIHaveNotAddedTheWindowsUserToTheActiveDirectoryGroup()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I restrict access to an action by group ""(.*)""")]
        public void GivenIRestrictAccessToAnActionByGroup(string groupName)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
