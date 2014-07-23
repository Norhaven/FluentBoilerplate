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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace FluentBoilerplate
{
    public class Identity:IIdentity
    {
        public IImmutableSet<IRole> PermittedRoles { get; private set; }
        public IImmutableSet<IRole> DeniedRoles { get; private set; }
        public IImmutableSet<IRight> PermittedRights { get; private set; }
        public IImmutableSet<IRight> DeniedRights { get; private set; }

        private Identity(IImmutableSet<IRole> permittedRoles = null,
                         IImmutableSet<IRole> deniedRoles = null,
                         IImmutableSet<IRight> permittedRights = null,
                         IImmutableSet<IRight> deniedRights = null)
        {
            this.PermittedRoles = permittedRoles;
            this.DeniedRoles = deniedRoles;
            this.PermittedRights = permittedRights;
            this.DeniedRights = deniedRights;
        }

        public static IIdentity Default { get { return new Identity(); } }
        
        public IIdentity Copy(IImmutableSet<IRole> permittedRoles = null, 
                              IImmutableSet<IRole> deniedRoles = null, 
                              IImmutableSet<IRight> permittedRights = null, 
                              IImmutableSet<IRight> deniedRights = null)
        {
            return new Identity(permittedRoles ?? this.PermittedRoles,
                                deniedRoles ?? this.DeniedRoles,
                                permittedRights ?? this.PermittedRights,
                                deniedRights ?? this.DeniedRights);
        }
    }
}
