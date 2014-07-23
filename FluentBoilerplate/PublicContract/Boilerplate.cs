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
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Runtime.Providers.Logging;
using FluentBoilerplate.Runtime.Providers.Translation;
using FluentBoilerplate.Runtime.Providers.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    public static class Boilerplate
    {
        public static IBoilerplateContext New(IIdentity identity = null, IConnectionAccessProvider serviceProvider = null, IConnectionAccessProvider dataProvider = null)
        {
            var actualIdentity = identity ?? Identity.Default;
            var functionGenerator = new FunctionGenerator();
            var logProvider = new LogProvider(functionGenerator, LogVisibility.All);
            var tryCatchProvider = new TryCatchBlockProvider(functionGenerator);
            var exceptionHandlerProvider = new ExceptionHandlerProvider(logProvider);
            var errorContext = new ImmutableErrorContext(logProvider, tryCatchProvider, exceptionHandlerProvider);

            var translationProvider = new TranslationProvider(functionGenerator);
            var validationProvider = new ValidationProvider(functionGenerator);

            var settings = new ContextSettings(errorContext: errorContext,
                                               translationProvider: translationProvider,
                                               validationProvider: validationProvider,
                                               serviceProvider: serviceProvider,
                                               dataProvider: dataProvider);

            var contract = new BoilerplateContractContext(settings);
            return new BoilerplateContext(settings, actualIdentity, contract);
        }
    }
}
