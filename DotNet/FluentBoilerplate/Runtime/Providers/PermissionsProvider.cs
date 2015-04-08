/*
   Copyright 2015 Chris Hannon

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
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

namespace FluentBoilerplate.Runtime.Providers
{
    internal sealed class PermissionsProvider : IPermissionsProvider
    {
        public static IPermissionsProvider Default { get { return new PermissionsProvider(Permissions.Empty); } }
        //TODO: Include when they're solid
        //public static IPermissionsProvider ActiveDirectoryDomain { get { return new PermissionsProvider(Permissions.Empty, ContextType.Domain); } }
        //public static IPermissionsProvider ActiveDirectoryApplicationDirectory { get { return new PermissionsProvider(Permissions.Empty, ContextType.ApplicationDirectory); } }
        public static IPermissionsProvider IncludingActiveDirectoryMachine { get { return new PermissionsProvider(Permissions.Empty, ContextType.Machine); } }

        private readonly Permissions permissions;
        private readonly IImmutableSet<IRight> requiredRights;
        private readonly IImmutableSet<IRight> restrictedRights;
        private readonly IImmutableSet<IRole> requiredRoles;
        private readonly IImmutableSet<IRole> restrictedRoles;
        private readonly IImmutableQueue<IRole> requiredActiveDirectoryRoles;
        private readonly IImmutableQueue<IRole> restrictedActiveDirectoryRoles;
        private readonly ContextType? activeDirectoryContextType;
        
        public bool HasRequiredRights { get { return this.requiredRights.Count > 0; } }
        public bool HasRestrictedRights { get { return this.restrictedRights.Count > 0; } }
        public bool HasRequiredRoles { get { return this.requiredRoles.Count > 0 || !this.requiredActiveDirectoryRoles.IsEmpty; } }
        public bool HasRestrictedRoles { get { return this.restrictedRoles.Count > 0 || !this.restrictedActiveDirectoryRoles.IsEmpty; } }
        public bool HasNoRestrictions { get { return !this.HasRestrictedRights && !this.HasRestrictedRoles; } }
        public bool HasNoRequirements { get { return !this.HasRequiredRights && !this.HasRequiredRoles; } }

        public PermissionsProvider(Permissions permissions, ContextType? activeDirectoryContextType = null)
        {
            this.permissions = permissions;
            this.requiredRights = ConsolidateRights(permissions.RequiredRights, permissions.RequiredRoles);
            this.restrictedRights = ConsolidateRights(permissions.RestrictedRights, permissions.RestrictedRoles);
            this.requiredRoles = GetManualRoles(permissions.RequiredRoles);
            this.restrictedRoles = GetManualRoles(permissions.RestrictedRoles);
            this.requiredActiveDirectoryRoles = GetActiveDirectoryRoles(permissions.RequiredRoles);
            this.restrictedActiveDirectoryRoles = GetActiveDirectoryRoles(permissions.RestrictedRoles);
            this.activeDirectoryContextType = activeDirectoryContextType; 
        }
        
        private IImmutableSet<IRole> GetManualRoles(IEnumerable<IRole> roles)
        {
            var queue = GetFilteredRoles(roles, PermissionsSource.Manual);
            return new HashSet<IRole>(queue).ToImmutableHashSet();
        }

        private IImmutableQueue<IRole> GetActiveDirectoryRoles(IEnumerable<IRole> roles)
        {
            return GetFilteredRoles(roles, PermissionsSource.ActiveDirectory);
        }

        private IImmutableQueue<IRole> GetFilteredRoles(IEnumerable<IRole> roles, PermissionsSource source)
        {
            var queue = ImmutableQueue<IRole>.Empty;

            foreach (var role in roles)
            {
                if (role.Source == source)
                    queue = queue.Enqueue(role);
            }

            return queue;
        }

        private IImmutableSet<IRight> ConsolidateRights(IImmutableSet<IRight> rights, IImmutableSet<IRole> roles)
        {
            if (roles == null)
                return rights;

            foreach (var role in roles)
            {
                rights = rights.Merge(role.Rights);
            }

            return rights.DefaultIfNull();
        }
        
        public bool HasPermission(IIdentity identity)
        {
            if (this.HasNoRestrictions && this.HasNoRequirements)
                return true;

            //Manual roles/rights will always apply first, regardless of extended permissions verification

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

            //TODO: Break extended permissions verifications into injectable types

            //Active Directory roles (if specified) apply as an extended permissions verification

            if (!VerifyActiveDirectoryRoles(identity, this.requiredActiveDirectoryRoles, isMember => isMember == true))
                return false;

            if (!VerifyActiveDirectoryRoles(identity, this.restrictedActiveDirectoryRoles, isMember => isMember == false))
                return false;

            return true;
        }

        private bool VerifyActiveDirectoryRoles(IIdentity identity, IImmutableQueue<IRole> roles, Func<bool,bool> isValid)
        {
            if (roles.IsEmpty)
                return true;

            if (this.activeDirectoryContextType == null)
                return true;

            var contextType = this.activeDirectoryContextType.Value;
            var context = new PrincipalContext(contextType);
            var user = UserPrincipal.FindByIdentity(context, identity.Name);

            if (user == null)
                throw new IdentityConfigurationException(String.Format("Identity '{0}' requires Active Directory but could not be found", identity.Name));

            for (var currentRoles = roles; !currentRoles.IsEmpty; currentRoles = currentRoles.Dequeue())
            {
                var role = currentRoles.Peek();
                var isMember = user.IsMemberOf(context, IdentityType.SamAccountName, role.Name);

                if (!isValid(isMember))
                    return false;
            }

            return true;
        }
        
        public IPermissionsProvider Merge(IImmutableSet<IRole> requiredRoles = null, 
                                          IImmutableSet<IRole> restrictedRoles = null, 
                                          IImmutableSet<IRight> requiredRights = null, 
                                          IImmutableSet<IRight> restrictedRights = null)
        {
            var mergedPermissions = this.permissions.Merge(requiredRoles, restrictedRoles, requiredRights, restrictedRights);
            return new PermissionsProvider(mergedPermissions);
        }
    }
}
