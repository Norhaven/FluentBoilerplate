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

using FluentBoilerplate.Runtime.Extensions;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class BoilerplateContractContext:
        IBoilerplateContractContext,
        IVerifiableContractContext
    {
        private readonly ContextBundle settings;

        public BoilerplateContractContext(ContextBundle settings)
        {
            this.settings = settings;
        }

        public IBoilerplateContractContext<TResult> Handles<TException, TResult>(string sectionName, Func<TException, TResult> action = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IBoilerplateContext EndContract()
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> RequiresValidInstanceOf<TType>(params TType[] instances)
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> Require(Func<bool> condition, string message = null)
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> Require<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> EnsureOnReturn(Func<bool> condition, string message = null)
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> EnsureOnReturn<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> EnsureOnThrow(Func<bool> condition, string message = null)
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> EnsureOnThrow<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> Handles<TException>(string sectionName, Action<TException> action = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public void VerifyPreConditions()
        {
            throw new NotImplementedException();
        }

        public void VerifyPostConditions(ContractExit exit)
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> RequiresRights(params IRight[] rights)
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> MustNotHaveRights(params IRight[] rights)
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> RequiresRoles(params IRole[] roles)
        {
            throw new NotImplementedException();
        }

        public IContractContext<IBoilerplateContractContext, IBoilerplateContext> MustNotHaveRoles(params IRole[] roles)
        {
            throw new NotImplementedException();
        }
    }    
}
