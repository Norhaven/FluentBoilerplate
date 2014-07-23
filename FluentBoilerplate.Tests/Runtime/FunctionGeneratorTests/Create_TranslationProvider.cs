using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.IO;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers.Translation;
using FluentBoilerplate.Providers;
using System.Diagnostics;

namespace FluentBoilerplate.Tests.Runtime.FunctionGeneratorTests
{
    [TestClass]
    public class Create_TranslationProvider:BasePEVerifyTest
    {
        public class TranslateFrom
        {
            public int Value { get; set; }
        }

        public class TranslateTo
        {
            public int Value { get; set; }
        }

        private FunctionGenerator.PhysicalAssemblySettings settings;
        private FunctionGenerator functionGenerator;
        private TranslationProvider provider;

        [TestInitialize]
        public void Initialize()
        {
            this.settings = new FunctionGenerator.PhysicalAssemblySettings("Translate", "dll", AppDomain.CurrentDomain.BaseDirectory);

            this.functionGenerator = new FunctionGenerator(this.settings);
            this.provider = new TranslationProvider(this.functionGenerator, shouldThrowExceptions: true);
        }

        [TestMethod]
        [Conditional("PEVERIFY")]
        public void TranslatorBodyPassesPEVerify()
        {
            var translate = this.functionGenerator.Create<TranslateFrom, TranslateTo>(
                writer => this.provider.WriteTranslatorBody<TranslateFrom, TranslateTo>(writer));

            RunPEVerifyOnAssembly(this.settings.FullPath);
        }
    }
}
