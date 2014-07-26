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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using FluentBoilerplate.Runtime;

namespace FluentBoilerplate.Tests.Runtime.ILWriterTests
{
    [TestClass]
    public class CatchBlock
    {
        [TestMethod]
        public void CatchBlockDoesNotUnbalanceTheStack()
        {
            var method = new DynamicMethod(String.Empty, typeof(void), Type.EmptyTypes);
            var generator = method.GetILGenerator();
            var il = new ILWriter(generator);

            var exceptionBlock = generator.BeginExceptionBlock();
            generator.Emit(OpCodes.Leave, exceptionBlock);
            il.CatchBlock(typeof(Exception), exceptionBlock, () => { });
            il.VerifyStack();
        }

        [TestMethod]
        public void CatchBlockOverloadDoesNotUnbalanceTheStack()
        {
            var method = new DynamicMethod(String.Empty, typeof(void), Type.EmptyTypes);
            var generator = method.GetILGenerator();
            var il = new ILWriter(generator);

            var exceptionBlock = generator.BeginExceptionBlock();
            generator.Emit(OpCodes.Leave, exceptionBlock);
            il.CatchBlock<Exception>(exceptionBlock, () => { });
            il.VerifyStack();
        }
    }
}
