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
