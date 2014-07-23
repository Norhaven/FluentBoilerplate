using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers.Validation;

namespace FluentBoilerplate.Tests.Runtime.FunctionGeneratorTests
{
    [TestClass]
    public class Create_ValidationProvider:BasePEVerifyTest
    {
        public class BasicValidatedTest
        {
            public int IntegerValue { get; set; }
            [StringLength(MinLength=2, MaxLength=4)]
            public string StringValue { get; set; }
            [NotNull]
            public object ObjectValue { get; set; }
        }

        private FunctionGenerator.PhysicalAssemblySettings settings;
        private FunctionGenerator functionGenerator;
        private ValidationProvider provider;

        [TestInitialize]
        public void Initialize()
        {
            this.settings = new FunctionGenerator.PhysicalAssemblySettings("Translate", "dll", AppDomain.CurrentDomain.BaseDirectory);

            this.functionGenerator = new FunctionGenerator(this.settings);
            this.provider = new ValidationProvider(this.functionGenerator, shouldThrowExceptions: true);
        }


        [TestMethod]
        [Conditional("PEVERIFY")]
        public void ValidatorBodyPassessPEVerify()
        {            
            var method = this.functionGenerator.Create<BasicValidatedTest, IValidationResult>(
                writer => this.provider.WriteValidatorBody<BasicValidatedTest>(writer));

            RunPEVerifyOnAssembly(this.settings.FullPath);
        }
    }
}
