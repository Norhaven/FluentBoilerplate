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

using FluentBoilerplate.Contexts;

namespace FluentBoilerplate.Traits
{
    /// <summary>
    /// Represents a trait for copying a context
    /// </summary>
    /// <typeparam name="TContext">The context</typeparam>
    public interface ICopyableTrait<TContext>
    {
        /// <summary>
        /// Copies the current context
        /// </summary>
        /// <param name="bundle">A new bundle associated with the context</param>
        /// <param name="contractBundle">A new contract bundle associated with the context</param>
        /// <returns>An instance of <typeparamref name="TContext"/> that contains any applied bundle changes</returns>
        TContext Copy(IContextBundle bundle = null,
                      IContractBundle contractBundle = null);
    }
}
