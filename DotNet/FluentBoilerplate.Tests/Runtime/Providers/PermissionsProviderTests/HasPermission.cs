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
using FluentAssertions;
using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime.Contexts;
using System.Collections.Immutable;

namespace FluentBoilerplate.Tests.Runtime.Providers.PermissionsProviderTests
{
    [TestFixture]
    public class HasPermission
    {
        [Test]
        public void EmptyPermissionSetWillPassIfIdentityHasNoPermissions()
        {
            var provider = new PermissionsProvider(Permissions.Empty);
            var identity = Identity.Default;
            var success = provider.HasPermission(identity);

            success.Should().BeTrue("because there are no permissions to check");
        }

        [Test]
        public void RequiredPermissionSetWillPassIfIdentityHasRequiredPermissions()
        {
            var right = new Right(0, "Test Right");
            var set = new IRight[] { right }.ToImmutableHashSet();
            var permissions = new Permissions(requiredRights: set);
            var provider = new PermissionsProvider(Permissions.Empty);
            var identity = Identity.Default.Copy(permittedRights: set);
            var success = provider.HasPermission(identity);

            success.Should().BeTrue("because the identity has the correct permissions");
        }

        [Test]
        public void RequiredPermissionSetWillFailIfIdentityDoesNotHaveRequiredPermissions()
        {
            var right = new Right(0, "Test Right");
            var set = new IRight[] { right }.ToImmutableHashSet();
            var permissions = new Permissions(requiredRights: set);
            var provider = new PermissionsProvider(permissions);
            var identity = Identity.Default;
            var success = provider.HasPermission(identity);

            success.Should().BeFalse("because the identity does not have the correct permissions");
        }
    }
}
