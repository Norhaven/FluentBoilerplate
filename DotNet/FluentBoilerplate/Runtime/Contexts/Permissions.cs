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

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class Permissions
    {
        public static Permissions Empty { get { return new Permissions(); } }

        public IImmutableSet<IRight> RequiredRights { get; private set; }
        public IImmutableSet<IRight> RestrictedRights { get; private set; }
        public IImmutableSet<IRole> RequiredRoles { get; private set; }
        public IImmutableSet<IRole> RestrictedRoles { get; private set; }

        public Permissions(IImmutableSet<IRole> requiredRoles = null,
                           IImmutableSet<IRole> restrictedRoles = null,
                           IImmutableSet<IRight> requiredRights = null,
                           IImmutableSet<IRight> restrictedRights = null)
        {
            this.RequiredRoles = requiredRoles;
            this.RestrictedRoles = restrictedRoles;
            this.RequiredRights = requiredRights;
            this.RestrictedRights = restrictedRights;
        }

        public Permissions Merge(IImmutableSet<IRole> requiredRoles = null,
                                 IImmutableSet<IRole> restrictedRoles = null,
                                 IImmutableSet<IRight> requiredRights = null,
                                 IImmutableSet<IRight> restrictedRights = null)
        {
            return new Permissions(requiredRoles.Merge(this.RequiredRoles),
                                   restrictedRoles.Merge(this.RestrictedRoles),
                                   requiredRights.Merge(this.RequiredRights),
                                   restrictedRights.Merge(this.RestrictedRights));
        }
    }
}
