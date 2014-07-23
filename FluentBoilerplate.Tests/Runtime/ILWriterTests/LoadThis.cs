using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using FluentBoilerplate.Runtime;

namespace FluentBoilerplate.Tests.Runtime.ILWriterTests
{
    [TestClass]
    public class LoadThis
    {
        [TestMethod]
        public void LoadAndPopWillNotUnbalanceTheStack()
        {
            var method = new DynamicMethod(String.Empty, typeof(void), Type.EmptyTypes);
            var generator = method.GetILGenerator();
            var il = new ILWriter(generator);

            il.LoadThis();
            il.Pop();
            il.VerifyStack();        
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadWithoutPopWillUnbalanceTheStack()
        {
            var method = new DynamicMethod(String.Empty, typeof(void), Type.EmptyTypes);
            var generator = method.GetILGenerator();
            var il = new ILWriter(generator);

            il.LoadThis();
            il.VerifyStack();   
        }
    }
}
