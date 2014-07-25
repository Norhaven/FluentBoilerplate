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
using FluentBoilerplate.Traits;
using FluentBoilerplate.Contexts;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class ContractContext:
        IInitialContractContext,
        IVerifiableContractContext,
        IContext,
        IBundledContractContext,
        ICopyableContractTrait<ContractContext>
    {
        private readonly ContextBundle settings;
        private readonly IContractBundle contractBundle;

        public IIdentity Identity { get; private set; }

        public ContractContext() { }
        public ContractContext(ContextBundle settings, IContractBundle contractBundle)
        {
            this.settings = settings;
            this.contractBundle = contractBundle;
        }

        public void VerifyPreConditions()
        {
            throw new NotImplementedException();
        }

        public void VerifyPostConditions(ContractExit exit)
        {
            throw new NotImplementedException();
        }




        public IInitialContractContext BeginContract()
        {
            throw new NotImplementedException();
        }

        public IContext<TResult> Get<TResult>(Func<IContext, TResult> action)
        {
            throw new NotImplementedException();
        }

        public IContext<TResult> Open<TType, TResult>(Func<IContext, TType, TResult> action)
        {
            throw new NotImplementedException();
        }

        public IContext Do(Action<IContext> action)
        {
            throw new NotImplementedException();
        }

        public TTo As<TFrom, TTo>(TFrom instance)
        {
            throw new NotImplementedException();
        }

        public ContractContext Copy(ContextBundle bundle = null, IContractBundle contractBundle = null)
        {
            throw new NotImplementedException();
        }

        public IResultContractContext<TResult> Handles<TException, TResult>(string sectionName, Func<TException, TResult> action = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IVoidReturnContractContext Handles<TException>(string sectionName, Action<TException> action = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IContext EndContract()
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext RequiresValidInstanceOf<TType>(params TType[] instances)
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext Require(Func<bool> condition, string message = null)
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext Require<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext EnsureOnReturn(Func<bool> condition, string message = null)
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext EnsureOnReturn<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext EnsureOnThrow(Func<bool> condition, string message = null)
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext EnsureOnThrow<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext RequiresRights(params IRight[] rights)
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext MustNotHaveRights(params IRight[] rights)
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext RequiresRoles(params IRole[] roles)
        {
            throw new NotImplementedException();
        }

        public IInitialContractContext MustNotHaveRoles(params IRole[] roles)
        {
            throw new NotImplementedException();
        }

        public IContractBundle Bundle
        {
            get { throw new NotImplementedException(); }
        }
    }    
}
