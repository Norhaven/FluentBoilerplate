/*
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

using FluentBoilerplate.Messages;
using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Messages.Developer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Providers;

namespace FluentBoilerplate.Runtime.Providers.ErrorHandling
{
    internal sealed class ExceptionHandler<TException> : IVoidReturnExceptionHandler<TException> where TException : Exception
    {
        public static implicit operator ExceptionHandler<Exception>(ExceptionHandler<TException> handler)
        {
            return LowerHandler(handler);
        }

        private static ExceptionHandler<Exception> LowerHandler(ExceptionHandler<TException> handler)
        {
            Debug.Assert(handler.actionHandler != null, AssertFailures.NoExceptionHandlerForType.WithValues(typeof(TException)));

            //The call to Generalize() is a quick way to patch in non-typesafe conversions 
            //in what should be a typesafe way (as far as the caller is concerned).
            //It amounts to non-typesafely covariantly converting a generically typed delegate (the patch) and
            //then putting it back in line with a contravariant conversion to its previously known type
            //when retrieving it through the ExceptionHandlerProvider. 
            //THIS IS DANGEROUS AND SHOULD ONLY BE DONE UNDER TIGHTLY CONTROLLED CIRCUMSTANCES!

            var action = handler.actionHandler.Generalize();
            return new ExceptionHandler<Exception>(handler.log, action);
        }

        private readonly ILogProvider log;
        private readonly Action<TException> actionHandler;

        public int RetryCount { get; set; }
        public int RetryIntervalInMilliseconds { get; set; }
        public RetryBackoff Backoff { get; set; }

        public ExceptionHandler(ILogProvider log, Action<TException> actionHandler)
        {
            Debug.Assert(log != null, AssertFailures.InstanceShouldNotBeNull.WithValues("log"));
            Debug.Assert(actionHandler != null, AssertFailures.InstanceShouldNotBeNull.WithValues("actionHandler"));

            this.log = log;
            this.actionHandler = actionHandler;
        }

        public void Handle(TException exception)
        {
            LogException(exception);

            Debug.Assert(this.actionHandler != null, AssertFailures.NoExceptionHandlerForType.WithValues(typeof(TException)));

            this.actionHandler(exception);
        }

        private void LogException(TException exception)
        {
            var message = LogErrors.ActionResultedInException.WithValues(exception.GetType());
            this.log.Error(message, exception);
        }
    }
}
