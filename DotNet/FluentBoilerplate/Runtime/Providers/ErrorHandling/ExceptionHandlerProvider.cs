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
using FluentBoilerplate.Messages;
using FluentBoilerplate.Messages.Developer;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Providers.Logging;

namespace FluentBoilerplate.Runtime.Providers.ErrorHandling
{
    internal sealed class ExceptionHandlerProvider : IExceptionHandlerProvider
    {
        public static IExceptionHandlerProvider Empty { get { return new ExceptionHandlerProvider(LogProvider.Empty); } }

        public IImmutableSet<Type> HandledExceptionTypes { get { return this.handledTypes; } }
        public IImmutableQueue<Type> HandledTypesInCatchOrder { get { return this.handledTypesInCatchOrder; } }

        private readonly ILogProvider log;
        private readonly IImmutableQueue<KeyValuePair<Type, IExceptionHandler<Exception>>> orderedHandlers;
        private readonly IImmutableQueue<Type> handledTypesInCatchOrder;
        private readonly IImmutableSet<Type> handledTypes;
        private readonly IImmutableDictionary<Type, IExceptionHandler<Exception>> handlers;

        internal ExceptionHandlerProvider(ILogProvider log, IImmutableQueue<KeyValuePair<Type, IExceptionHandler<Exception>>> orderedHandlers = null)
        {
            this.log = log;
            this.orderedHandlers = orderedHandlers ?? ImmutableQueue<KeyValuePair<Type, IExceptionHandler<Exception>>>.Empty;
            this.handledTypes = (from h in this.orderedHandlers select h.Key).ToImmutableHashSet();
            var queue = ImmutableQueue<Type>.Empty;
            foreach (var handler in this.orderedHandlers)
                queue = queue.Enqueue(handler.Key);
            this.handledTypesInCatchOrder = queue;
            this.handlers = (from h in this.orderedHandlers select h).ToDictionary(k => k.Key, k => k.Value).ToImmutableDictionary();
        }

        public IExceptionHandler<TException> TryGetHandler<TException>() where TException:Exception
        {
            IExceptionHandler<Exception> handler;
            if (!this.handlers.TryGetValue(typeof(TException), out handler))
                return null;

            return handler;
        }

        public IExceptionHandler<TException, TResult> TryGetHandler<TException, TResult>() where TException : Exception
        {
            IExceptionHandler<Exception> handler;
            if (!this.handlers.TryGetValue(typeof(TException), out handler))
                return null;

            if (handler.CanBe<IExceptionHandler<TException, TResult>>())
                return (IExceptionHandler<TException, TResult>)handler;

            return null;
        }
        
        public IExceptionHandlerProvider Add<TException, TResult>(Func<TException, TResult> action) where TException : Exception
        {
            //Lower the handler for storage and persist associated with actual type.
            //THIS IS DANGEROUS AND SHOULD ONLY BE DONE UNDER TIGHTLY CONTROLLED CIRCUMSTANCES!
            var typedHandler = new ExceptionHandler<TException, TResult>(this.log, action);
            IExceptionHandler<Exception, TResult> loweredHandler = (ExceptionHandler<Exception, TResult>)typedHandler;
           
            var type = typeof(TException);
            if (this.handledTypes.Contains(type))
                return this;

            var elevatedQueue = this.orderedHandlers.Enqueue(new KeyValuePair<Type, IExceptionHandler<Exception>>(type, loweredHandler));
            return new ExceptionHandlerProvider(this.log, elevatedQueue);            
        }

        public IExceptionHandlerProvider Add<TException>(Action<TException> action) where TException:Exception
        {
            //Lower the handler for storage and persist associated with actual type.
            //THIS IS DANGEROUS AND SHOULD ONLY BE DONE UNDER TIGHTLY CONTROLLED CIRCUMSTANCES!
            IExceptionHandler<Exception> handler = (ExceptionHandler<Exception>)new ExceptionHandler<TException>(this.log, action);

            var type = typeof(TException);
            if (this.handledTypes.Contains(type))
                return this;

            var elevatedQueue = this.orderedHandlers.Enqueue(new KeyValuePair<Type, IExceptionHandler<Exception>>(type, handler));
            return new ExceptionHandlerProvider(this.log, elevatedQueue);  
        }
    }
}
