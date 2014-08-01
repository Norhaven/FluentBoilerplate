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
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Contexts;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    public sealed class TestContext
    {
        public IIdentity Identity { get; set; }
        public IContext Boilerplate { get; set; }
        public ITypeAccessProvider Access { get; set; }
        public IImmutableErrorContext Errors { get; set; }
        public IPermissionsProvider Permissions { get; set; }
        public ITranslationProvider Translation { get; set; }
        public IValidationProvider Validation { get; set; }
        public bool ResultExpected { get; set; }
        public IResponse Response { get; set; }
        public Action CustomAction { get; set; }
        public Exception UnhandledException { get; set; }
        public IExceptionHandler<Exception> NonSpecificExceptionHandler { get; set; }
        public IExceptionHandler<ArgumentException> SpecificExceptionHandler { get; set; }
       
        public TestContext()
        {
        }
    }
}
