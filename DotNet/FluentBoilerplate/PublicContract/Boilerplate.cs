﻿/*
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
        /// Creates an instance of <see cref="IContext"/>
        /// </summary>
        /// <param name="identity">The current identity being used (rights/roles contract requirements or restrictions will apply to this identity)</param>
        /// <param name="accessProvider">An access provider for specific types (available through IContext.Open())</param>
        /// <returns>An instance of <see cref="IContext"/></returns>
        public static IContext New(IIdentity identity = null, ITypeAccessProvider accessProvider = null)
        {
            var actualIdentity = identity ?? Identity.Default;
            var actualTypeAccessProvider = accessProvider ?? TypeAccessProvider.Empty;
            var functionGenerator = new FunctionGenerator();
            var logProvider = new LogProvider(functionGenerator, LogVisibility.All);
            var tryCatchProvider = new TryCatchBlockProvider(functionGenerator);
            var exceptionHandlerProvider = new ExceptionHandlerProvider(logProvider);
            var errorContext = new ImmutableErrorContext(logProvider, tryCatchProvider, exceptionHandlerProvider);

            var translationProvider = new TranslationProvider(functionGenerator);
            var validationProvider = new ValidationProvider(functionGenerator);

            var settings = new ContextBundle(PermissionsProvider.Empty,
                                             errorContext: errorContext,
                                             translationProvider: translationProvider,
                                             accessProvider: actualTypeAccessProvider,
                                             validationProvider: validationProvider);

            return new InitialBoilerplateContext<ContractContext>(settings, actualIdentity, null);
        }
    }
}
