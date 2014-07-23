using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.IO;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers.Translation;
using FluentBoilerplate.Providers;
using System.Diagnostics;
using FluentBoilerplate.Runtime.Providers.Logging;

namespace FluentBoilerplate.Tests.Runtime.FunctionGeneratorTests
{
    [TestClass]
    public class Create_LogProvider:BasePEVerifyTest
    {
        [Log]
        public class DebugTest
        {
            [Log(Visibility=LogVisibility.Debug)]
            public string Message = "Hello";
        }
        private FunctionGenerator.PhysicalAssemblySettings settings;
        private FunctionGenerator functionGenerator;
        private LogProvider provider;

        [TestInitialize]
        public void Initialize()
        {
            this.settings = new FunctionGenerator.PhysicalAssemblySettings("Log", "dll", AppDomain.CurrentDomain.BaseDirectory);

            this.functionGenerator = new FunctionGenerator(this.settings);
            this.provider = new LogProvider(this.functionGenerator, LogVisibility.Debug | LogVisibility.Warning);
        }

        [TestMethod]
        [Conditional("PEVERIFY")]
        public void LogProviderBodyPassesPEVerify()
        {
            this.provider.CreateTypeLogger(typeof(DebugTest));

            RunPEVerifyOnAssembly(this.settings.FullPath);
        }
    }
}
