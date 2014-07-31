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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime.Contexts;
using FluentAssertions;
using FluentBoilerplate.Runtime.Extensions;
using System.Collections.Immutable;

namespace FluentBoilerplate.Tests.Runtime.Providers.PermissionsProviderTests
{
    [TestClass]
    public class Merge
    {
        [TestMethod]
        public void NoNewItemsWillReturnIdenticalInstance()
        {
            var provider = new PermissionsProvider(Permissions.Empty);
            var merged = provider.Merge();

            merged.Should().NotBeNull("because the instance wasn't null");
            merged.HasRestrictedRoles.Should().Be(provider.HasRestrictedRoles, "because it's an identical instance");
            merged.HasRestrictedRights.Should().Be(provider.HasRestrictedRights, "because it's an identical instance");
            merged.HasRequiredRoles.Should().Be(provider.HasRequiredRoles, "because it's an identical instance");
            merged.HasRequiredRights.Should().Be(provider.HasRequiredRights, "because it's an identical instance");
        }

        [TestMethod]
        public void NewRightsOnTopOfEmptyProviderWillReturnProviderWithNewRights()
        {
            var provider = new PermissionsProvider(Permissions.Empty);
            var merged = provider.Merge(requiredRights: new IRight[] { new Right(0, "Test Right") }.ToImmutableHashSet());

            merged.Should().NotBeNull("because the instance wasn't null");
            merged.HasRestrictedRoles.Should().Be(provider.HasRestrictedRoles, "because this property didn't change");
            merged.HasRestrictedRights.Should().Be(provider.HasRestrictedRights, "because this property didn't change");
            merged.HasRequiredRoles.Should().Be(provider.HasRequiredRoles, "because this property didn't change");
            merged.HasRequiredRights.Should().Be(true, "because required rights were added");
        }

        [TestMethod]
        public void NewRightsOnTopOfProviderWithRightsWillReturnProviderWithMergedRights()
        {
            var permissions = new Permissions(requiredRights: new IRight[] { new Right(1, "Some Test Right") }.ToImmutableHashSet());
            var provider = new PermissionsProvider(Permissions.Empty);
            var merged = provider.Merge(requiredRights: new IRight[] { new Right(0, "Test Right") }.ToImmutableHashSet());

            merged.Should().NotBeNull("because the instance wasn't null");
            merged.HasRestrictedRoles.Should().Be(provider.HasRestrictedRoles, "because this property didn't change");
            merged.HasRestrictedRights.Should().Be(provider.HasRestrictedRights, "because this property didn't change");
            merged.HasRequiredRoles.Should().Be(provider.HasRequiredRoles, "because this property didn't change");
            merged.HasRequiredRights.Should().Be(true, "because rights existed and were added");
        }
    }
}
