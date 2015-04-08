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

using FluentBoilerplate;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    /// <summary>
    /// An identity that encapsulates the permissions for a given user
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// Gets the name of this identity
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the roles that this identity is a member of
        /// </summary>
        IImmutableSet<IRole> PermittedRoles { get; }

        /// <summary>
        /// Gets the roles that this identity is explicitly denied
        /// </summary>
        IImmutableSet<IRole> DeniedRoles { get; }

        /// <summary>
        /// Gets the rights that this identity has
        /// </summary>
        IImmutableSet<IRight> PermittedRights { get; }

        /// <summary>
        /// Gets the rights that this identity is explicitly denied
        /// </summary>
        IImmutableSet<IRight> DeniedRights { get; }

        /// <summary>
        /// Copies the identity
        /// </summary>
        /// <param name="permittedRoles">A new set of permitted roles associated with the identity</param>
        /// <param name="deniedRoles">A new set of denied roles associated with the identity</param>
        /// <param name="permittedRights">A new set of permitted rights associated with the identity</param>
        /// <param name="deniedRights">A new set of denied rights associated with the identity</param>
        /// <returns></returns>
        IIdentity Copy(IImmutableSet<IRole> permittedRoles = null,
                       IImmutableSet<IRole> deniedRoles = null,
                       IImmutableSet<IRight> permittedRights = null,
                       IImmutableSet<IRight> deniedRights = null);
    }
}
