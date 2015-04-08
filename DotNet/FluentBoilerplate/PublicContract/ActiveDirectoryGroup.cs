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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents an Active Directory group as a standard role
    /// </summary>
    public sealed class ActiveDirectoryGroup:Role
    {
        /// <summary>
        /// Gets the "Administrators" role
        /// </summary>
        public static IRole Administrators { get { return new ActiveDirectoryGroup("Administrators"); } }
        /// <summary>
        /// Gets the "Everyone" role
        /// </summary>
        public static IRole Everyone { get { return new ActiveDirectoryGroup("Everyone"); } }
        /// <summary>
        /// Gets the "Users" role
        /// </summary>
        public static IRole Users { get { return new ActiveDirectoryGroup("Users"); } }
        /// <summary>
        /// Gets the "Authenticated Users" role
        /// </summary>
        public static IRole AuthenticatedUsers { get { return new ActiveDirectoryGroup("Authenticated Users"); } }
        /// <summary>
        /// Gets the "SYSTEM" role
        /// </summary>
        public static IRole System { get { return new ActiveDirectoryGroup("SYSTEM"); } }

        /// <summary>
        /// Creates a new instance of the <see cref="ActiveDirectoryGroup"/> class.
        /// </summary>
        /// <param name="samName">The Security Access Management name of this group</param>
        /// <param name="description">The description</param>
        public ActiveDirectoryGroup(string samName, string description = null)
            :base(0, samName, description ?? String.Empty, Right.EmptyRights, PermissionsSource.ActiveDirectory)
        {
        }
    }
}
