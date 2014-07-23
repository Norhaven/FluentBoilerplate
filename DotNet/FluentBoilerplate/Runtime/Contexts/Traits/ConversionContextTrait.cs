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
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts.Traits
{
    internal sealed class ConversionContextTrait<TContext>:IConversionTrait<TContext>
    {
        private readonly TContext context;
        private readonly ITranslationProvider translationProvider;
        private readonly IValidationProvider validationProvider;

        public ConversionContextTrait(TContext context, ITranslationProvider translationProvider, IValidationProvider validationProvider)
        {
            this.context = context;
            this.translationProvider = translationProvider;
            this.validationProvider = validationProvider;
        }

        public TContext RequiresValidInstanceOf<TType>(params TType[] instances)
        {
            if (instances == null || instances.Length == 0)
                return this.context;

            foreach (var instance in instances)
            {
                this.validationProvider.Validate<TType>(instance);
            }

            return this.context;
        }

        public TTo As<TFrom, TTo>(TFrom instance)
        {
            return this.translationProvider.Translate<TFrom, TTo>(instance);
        }
    }
}
