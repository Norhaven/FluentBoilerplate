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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Contexts
{
    public interface IContractBundle
    {
        IImmutableQueue<IContractCondition> Preconditions { get; }
        IImmutableQueue<IContractCondition> PostconditionsOnReturn { get; }
        IImmutableQueue<IContractCondition> PostconditionsOnThrow { get; }
        IImmutableQueue<Action> InstanceValidations { get; }

        IContractBundle AddPrecondition(IContractCondition condition);
        IContractBundle AddPostconditionOnReturn(IContractCondition condition);
        IContractBundle AddPostconditionOnThrow(IContractCondition condition);
        IContractBundle AddInstanceValidation(Action action);
    }
}
