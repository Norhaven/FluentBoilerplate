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

using FluentBoilerplate.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Collections;
using System.Diagnostics;
using FluentBoilerplate.Messages;
using FluentBoilerplate.Messages.Developer;
using FluentBoilerplate;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Runtime.Providers.Logging;
using FluentBoilerplate.Contexts;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class ImmutableErrorContext : IImmutableErrorContext
    {
        public static IImmutableErrorContext Empty { get { return new ImmutableErrorContext(LogProvider.Empty, TryCatchBlockProvider.Empty, ExceptionHandlerProvider.Empty); } }
        private readonly ILogProvider log;
        private readonly IExceptionHandlerProvider handlerProvider;
        private readonly ITryCatchBlockProvider tryCatchBlockProvider;

        public int HandlerCount { get { return this.handlerProvider.HandledExceptionTypes.Count; } }
        public bool HasHandlers { get { return this.handlerProvider.HandledExceptionTypes.Count != 0; } }
        
        internal ImmutableErrorContext(ILogProvider log, ITryCatchBlockProvider tryCatchBlockProvider,IExceptionHandlerProvider handlerProvider)
        {
            this.log = log;
            this.tryCatchBlockProvider = tryCatchBlockProvider;
            this.handlerProvider = handlerProvider;
        }

        public bool HasHandlerFor<TException>() where TException:Exception
        {
            return this.handlerProvider.HandledExceptionTypes.Contains(typeof(TException));
        }

        public IImmutableErrorContext RegisterExceptionHandler<TException,TResult>(string sectionName, Func<TException, TResult> handler) where TException : Exception
        {
            var elevatedHandlerProvider = this.handlerProvider.Add(sectionName, handler);

            return new ImmutableErrorContext(this.log, this.tryCatchBlockProvider,elevatedHandlerProvider);
        }

        public IImmutableErrorContext RegisterExceptionHandler<TException>(string sectionName, Action<TException> handler) where TException : Exception
        {
            //Re-registering for the same exception type needs to ignore the request
            if (this.handlerProvider.HandledExceptionTypes.Contains(typeof(TException)))
                return this;

            var elevatedHandlerProvider = this.handlerProvider.Add(sectionName, handler);

            return new ImmutableErrorContext(this.log, this.tryCatchBlockProvider, elevatedHandlerProvider);
        }
        
        public void DoInContext(Action<IImmutableErrorContext> action)
        {
            var tryCatch = this.tryCatchBlockProvider.GetTryCatchFor(this.handlerProvider);
            tryCatch.Try(() => action(this));
        }

        public T DoInContext<T>(Func<IImmutableErrorContext, T> action)
        {
            var tryCatch = this.tryCatchBlockProvider.GetTryCatchFor(this.handlerProvider);
            return tryCatch.Try(() => action(this));
        }

        public Action ExtendAround(Action action)
        {
            if (this.handlerProvider.HandledExceptionTypes.Count == 0)
                return action;

            Debug.Assert(this.tryCatchBlockProvider != null, AssertFailures.TryCatchProviderNotFound);

            return () => DoInContext(_ => action);
        }

        public Action<T> ExtendAround<T>(Action<T> action)
        {
            if (this.handlerProvider.HandledExceptionTypes.Count == 0)
                return action;

            Debug.Assert(this.tryCatchBlockProvider != null, AssertFailures.TryCatchProviderNotFound);

            return value => DoInContext(_ => action(value));
        }

        public Func<T> ExtendAround<T>(Func<T> action)
        {
            if (this.handlerProvider.HandledExceptionTypes.Count == 0)
                return action;

            Debug.Assert(this.tryCatchBlockProvider != null, AssertFailures.TryCatchProviderNotFound);

            return () => DoInContext(_ => action());
        }

        public Func<T, TResult> ExtendAround<T, TResult>(Func<T, TResult> action)
        {
            if (this.handlerProvider.HandledExceptionTypes.Count == 0)
                return action;

            Debug.Assert(this.tryCatchBlockProvider != null, AssertFailures.TryCatchProviderNotFound);

            return value => DoInContext(_ => action(value));
        }


        public IImmutableErrorContext Copy(bool includeHandlers = true)
        {
            var provider = (includeHandlers) ? this.handlerProvider : new ExceptionHandlerProvider(this.log);
            return new ImmutableErrorContext(this.log, this.tryCatchBlockProvider, provider);
        }
        
        public Action<T1, T2> ExtendAround<T1, T2>(Action<T1, T2> action)
        {
            if (this.handlerProvider.HandledExceptionTypes.Count == 0)
                return action;

            Debug.Assert(this.tryCatchBlockProvider != null, AssertFailures.TryCatchProviderNotFound);

            return (t1, t2) => DoInContext(_ => action(t1, t2));
        }

        public Func<T1, T2, TResult> ExtendAround<T1, T2, TResult>(Func<T1, T2, TResult> action)
        {
            if (this.handlerProvider.HandledExceptionTypes.Count == 0)
                return action;

            Debug.Assert(this.tryCatchBlockProvider != null, AssertFailures.TryCatchProviderNotFound);

            return (t1, t2) => DoInContext(_ => action(t1, t2));
        }
    }
}
