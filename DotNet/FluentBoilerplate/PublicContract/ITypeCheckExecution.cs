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

using System;

namespace FluentBoilerplate
{
	public interface ITypeCheckExecution
	{
		/// <summary>
		/// Executes the provided action if the instance is of the requested type
		/// </summary>
		/// <typeparam name="TType">The requested type.</typeparam>
		/// <param name="action">The action.</param>
		TypeCheckResult IfTypeIs<TType>(Action<TType> action);
		/// <summary>
		/// Executes the provided function if the instance is of the requested type
		/// </summary>
		/// <typeparam name="TType">The requested type.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		TypeCheckResult<TResult> IfTypeIs<TType, TResult>(Func<TType, TResult> action);

		/// <summary>
        /// Conditionally executes the method call following this one if the instance 
		/// has a runtime type matching any of the provided type parameters.
        /// </summary>
        /// <typeparam name="T1">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T2">A potential type that will cause the conditional method execution.</typeparam>
        /// <returns>An instance of a conditionally executed choice.</returns>
		IThen<T1, T2> IfTypeIsAnyOf<T1, T2>();
		/// <summary>
        /// Conditionally executes the method call following this one if the instance 
		/// has a runtime type matching any of the provided type parameters.
        /// </summary>
        /// <typeparam name="T1">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T2">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T3">A potential type that will cause the conditional method execution.</typeparam>
        /// <returns>An instance of a conditionally executed choice.</returns>
		IThen<T1, T2, T3> IfTypeIsAnyOf<T1, T2, T3>();
		/// <summary>
        /// Conditionally executes the method call following this one if the instance 
		/// has a runtime type matching any of the provided type parameters.
        /// </summary>
        /// <typeparam name="T1">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T2">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T3">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T4">A potential type that will cause the conditional method execution.</typeparam>
        /// <returns>An instance of a conditionally executed choice.</returns>
		IThen<T1, T2, T3, T4> IfTypeIsAnyOf<T1, T2, T3, T4>();
		/// <summary>
        /// Conditionally executes the method call following this one if the instance 
		/// has a runtime type matching any of the provided type parameters.
        /// </summary>
        /// <typeparam name="T1">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T2">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T3">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T4">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T5">A potential type that will cause the conditional method execution.</typeparam>
        /// <returns>An instance of a conditionally executed choice.</returns>
		IThen<T1, T2, T3, T4, T5> IfTypeIsAnyOf<T1, T2, T3, T4, T5>();
		/// <summary>
        /// Conditionally executes the method call following this one if the instance 
		/// has a runtime type matching any of the provided type parameters.
        /// </summary>
        /// <typeparam name="T1">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T2">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T3">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T4">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T5">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T6">A potential type that will cause the conditional method execution.</typeparam>
        /// <returns>An instance of a conditionally executed choice.</returns>
		IThen<T1, T2, T3, T4, T5, T6> IfTypeIsAnyOf<T1, T2, T3, T4, T5, T6>();
		/// <summary>
        /// Conditionally executes the method call following this one if the instance 
		/// has a runtime type matching any of the provided type parameters.
        /// </summary>
        /// <typeparam name="T1">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T2">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T3">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T4">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T5">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T6">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T7">A potential type that will cause the conditional method execution.</typeparam>
        /// <returns>An instance of a conditionally executed choice.</returns>
		IThen<T1, T2, T3, T4, T5, T6, T7> IfTypeIsAnyOf<T1, T2, T3, T4, T5, T6, T7>();
		/// <summary>
        /// Conditionally executes the method call following this one if the instance 
		/// has a runtime type matching any of the provided type parameters.
        /// </summary>
        /// <typeparam name="T1">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T2">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T3">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T4">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T5">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T6">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T7">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T8">A potential type that will cause the conditional method execution.</typeparam>
        /// <returns>An instance of a conditionally executed choice.</returns>
		IThen<T1, T2, T3, T4, T5, T6, T7, T8> IfTypeIsAnyOf<T1, T2, T3, T4, T5, T6, T7, T8>();
		/// <summary>
        /// Conditionally executes the method call following this one if the instance 
		/// has a runtime type matching any of the provided type parameters.
        /// </summary>
        /// <typeparam name="T1">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T2">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T3">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T4">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T5">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T6">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T7">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T8">A potential type that will cause the conditional method execution.</typeparam>
        /// <typeparam name="T9">A potential type that will cause the conditional method execution.</typeparam>
        /// <returns>An instance of a conditionally executed choice.</returns>
		IThen<T1, T2, T3, T4, T5, T6, T7, T8, T9> IfTypeIsAnyOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
    
	}

	internal abstract class Then
	{
		private readonly object instance;

		public Then(object instance)
		{
			this.instance = instance;
		}

		protected internal TypeCheckResult IfTypeIs<TType>(Action<TType> action)
		{
		   var isType = this.instance is TType;
       
		   if (isType)
		   {
			   action((TType)(object)this.instance);                
		   }

		   return new TypeCheckResult(isType);
	   }

