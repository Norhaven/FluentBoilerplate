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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{   
    public class ContextSettings
    {
        protected readonly IImmutableErrorContext errorContext;
        protected readonly IConnectionAccessProvider serviceProvider;
        protected readonly IConnectionAccessProvider dataProvider;
        protected readonly ITranslationProvider translationProvider;
        protected readonly IValidationProvider validationProvider;

        //TODO: Replace with permissions provider
        public IImmutableSet<IRight> RequiredRights { get; private set; }
        public IImmutableSet<IRight> RestrictedRights { get; private set; }
        public IImmutableSet<IRole> RequiredRoles { get; private set; }
        public IImmutableSet<IRole> RestrictedRoles { get; private set; }
        public IValidationProvider ValidationProvider { get { return this.validationProvider; } }
        public IImmutableErrorContext ErrorContext { get { return errorContext; } }
        public IConnectionAccessProvider ServiceProvider { get { return serviceProvider; } }
        public IConnectionAccessProvider DataProvider { get { return dataProvider; } }
        public ITranslationProvider TranslationProvider { get { return translationProvider; } }

        public ContextSettings(IImmutableSet<IRole> requiredRoles = null,
                               IImmutableSet<IRole> restrictedRoles = null,
                               IImmutableSet<IRight> requiredRights = null,
                               IImmutableSet<IRight> restrictedRights = null, 
                               IImmutableErrorContext errorContext = null, 
                               IConnectionAccessProvider serviceProvider = null, 
                               IConnectionAccessProvider dataProvider = null, 
                               ITranslationProvider translationProvider = null, 
                               IValidationProvider validationProvider = null)
        {
            this.RequiredRoles = requiredRoles;
            this.RestrictedRoles = restrictedRoles;
            this.RequiredRights = requiredRights;
            this.RestrictedRights = restrictedRights;
            this.errorContext = errorContext;
            this.serviceProvider = serviceProvider;
            this.dataProvider = dataProvider;
            this.translationProvider = translationProvider;
            this.validationProvider = validationProvider;
        }

        public ContextSettings Copy(IImmutableSet<IRole> requiredRoles = null,
                                                           IImmutableSet<IRole> restrictedRoles = null,
                                                           IImmutableSet<IRight> requiredRights = null,
                                                           IImmutableSet<IRight> restrictedRights = null,
                                                           IImmutableErrorContext errorContext = null,
                                                           IConnectionAccessProvider serviceProvider = null,
                                                           IConnectionAccessProvider dataProvider = null,
                                                           ITranslationProvider translationProvider = null,
                                                           IValidationProvider validationProvider = null)
        {
            return new ContextSettings(requiredRoles ?? this.RequiredRoles,
                                                              restrictedRoles ?? this.RestrictedRoles,
                                                              requiredRights ?? this.RequiredRights,
                                                              restrictedRights ?? this.RestrictedRights,
                                                              errorContext ?? this.errorContext,
                                                              serviceProvider ?? this.serviceProvider,
                                                              dataProvider ?? this.dataProvider,
                                                              translationProvider ?? this.translationProvider,
                                                              validationProvider ?? this.validationProvider);
        }
    }
}
