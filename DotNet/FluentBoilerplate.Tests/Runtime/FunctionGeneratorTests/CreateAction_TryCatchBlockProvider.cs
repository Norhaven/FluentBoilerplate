using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using Moq;
using FluentBoilerplate.Providers;

namespace FluentBoilerplate.Tests.Runtime.FunctionGeneratorTests
{
    [TestClass]
    public class CreateAction_TryCatchBlockProvider:BasePEVerifyTest
    {
        private FunctionGenerator.PhysicalAssemblySettings settings;
        private FunctionGenerator functionGenerator;
        private TryCatchBlockProvider provider;
        private Mock<ILogProvider> log = new Mock<ILogProvider>();

        [TestInitialize]
        public void Initialize()
        {
            this.settings = new FunctionGenerator.PhysicalAssemblySettings("TryCatch", "dll", AppDomain.CurrentDomain.BaseDirectory);

            this.functionGenerator = new FunctionGenerator(this.settings);
            this.provider = new TryCatchBlockProvider(this.functionGenerator);
        }

        [TestMethod]
        public void TryCatchBodyPassesPEVerifyWithOneBlock()
        {
            var originalHandlerProvider = new ExceptionHandlerProvider(log.Object);
            var handlerProvider = originalHandlerProvider.Add<Exception>(String.Empty, ex => { });
            provider.GetTryCatchFor(handlerProvider);

            RunPEVerifyOnAssembly(this.settings.FullPath);
        }
    }
}
