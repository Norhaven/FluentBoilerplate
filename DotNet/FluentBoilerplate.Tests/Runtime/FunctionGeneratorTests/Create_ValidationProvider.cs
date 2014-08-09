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
using NUnit.Framework;
using System.Diagnostics;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers.Validation;

namespace FluentBoilerplate.Tests.Runtime.FunctionGeneratorTests
{
    [TestFixture]
    public class Create_ValidationProvider:BasePEVerifyTest
    {
        public class BasicValidatedTest
        {
            public int IntegerValue { get; set; }
            [StringLength(Minimum=2, Maximum=4)]
            public string StringValue { get; set; }
            [NotNull]
            public object ObjectValue { get; set; }
        }

        private FunctionGenerator.PhysicalAssemblySettings settings;
        private FunctionGenerator functionGenerator;
        private ValidationProvider provider;

        [SetUp]
        public void Initialize()
        {
            this.settings = new FunctionGenerator.PhysicalAssemblySettings("Translate", "dll", AppDomain.CurrentDomain.BaseDirectory);

            this.functionGenerator = new FunctionGenerator(this.settings);
            this.provider = new ValidationProvider(this.functionGenerator, shouldThrowExceptions: true);
        }


        [Test]
        [Conditional("PEVERIFY")]
        public void ValidatorBodyPassessPEVerify()
        {            
            var method = this.functionGenerator.Create<BasicValidatedTest, IValidationResult>(
                writer => this.provider.WriteValidatorBody<BasicValidatedTest>(writer));

            RunPEVerifyOnAssembly(this.settings.FullPath);
        }
    }
}
