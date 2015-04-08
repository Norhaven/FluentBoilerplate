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
namespace FluentBoilerplate.Runtime
{
#if DEBUG
    public
#else
    internal 
#endif 
        interface IFunctionGenerator
    {
        Action<T> CreateAction<T>(Action<ILWriter> writeActionBody);
        Action<T1, T2> CreateAction<T1, T2>(Action<ILWriter> writeActionBody);
        Func<TIn, TOut> Create<TIn, TOut>(Action<ILWriter> writeFunctionBody);
        Func<TIn1, TIn2, TOut> Create<TIn1, TIn2, TOut>(Action<ILWriter> writeFunctionBody);
    }
}
