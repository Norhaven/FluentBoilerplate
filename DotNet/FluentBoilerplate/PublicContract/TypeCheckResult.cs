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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    public sealed class TypeCheckResult
    {
        public bool IsSuccess { get; private set; }

        public static bool operator true(TypeCheckResult result)
        {
            return result.IsSuccess;
        }

        public static bool operator false(TypeCheckResult result)
        {
            return !result.IsSuccess;
        }

        public static implicit operator bool(TypeCheckResult typeCheckResult)
        {
            return typeCheckResult.IsSuccess;
        }

        public static implicit operator TypeCheckResult(bool isSuccess)
        {
            return new TypeCheckResult(isSuccess);
        }

        public TypeCheckResult(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }
    }

    public sealed class TypeCheckResult<TResult>
    {
        public bool IsSuccess { get; private set; }
        public TResult Result { get; private set; }

        public static bool operator true(TypeCheckResult<TResult> result)
        {
            return result.IsSuccess;
        }

        public static bool operator false(TypeCheckResult<TResult> result)
        {
            return !result.IsSuccess;
        }

        public static implicit operator TResult(TypeCheckResult<TResult> typeCheckResult)
        {
            return typeCheckResult.Result;
        }

        public static implicit operator bool(TypeCheckResult<TResult> typeCheckResult)
        {
            return typeCheckResult.IsSuccess;
        }

        public static implicit operator TypeCheckResult<TResult>(bool isSuccess)
        {
            return new TypeCheckResult<TResult>(isSuccess, default(TResult));
        }

        public TypeCheckResult(bool isSuccess, TResult result)
        {
            this.IsSuccess = isSuccess;
            this.Result = result;
        }
    }
}
