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

using FluentBoilerplate.Messages;
using FluentBoilerplate.Messages.Developer;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    public abstract class ImmutableContext<TContext> 
    {
        protected readonly ContextBundle bundle;

        internal ImmutableContext(ContextBundle bundle)
        {
            this.bundle = bundle;
        }

        protected ContextBundle DowngradeErrorHandling()
        {
            var downgradedErrorContext = this.bundle.Errors.Copy(includeHandlers: false);
            return this.bundle.Copy(errorContext: downgradedErrorContext);
        }
        //TODO: Include Copy() call
    }
}
