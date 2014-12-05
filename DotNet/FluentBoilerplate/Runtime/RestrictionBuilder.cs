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
using FluentBoilerplate.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class RestrictionBuilder<TPublicContext, TActualContext> : IRestrictionBuilder<TPublicContext>
        where TActualContext :
            ICopyableTrait<TPublicContext>,
            TPublicContext
    {
        private readonly TActualContext actualContext;
        private readonly IContextBundle bundle;
        private readonly IContractBundle contractBundle;
        
        public IThreadRestrictionBuilder<TPublicContext> Threads
        {
            get
            {
                return new ThreadRestrictionBuilder<TPublicContext, TActualContext>(this.actualContext, this.bundle, this.contractBundle);
            }
        }

        public RestrictionBuilder(TActualContext actualContext, IContextBundle bundle, IContractBundle contractBundle)
        {
            this.actualContext = actualContext;
            this.bundle = bundle;
            this.contractBundle = contractBundle;
        }
    }
}
