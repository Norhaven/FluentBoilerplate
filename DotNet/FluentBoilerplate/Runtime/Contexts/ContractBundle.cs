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

using FluentBoilerplate.Contexts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class ContractBundle:IContractBundle
    {
        public IImmutableQueue<IContractCondition> Preconditions { get; private set; }

        public IImmutableQueue<IContractCondition> PostconditionsOnReturn { get; private set; }

        public IImmutableQueue<IContractCondition> PostconditionsOnThrow { get; private set; }

        public IImmutableQueue<Action> InstanceValidations { get; private set; }

        public ContractBundle(IImmutableQueue<IContractCondition> preconditions = null,
                              IImmutableQueue<IContractCondition> postconditionsOnReturn = null,
                              IImmutableQueue<IContractCondition> postconditionsOnThrow = null,
                              IImmutableQueue<Action> instanceValidations = null)
        {
            this.Preconditions = preconditions.DefaultIfNull();
            this.PostconditionsOnReturn = postconditionsOnReturn.DefaultIfNull();
            this.PostconditionsOnThrow = postconditionsOnThrow.DefaultIfNull();
            this.InstanceValidations = instanceValidations.DefaultIfNull();
        }

        public IContractBundle AddPrecondition(IContractCondition condition)
        {
            var elevatedPreconditions = this.Preconditions.Enqueue(condition);
            return Copy(preconditions: elevatedPreconditions);
        }

        public IContractBundle AddPostconditionOnReturn(IContractCondition condition)
        {
            var elevatedPostConditions = this.PostconditionsOnReturn.Enqueue(condition);
            return Copy(postconditionsOnReturn: elevatedPostConditions);
        }

        public IContractBundle AddPostconditionOnThrow(IContractCondition condition)
        {
            var elevatedPostConditions = this.PostconditionsOnThrow.Enqueue(condition);
            return Copy(postconditionsOnThrow: elevatedPostConditions);
        }

        public IContractBundle AddInstanceValidation(Action validate)
        {
            var elevatedInstanceValidations = this.InstanceValidations.Enqueue(validate);
            return Copy(instanceValidations: elevatedInstanceValidations);
        }

        private IContractBundle Copy(IImmutableQueue<IContractCondition> preconditions = null,
                                     IImmutableQueue<IContractCondition> postconditionsOnReturn = null,
                                     IImmutableQueue<IContractCondition> postconditionsOnThrow = null,
                                     IImmutableQueue<Action> instanceValidations = null)
        {
            return new ContractBundle(preconditions ?? this.Preconditions,
                                      postconditionsOnReturn ?? this.PostconditionsOnReturn,
                                      postconditionsOnThrow ?? this.PostconditionsOnThrow,
                                      instanceValidations ?? this.InstanceValidations);
        }
    }
}
