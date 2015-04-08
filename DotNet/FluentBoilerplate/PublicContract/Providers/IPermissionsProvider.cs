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

using System.Collections.Immutable;

namespace FluentBoilerplate.Providers
{
    /// <summary>
    /// Represents a provider for permissions
    /// </summary>
    public interface IPermissionsProvider
    {
        /// <summary>
        /// Gets whether the provider has no known requirements
        /// </summary>
        bool HasNoRequirements { get; }

        /// <summary>
        /// Gets whether the provider has no known restrictions
        /// </summary>
        bool HasNoRestrictions { get; }        
        /// <summary>
        /// Gets whether the provider has any required rights
        /// </summary>
        bool HasRequiredRights { get; }
        /// <summary>
        /// Gets whether the provider has any required roles
        /// </summary>
        bool HasRequiredRoles { get; }
        /// <summary>
        /// Gets whether the provider has any restricted rights
        /// </summary>
        bool HasRestrictedRights { get; }
        /// <summary>
        /// Gets whether the provider has any restricted roles
        /// </summary>
        bool HasRestrictedRoles { get; }

        /// <summary>
        /// Determines whether the given identity has permission
        /// </summary>
        bool HasPermission(IIdentity identity);

        /// <summary>
        /// Merges any provided requirements or restrictions with the current set of permissions
        /// </summary>
        /// <param name="requiredRoles">The required roles</param>
        /// <param name="restrictedRoles">The restricted roles</param>
        /// <param name="requiredRights">The required rights</param>
        /// <param name="restrictedRights">The restricted rights</param>
        /// <returns>An instance of <see cref="IPermissionsProvider"/> that contains the merged permissions</returns>
        IPermissionsProvider Merge(IImmutableSet<IRole> requiredRoles = null,
                                  IImmutableSet<IRole> restrictedRoles = null,
                                  IImmutableSet<IRight> requiredRights = null,
                                  IImmutableSet<IRight> restrictedRights = null);
    }
}
