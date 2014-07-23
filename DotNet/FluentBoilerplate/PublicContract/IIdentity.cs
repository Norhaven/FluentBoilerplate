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

using FluentBoilerplate;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    public interface IIdentity
    {
        IImmutableSet<IRole> PermittedRoles { get; }
        IImmutableSet<IRole> DeniedRoles { get; }
        IImmutableSet<IRight> PermittedRights { get; }
        IImmutableSet<IRight> DeniedRights { get; }

        IIdentity Copy(IImmutableSet<IRole> permittedRoles = null,
                       IImmutableSet<IRole> deniedRoles = null,
                       IImmutableSet<IRight> permittedRights = null,
                       IImmutableSet<IRight> deniedRights = null);
    }
}
