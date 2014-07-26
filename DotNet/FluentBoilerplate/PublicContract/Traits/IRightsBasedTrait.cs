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

namespace FluentBoilerplate.Traits
{
    /// <summary>
    /// Represents a trait that allows contract requirements for rights
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    public interface IRightsBasedTrait<TContext>
    {
        /// <summary>
        /// Indicates that the current identity must have a set of rights prior to performing a context action
        /// </summary>
        /// <param name="rights">The required rights</param>
        /// <returns>An instance of <typeparamref name="TContext"/> that contains the new requirements</returns>
        TContext RequiresRights(params IRight[] rights);
        /// <summary>
        /// Indicates that the current identity must not have a set of rights prior to performing a context action
        /// </summary>
        /// <param name="rights">The restricted rights</param>
        /// <returns>An instance of <typeparamref name="TContext"/> that contains the new requirements</returns>
        TContext MustNotHaveRights(params IRight[] rights);
    }
}
