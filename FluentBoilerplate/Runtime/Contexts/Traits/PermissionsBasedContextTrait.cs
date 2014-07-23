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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Runtime.Contexts;
using FluentBoilerplate.Traits;

namespace FluentBoilerplate.Runtime.Contexts.Traits
{
    public sealed class PermissionsBasedContextTrait<TContext> :
        IPermissionsBasedTrait<TContext>
        where TContext : IRightsBasedTrait<TContext>,
                         IRolesBasedTrait<TContext>,
                         ICopyableTrait<TContext>,
                         IPermissionsBasedTrait<TContext>
    {
        private readonly TContext context;
        private readonly ContextSettings settings;
        private readonly IImmutableSet<IRight> requiredRights;
        private readonly IImmutableSet<IRight> restrictedRights;
        private readonly IImmutableSet<IRole> requiredRoles;
        private readonly IImmutableSet<IRole> restrictedRoles;

        public IImmutableSet<IRight> RequiredRights { get { return this.requiredRights; } }
        public IImmutableSet<IRight> RestrictedRights { get { return this.restrictedRights; } }
        public IImmutableSet<IRole> RequiredRoles { get { return this.requiredRoles; } }
        public IImmutableSet<IRole> RestrictedRoles { get { return this.restrictedRoles; } }

        public PermissionsBasedContextTrait(TContext context, ContextSettings settings)
        {
            this.context = context;
            this.settings = settings;
        }

        public TContext RequiresRights(params IRight[] rights)
        {
            if (rights == null || rights.Length == 0)
                return this.context;

            var newRequiredRights = this.requiredRights.Merge(rights);
            var elevatedSettings = this.settings.Copy(requiredRights: newRequiredRights);
            return this.context.Copy(elevatedSettings);
        }

        public TContext MustNotHaveRights(params IRight[] rights)
        {
            if (rights == null || rights.Length == 0)
                return this.context;

            var newRestrictedRights = this.restrictedRights.Merge(rights);
            var elevatedSettings = this.settings.Copy(restrictedRights: newRestrictedRights);
            return this.context.Copy(elevatedSettings);
        }

        public TContext RequiresRoles(params IRole[] roles)
        {
            if (roles == null || roles.Length == 0)
                return this.context;

            var newRequiredRoles = this.requiredRoles.Merge(roles);
            var elevatedSettings = this.settings.Copy(requiredRoles: newRequiredRoles);
            return this.context.Copy(elevatedSettings);
        }

        public TContext MustNotHaveRoles(params IRole[] roles)
        {
            if (roles == null || roles.Length == 0)
                return this.context;

            var newRestrictedRoles = this.restrictedRoles.Merge(roles);
            var elevatedSettings = this.settings.Copy(restrictedRoles: newRestrictedRoles);
            return this.context.Copy(elevatedSettings);
        }
    }
}
