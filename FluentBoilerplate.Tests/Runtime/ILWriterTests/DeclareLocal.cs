using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using FluentBoilerplate.Runtime;

namespace FluentBoilerplate.Tests.Runtime.ILWriterTests
{
    [TestClass]
    public class DeclareLocal
    {
        [TestMethod]
        public void DeclareLocalDoesNotUnbalanceTheStack()
        {
            var method = new DynamicMethod(String.Empty, typeof(void), Type.EmptyTypes);
            var generator = method.GetILGenerator();
            var il = new ILWriter(generator);

            var localBuilder = il.DeclareLocal(typeof(object));            
            il.VerifyStack();
        }
        [TestMethod]
        public void DeclareLocalOverloadDoesNotUnbalanceTheStack()
        {
            var method = new DynamicMethod(String.Empty, typeof(void), Type.EmptyTypes);
            var generator = method.GetILGenerator();
            var il = new ILWriter(generator);

            var localBuilder = il.DeclareLocal<object>();
            il.VerifyStack();
        }
    }
}
