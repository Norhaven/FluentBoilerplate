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
using FluentBoilerplate.Providers;


namespace FluentBoilerplate.Runtime
{
    internal sealed class ConversionBuilder<TFrom>:IConversionBuilder<TFrom>, ITypeCheckExecution
    {
        private readonly TFrom instance;
        private readonly ITranslationProvider provider;

        public ConversionBuilder(ITranslationProvider provider, TFrom instance)
        {
            this.instance = instance;
            this.provider = provider;
        }

        public TType As<TType>()
        {
            return this.provider.Translate<TFrom, TType>(this.instance);
        }

        public Atomic<TFrom> AsAtomic()
        {
            return new Atomic<TFrom>(this.instance);
        }

        public TypeCheckResult IfTypeIs<TType>(Action<TType> action)
        {
            var then = new Then<TType>(this.instance);
			return then.IfTypeIs<TType>(action);
        }

        public TypeCheckResult<TResult> IfTypeIs<TType, TResult>(Func<TType, TResult> action)
        {
            var then = new Then<TType>(this.instance);
			return this.IfTypeIs<TType, TResult>(action);
        }

        public IThen<T1, T2> IfTypeIsAnyOf<T1, T2>()
        {
            return new Then<T1, T2>(this.instance);
        }

        public IThen<T1, T2, T3> IfTypeIsAnyOf<T1, T2, T3>()
        {
            return new Then<T1, T2, T3>(this.instance);
        }

        public IThen<T1, T2, T3, T4> IfTypeIsAnyOf<T1, T2, T3, T4>()
        {
            return new Then<T1, T2, T3, T4>(this.instance);
        }

        public IThen<T1, T2, T3, T4, T5> IfTypeIsAnyOf<T1, T2, T3, T4, T5>()
        {
            return new Then<T1, T2, T3, T4, T5>(this.instance);
        }

        public IThen<T1, T2, T3, T4, T5, T6> IfTypeIsAnyOf<T1, T2, T3, T4, T5, T6>()
        {
            return new Then<T1, T2, T3, T4, T5, T6>(this.instance);
        }

        public IThen<T1, T2, T3, T4, T5, T6, T7> IfTypeIsAnyOf<T1, T2, T3, T4, T5, T6, T7>()
        {
            return new Then<T1, T2, T3, T4, T5, T6, T7>(this.instance);
        }

        public IThen<T1, T2, T3, T4, T5, T6, T7, T8> IfTypeIsAnyOf<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            return new Then<T1, T2, T3, T4, T5, T6, T7, T8>(this.instance);
        }

        public IThen<T1, T2, T3, T4, T5, T6, T7, T8, T9> IfTypeIsAnyOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
        {
            return new Then<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this.instance);
        }
		
	}
}

