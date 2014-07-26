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
