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
using System.Configuration;
using System.IO;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers.Translation;
using FluentBoilerplate.Providers;
using System.Diagnostics;
using FluentBoilerplate.Runtime.Providers.Logging;

namespace FluentBoilerplate.Tests.Runtime.FunctionGeneratorTests
{
    [TestFixture]
    public class Create_LogProvider:BasePEVerifyTest
    {
        [Log]
        public class DebugTest
        {
            [Log(Visibility=Visibility.Debug)]
            public string Message = "Hello";
        }
        private FunctionGenerator.PhysicalAssemblySettings settings;
        private FunctionGenerator functionGenerator;
        private LogProvider provider;

        [SetUp]
        public void Initialize()
        {
            this.settings = new FunctionGenerator.PhysicalAssemblySettings("Log", "dll", AppDomain.CurrentDomain.BaseDirectory);

            this.functionGenerator = new FunctionGenerator(this.settings);
            this.provider = new LogProvider(this.functionGenerator, Visibility.Debug | Visibility.Warning);
        }

        [Test]
        [Conditional("PEVERIFY")]
        public void LogProviderBodyPassesPEVerify()
        {
            this.provider.CreateTypeLogger(typeof(DebugTest));

            RunPEVerifyOnAssembly(this.settings.FullPath);
        }
    }
}
