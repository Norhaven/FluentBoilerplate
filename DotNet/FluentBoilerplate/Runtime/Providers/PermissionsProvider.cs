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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;
using System.Collections.Immutable;
using FluentBoilerplate.Providers;
namespace FluentBoilerplate.Runtime.Providers
{
    internal sealed class PermissionsProvider : IPermissionsProvider
    {
        private readonly IImmutableSet<IRight> requiredRights;
        private readonly IImmutableSet<IRight> restrictedRights;
        private readonly IImmutableSet<IRole> requiredRoles;
        private readonly IImmutableSet<IRole> restrictedRoles;

        public bool HasRequiredRights { get { return this.requiredRights.Count > 0; } }
        public bool HasRestrictedRights { get { return this.restrictedRights.Count > 0; } }
        public bool HasRequiredRoles { get { return this.requiredRoles.Count > 0; } }
        public bool HasRestrictedRoles { get { return this.restrictedRoles.Count > 0; } }
        public bool HasNoRestrictions { get { return !this.HasRestrictedRights && !this.HasRestrictedRoles; } }
        public bool HasNoRequirements { get { return !this.HasRequiredRights && !this.HasRequiredRoles; } }

        public PermissionsProvider(Permissions permissions)
        {
            this.requiredRights = ConsolidateRights(permissions.RequiredRights, permissions.RequiredRoles);
            this.restrictedRights = ConsolidateRights(permissions.RestrictedRights, permissions.RestrictedRoles);
            this.requiredRoles = permissions.RequiredRoles;
            this.restrictedRoles = permissions.RestrictedRoles;
        }

        private IImmutableSet<IRight> ConsolidateRights(IImmutableSet<IRight> rights, IImmutableSet<IRole> roles)
        {               
            foreach (var role in roles)
            {
                rights = rights.Merge(role.Rights);
            }

            return rights;
        }
        
        public bool HasPermission(IIdentity identity)
        {
            if (this.HasNoRestrictions && this.HasNoRequirements)
                return true;

            //Were they explicitly denied any of the required roles or rights?  
            if (this.requiredRoles.Overlaps(identity.DeniedRoles))
                return false;

            if (this.requiredRights.Overlaps(identity.DeniedRights))
                return false;

            //Are they permitted all of the roles or rights that are required?
            if (!this.requiredRoles.IsSubsetOf(identity.PermittedRoles))
                return false;

            if (!this.requiredRights.IsSubsetOf(identity.PermittedRights))
                return false;

            //Are they permitted any roles or rights that are restricted?
            if (this.restrictedRoles.Overlaps(identity.PermittedRoles))
                return false;

            if (this.restrictedRights.Overlaps(identity.PermittedRights))
                return false;

            return true;
        }
    }
}
