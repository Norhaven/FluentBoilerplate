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
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Providers;

namespace FluentBoilerplate.Runtime.Providers.ErrorHandling
{
    internal sealed class ExceptionHandler<TException, TResult> : ExceptionHandler<TException>, IExceptionHandler<TException, TResult> where TException : Exception
    {
        public static implicit operator ExceptionHandler<Exception, TResult>(ExceptionHandler<TException, TResult> handler)
        {
            return LowerHandler(handler);
        }

        private static ExceptionHandler<Exception, TResult> LowerHandler(ExceptionHandler<TException, TResult> handler)
        {
            Debug.Assert(handler.funcHandler != null, AssertFailures.NoExceptionHandlerForType.WithValues(typeof(TException)));

            //The call to Generalize() is a quick way to patch in non-typesafe conversions 
            //in what should be a typesafe way (as far as the caller is concerned).
            //It amounts to covariantly converting a generically typed delegate (the patch) and
            //then putting it back in line with a contravariant conversion, both below in the constructor
            //and when retrieving it through the ExceptionHandlerProvider. 
            //THIS IS DANGEROUS AND SHOULD ONLY BE DONE UNDER TIGHTLY CONTROLLED CIRCUMSTANCES!

            var action = handler.funcHandler.Generalize();
            return new ExceptionHandler<Exception, TResult>(handler.log, handler.sectionName, action);
        }

        private readonly Func<TException, TResult> funcHandler;

        public ExceptionHandler(ILogProvider log, string sectionName, Func<TException, TResult> func)
            : base(log, sectionName, func.AsAction())
        {
            this.funcHandler = func;
        }
        public TResult HandleWithResult(TException exception)
        {
            LogException(exception);

            if (this.funcHandler != null)
            {
                var result = this.funcHandler(exception);
                if (result == null)
                    return default(TResult);

                Debug.Assert(result is TResult, AssertFailures.ReturnedTypeIsNotExpectedType.WithValues(result.GetType(), typeof(TResult)));

                return (TResult)result;
            }
            else
            {
                Debug.Fail(AssertFailures.NoExceptionHandlerForType.WithValues(typeof(TException)));
                return default(TResult);
            }
        }
    }
}