	   protected internal TypeCheckResult<TResult> IfTypeIs<TType, TResult>(Func<TType, TResult> action)
	   {
		   var isType = this.instance is TType;
		   var result = default(TResult);

		   if (isType)
		   {
			   result = action((TType)(object)this.instance);
		   }

		   return new TypeCheckResult<TResult>(isType, result);
	   }
	}


	public interface IThen<T1>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1);
        TypeCheckResult DoFirstMatched(Action<T1> doT1);        
    }

	internal sealed class Then<T1> : Then, IThen<T1>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			return false;
		}
	}

	public interface IThen<T1, T2>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2);
        TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2);        
    }

	internal sealed class Then<T1, T2> : Then, IThen<T1, T2>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			result = IfTypeIs<T2, TResult>(doT2);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			result = IfTypeIs<T2>(doT2);
			if (result) return result;
			return false;
		}
	}

	public interface IThen<T1, T2, T3>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3);
        TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3);        
    }

	internal sealed class Then<T1, T2, T3> : Then, IThen<T1, T2, T3>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			result = IfTypeIs<T2, TResult>(doT2);
			if (result) return result;
			result = IfTypeIs<T3, TResult>(doT3);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			result = IfTypeIs<T2>(doT2);
			if (result) return result;
			result = IfTypeIs<T3>(doT3);
			if (result) return result;
			return false;
		}
	}

	public interface IThen<T1, T2, T3, T4>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4);
        TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4);        
    }

	internal sealed class Then<T1, T2, T3, T4> : Then, IThen<T1, T2, T3, T4>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			result = IfTypeIs<T2, TResult>(doT2);
			if (result) return result;
			result = IfTypeIs<T3, TResult>(doT3);
			if (result) return result;
			result = IfTypeIs<T4, TResult>(doT4);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			result = IfTypeIs<T2>(doT2);
			if (result) return result;
			result = IfTypeIs<T3>(doT3);
			if (result) return result;
			result = IfTypeIs<T4>(doT4);
			if (result) return result;
			return false;
		}
	}

	public interface IThen<T1, T2, T3, T4, T5>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5);
        TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5);        
    }

	internal sealed class Then<T1, T2, T3, T4, T5> : Then, IThen<T1, T2, T3, T4, T5>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			result = IfTypeIs<T2, TResult>(doT2);
			if (result) return result;
			result = IfTypeIs<T3, TResult>(doT3);
			if (result) return result;
			result = IfTypeIs<T4, TResult>(doT4);
			if (result) return result;
			result = IfTypeIs<T5, TResult>(doT5);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			result = IfTypeIs<T2>(doT2);
			if (result) return result;
			result = IfTypeIs<T3>(doT3);
			if (result) return result;
			result = IfTypeIs<T4>(doT4);
			if (result) return result;
			result = IfTypeIs<T5>(doT5);
			if (result) return result;
			return false;
		}
	}

	public interface IThen<T1, T2, T3, T4, T5, T6>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5, Func<T6, TResult> doT6);
        TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5, Action<T6> doT6);        
    }

	internal sealed class Then<T1, T2, T3, T4, T5, T6> : Then, IThen<T1, T2, T3, T4, T5, T6>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5, Func<T6, TResult> doT6)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			result = IfTypeIs<T2, TResult>(doT2);
			if (result) return result;
			result = IfTypeIs<T3, TResult>(doT3);
			if (result) return result;
			result = IfTypeIs<T4, TResult>(doT4);
			if (result) return result;
			result = IfTypeIs<T5, TResult>(doT5);
			if (result) return result;
			result = IfTypeIs<T6, TResult>(doT6);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5, Action<T6> doT6)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			result = IfTypeIs<T2>(doT2);
			if (result) return result;
			result = IfTypeIs<T3>(doT3);
			if (result) return result;
			result = IfTypeIs<T4>(doT4);
			if (result) return result;
			result = IfTypeIs<T5>(doT5);
			if (result) return result;
			result = IfTypeIs<T6>(doT6);
			if (result) return result;
			return false;
		}
	}

	public interface IThen<T1, T2, T3, T4, T5, T6, T7>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5, Func<T6, TResult> doT6, Func<T7, TResult> doT7);
        TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5, Action<T6> doT6, Action<T7> doT7);        
    }

	internal sealed class Then<T1, T2, T3, T4, T5, T6, T7> : Then, IThen<T1, T2, T3, T4, T5, T6, T7>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5, Func<T6, TResult> doT6, Func<T7, TResult> doT7)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			result = IfTypeIs<T2, TResult>(doT2);
			if (result) return result;
			result = IfTypeIs<T3, TResult>(doT3);
			if (result) return result;
			result = IfTypeIs<T4, TResult>(doT4);
			if (result) return result;
			result = IfTypeIs<T5, TResult>(doT5);
			if (result) return result;
			result = IfTypeIs<T6, TResult>(doT6);
			if (result) return result;
			result = IfTypeIs<T7, TResult>(doT7);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5, Action<T6> doT6, Action<T7> doT7)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			result = IfTypeIs<T2>(doT2);
			if (result) return result;
			result = IfTypeIs<T3>(doT3);
			if (result) return result;
			result = IfTypeIs<T4>(doT4);
			if (result) return result;
			result = IfTypeIs<T5>(doT5);
			if (result) return result;
			result = IfTypeIs<T6>(doT6);
			if (result) return result;
			result = IfTypeIs<T7>(doT7);
			if (result) return result;
			return false;
		}
	}

	public interface IThen<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5, Func<T6, TResult> doT6, Func<T7, TResult> doT7, Func<T8, TResult> doT8);
        TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5, Action<T6> doT6, Action<T7> doT7, Action<T8> doT8);        
    }

	internal sealed class Then<T1, T2, T3, T4, T5, T6, T7, T8> : Then, IThen<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5, Func<T6, TResult> doT6, Func<T7, TResult> doT7, Func<T8, TResult> doT8)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			result = IfTypeIs<T2, TResult>(doT2);
			if (result) return result;
			result = IfTypeIs<T3, TResult>(doT3);
			if (result) return result;
			result = IfTypeIs<T4, TResult>(doT4);
			if (result) return result;
			result = IfTypeIs<T5, TResult>(doT5);
			if (result) return result;
			result = IfTypeIs<T6, TResult>(doT6);
			if (result) return result;
			result = IfTypeIs<T7, TResult>(doT7);
			if (result) return result;
			result = IfTypeIs<T8, TResult>(doT8);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5, Action<T6> doT6, Action<T7> doT7, Action<T8> doT8)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			result = IfTypeIs<T2>(doT2);
			if (result) return result;
			result = IfTypeIs<T3>(doT3);
			if (result) return result;
			result = IfTypeIs<T4>(doT4);
			if (result) return result;
			result = IfTypeIs<T5>(doT5);
			if (result) return result;
			result = IfTypeIs<T6>(doT6);
			if (result) return result;
			result = IfTypeIs<T7>(doT7);
			if (result) return result;
			result = IfTypeIs<T8>(doT8);
			if (result) return result;
			return false;
		}
	}

	public interface IThen<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5, Func<T6, TResult> doT6, Func<T7, TResult> doT7, Func<T8, TResult> doT8, Func<T9, TResult> doT9);
        TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5, Action<T6> doT6, Action<T7> doT7, Action<T8> doT8, Action<T9> doT9);        
    }

	internal sealed class Then<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Then, IThen<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		public Then(object instance):base(instance)
		{
		}

		public TypeCheckResult<TResult> GetFirstMatched<TResult>(Func<T1, TResult> doT1, Func<T2, TResult> doT2, Func<T3, TResult> doT3, Func<T4, TResult> doT4, Func<T5, TResult> doT5, Func<T6, TResult> doT6, Func<T7, TResult> doT7, Func<T8, TResult> doT8, Func<T9, TResult> doT9)
		{
			TypeCheckResult<TResult> result;
			result = IfTypeIs<T1, TResult>(doT1);
			if (result) return result;
			result = IfTypeIs<T2, TResult>(doT2);
			if (result) return result;
			result = IfTypeIs<T3, TResult>(doT3);
			if (result) return result;
			result = IfTypeIs<T4, TResult>(doT4);
			if (result) return result;
			result = IfTypeIs<T5, TResult>(doT5);
			if (result) return result;
			result = IfTypeIs<T6, TResult>(doT6);
			if (result) return result;
			result = IfTypeIs<T7, TResult>(doT7);
			if (result) return result;
			result = IfTypeIs<T8, TResult>(doT8);
			if (result) return result;
			result = IfTypeIs<T9, TResult>(doT9);
			if (result) return result;
			return false;
		}

        public TypeCheckResult DoFirstMatched(Action<T1> doT1, Action<T2> doT2, Action<T3> doT3, Action<T4> doT4, Action<T5> doT5, Action<T6> doT6, Action<T7> doT7, Action<T8> doT8, Action<T9> doT9)
		{
			TypeCheckResult result;
			result = IfTypeIs<T1>(doT1);
			if (result) return result;
			result = IfTypeIs<T2>(doT2);
			if (result) return result;
			result = IfTypeIs<T3>(doT3);
			if (result) return result;
			result = IfTypeIs<T4>(doT4);
			if (result) return result;
			result = IfTypeIs<T5>(doT5);
			if (result) return result;
			result = IfTypeIs<T6>(doT6);
			if (result) return result;
			result = IfTypeIs<T7>(doT7);
			if (result) return result;
			result = IfTypeIs<T8>(doT8);
			if (result) return result;
			result = IfTypeIs<T9>(doT9);
			if (result) return result;
			return false;
		}
	}
}
 
