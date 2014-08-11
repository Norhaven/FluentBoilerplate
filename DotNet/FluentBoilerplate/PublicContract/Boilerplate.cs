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

using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Contexts;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Runtime.Providers.Logging;
using FluentBoilerplate.Runtime.Providers.Translation;
using FluentBoilerplate.Runtime.Providers.Validation;

namespace FluentBoilerplate
{
    /// <summary>
    /// Provides a way to easily create boilerplate contexts
    /// </summary>
    public static class Boilerplate
    {
        /// <summary>
        /// Creates an instance of <see cref="IBoilerplateContext"/>
        /// </summary>
        /// <param name="identity">The current identity being used (rights and roles contract requirements/restrictions will apply to this identity)</param>
        /// <param name="accessProvider">An access provider for specific types (available through IContext.Open())</param>
        /// <param name="permissionsProvider">The provider that will be used for all permissions verification attempts</param>
        /// <returns>An instance of <see cref="IBoilerplateContext"/></returns>
        public static IBoilerplateContext New(IIdentity identity = null, ITypeAccessProvider accessProvider = null, IPermissionsProvider permissionsProvider = null)
        {   
            var actualIdentity = identity ?? Identity.Default;
            var actualTypeAccessProvider = accessProvider ?? TypeAccessProvider.Empty;
            var actualPermissionsProvider = permissionsProvider ?? PermissionsProvider.Default;

            var functionGenerator = new FunctionGenerator();
            var translationProvider = new TranslationProvider(functionGenerator);
            var validationProvider = new ValidationProvider(functionGenerator);
            var logProvider = new LogProvider(functionGenerator, LogVisibility.All);
            var tryCatchProvider = new TryCatchBlockProvider(functionGenerator);
            var exceptionHandlerProvider = new ExceptionHandlerProvider(logProvider);
            var errorContext = new ImmutableErrorContext(logProvider, tryCatchProvider, exceptionHandlerProvider);

            var bundle = new ContextBundle(permissionsProvider: actualPermissionsProvider,
                                           errorContext: errorContext,
                                           translationProvider: translationProvider,
                                           accessProvider: actualTypeAccessProvider,
                                           validationProvider: validationProvider);

            return new InitialBoilerplateContext<ContractContext>(bundle, actualIdentity);
        }
    }
}
