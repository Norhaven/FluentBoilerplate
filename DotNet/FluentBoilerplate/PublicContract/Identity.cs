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

using System.Collections.Immutable;

namespace FluentBoilerplate
{
    /// <summary>
    /// An identity that encapsulates the permissions for a given user
    /// </summary>
    public class Identity : IIdentity
    {
        /// <summary>
        /// Gets the roles that this identity is a member of
        /// </summary>
        public IImmutableSet<IRole> PermittedRoles { get; private set; }
        /// <summary>
        /// Gets the roles that this identity is explicitly denied
        /// </summary>
        public IImmutableSet<IRole> DeniedRoles { get; private set; }
        /// <summary>
        /// Gets the rights that this identity has
        /// </summary>
        public IImmutableSet<IRight> PermittedRights { get; private set; }
        /// <summary>
        /// Gets the rights that this identity is explicitly denied
        /// </summary>
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

        /// <summary>
        /// Creates the default instance of <see cref="IIdentity"/>
        /// </summary>
        public static IIdentity Default { get { return new Identity(); } }

        /// <summary>
        /// Copies the identity
        /// </summary>
        /// <param name="permittedRoles">A new set of permitted roles associated with the identity</param>
        /// <param name="deniedRoles">A new set of denied roles associated with the identity</param>
        /// <param name="permittedRights">A new set of permitted rights associated with the identity</param>
        /// <param name="deniedRights">A new set of denied rights associated with the identity</param>
        /// <returns></returns>
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
