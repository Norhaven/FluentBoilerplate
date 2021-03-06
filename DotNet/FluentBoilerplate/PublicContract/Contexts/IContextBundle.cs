﻿/*
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

using FluentBoilerplate.Providers;
using System;
namespace FluentBoilerplate.Contexts
{
    public interface IContextBundle
    {
        ITypeAccessProvider Access { get; }
        IImmutableErrorContext Errors { get; }
        IPermissionsProvider Permissions { get; }
        ITranslationProvider Translation { get; }
        IValidationProvider Validation { get; }
        ILogProvider Log { get; }
        Visibility Visibility { get; }
        Visibility TimingVisibility { get; }

        IContextBundle Copy(IPermissionsProvider permissionsProvider = null, 
                            IImmutableErrorContext errorContext = null, 
                            ITypeAccessProvider accessProvider = null, 
                            ITranslationProvider translationProvider = null, 
                            IValidationProvider validationProvider = null,
                            ILogProvider logProvider = null,
                            Visibility? visibility = null,
                            Visibility? timingVisibility = null);
    }
}
