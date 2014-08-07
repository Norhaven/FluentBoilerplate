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
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

namespace FluentBoilerplate.Runtime.Providers
{
    internal sealed class PermissionsProvider : IPermissionsProvider
    {
        public static IPermissionsProvider Empty { get { return new PermissionsProvider(Permissions.Empty); } }
        public static IPermissionsProvider ActiveDirectoryDomain { get { return new PermissionsProvider(Permissions.Empty, ContextType.Domain); } }
        public static IPermissionsProvider ActiveDirectoryApplicationDirectory { get { return new PermissionsProvider(Permissions.Empty, ContextType.ApplicationDirectory); } }
        public static IPermissionsProvider ActiveDirectoryMachine { get { return new PermissionsProvider(Permissions.Empty, ContextType.Machine); } }

        private readonly Permissions permissions;
        private readonly IImmutableSet<IRight> requiredRights;
        private readonly IImmutableSet<IRight> restrictedRights;
        private readonly IImmutableSet<IRole> requiredRoles;
        private readonly IImmutableSet<IRole> restrictedRoles;
        private readonly IImmutableQueue<IRole> requiredActiveDirectoryRoles;
        private readonly IImmutableQueue<IRole> restrictedActiveDirectoryRoles;
        private readonly ContextType activeDirectoryContextType = ContextType.Domain;
        
        public bool HasRequiredRights { get { return this.requiredRights.Count > 0; } }
        public bool HasRestrictedRights { get { return this.restrictedRights.Count > 0; } }
        public bool HasRequiredRoles { get { return this.requiredRoles.Count > 0; } }
        public bool HasRestrictedRoles { get { return this.restrictedRoles.Count > 0; } }
        public bool HasNoRestrictions { get { return !this.HasRestrictedRights && !this.HasRestrictedRoles; } }
        public bool HasNoRequirements { get { return !this.HasRequiredRights && !this.HasRequiredRoles; } }

        public PermissionsProvider(Permissions permissions, ContextType activeDirectoryContextType = ContextType.Domain)
        {
            this.permissions = permissions;
            this.requiredRights = ConsolidateRights(permissions.RequiredRights, permissions.RequiredRoles);
            this.restrictedRights = ConsolidateRights(permissions.RestrictedRights, permissions.RestrictedRoles);            
            this.requiredRoles = permissions.RequiredRoles;
            this.restrictedRoles = permissions.RestrictedRoles;
            this.requiredActiveDirectoryRoles = GetActiveDirectoryRoles(permissions.RequiredRoles);
            this.restrictedActiveDirectoryRoles = GetActiveDirectoryRoles(permissions.RestrictedRoles);
            this.activeDirectoryContextType = activeDirectoryContextType;
        }
        
        private IImmutableQueue<IRole> GetActiveDirectoryRoles(IEnumerable<IRole> roles)
        {
            var queue = ImmutableQueue<IRole>.Empty;

            foreach (var role in roles)
            {
                if (role.Source == PermissionsSource.ActiveDirectory)
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

            var context = new PrincipalContext(this.activeDirectoryContextType);
            var user = UserPrincipal.FindByIdentity(context, identity.Name);

            if (user == null)
                throw new IdentityConfigurationException(String.Format("Identity '{0}' requires Active Directory but could not be found", identity.Name));

            for (var currentRoles = roles; !currentRoles.IsEmpty; currentRoles = currentRoles.Dequeue())
            {
                var role = currentRoles.Peek();
                var group = GroupPrincipal.FindByIdentity(context, role.Name);

                if (group == null)
                    throw new IdentityConfigurationException(String.Format("Identity '{0}' requires Active Directory group '{1}' which could not be found", identity.Name, role.Name));

                var isMember = user.IsMemberOf(group);

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
