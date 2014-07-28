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
using FluentBoilerplate.Runtime.Contexts;

using FluentAssertions;
using Moq;
using FluentBoilerplate.Contexts;
using System.Collections.Generic;
using System.Collections.Immutable;
using FluentBoilerplate.Runtime.Providers.Logging;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime;

namespace FluentBoilerplate.Tests.Runtime.Contexts.InitialBoilerplateContextTests
{
    [TestClass]
    public class Do
    {
        public class ValidationFailureTest
        {
            [NotNull]
            public string Text { get { return null; } }
        }

        [TestMethod]
        public void WillPerformActionWithNoContract()
        {
            var permissions = new Mock<IPermissionsProvider>(MockBehavior.Strict);
            permissions.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(true);

            var bundle = new Mock<IContextBundle>(MockBehavior.Strict);
            bundle.Setup(x => x.Errors).Returns(() => new ImmutableErrorContext(LogProvider.Empty, TryCatchBlockProvider.Empty, ExceptionHandlerProvider.Empty));
            bundle.Setup(x => x.Permissions).Returns(permissions.Object);
            bundle.Setup(x => x.Copy(It.IsAny<IPermissionsProvider>(),
                                     It.IsAny<IImmutableErrorContext>(),
                                     It.IsAny<ITypeAccessProvider>(),
                                     It.IsAny<ITranslationProvider>(),
                                     It.IsAny<IValidationProvider>())).Returns(() => bundle.Object);

            var contractBundle = new Mock<IContractBundle>(MockBehavior.Strict);
            contractBundle.Setup(x => x.Preconditions).Returns(() => ImmutableQueue<IContractCondition>.Empty);
            contractBundle.Setup(x => x.InstanceValidations).Returns(() => ImmutableQueue<Action>.Empty);
            contractBundle.Setup(x => x.PostconditionsOnReturn).Returns(() => ImmutableQueue<IContractCondition>.Empty);
            

            var context = new InitialBoilerplateContext<ContractContext>(bundle.Object,
                                                                         Identity.Default,
                                                                         contractBundle.Object);
            var success = false;
            context.Do(_ => success = true);

            success.Should().BeTrue("because the action should be performed");
        }

        [TestMethod]
        public void WillPerformActionWithOneExceptionHandler()
        {
            IImmutableErrorContext errorContext = new ImmutableErrorContext(LogProvider.Empty, TryCatchBlockProvider.Empty, ExceptionHandlerProvider.Empty);
            errorContext = errorContext.RegisterExceptionHandler<Exception>(ex => {});

            var permissions = new Mock<IPermissionsProvider>(MockBehavior.Strict);
            permissions.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(true);
            
            var bundle = new Mock<IContextBundle>(MockBehavior.Strict);
            bundle.Setup(x => x.Errors).Returns(() => errorContext);
            bundle.Setup(x => x.Permissions).Returns(permissions.Object);
            bundle.Setup(x => x.Copy(It.IsAny<IPermissionsProvider>(),
                                     It.IsAny<IImmutableErrorContext>(),
                                     It.IsAny<ITypeAccessProvider>(),
                                     It.IsAny<ITranslationProvider>(),
                                     It.IsAny<IValidationProvider>())).Returns(() => bundle.Object);

            var contractBundle = new Mock<IContractBundle>(MockBehavior.Strict);
            contractBundle.Setup(x => x.Preconditions).Returns(() => ImmutableQueue<IContractCondition>.Empty);
            contractBundle.Setup(x => x.InstanceValidations).Returns(() => ImmutableQueue<Action>.Empty);
            contractBundle.Setup(x => x.PostconditionsOnReturn).Returns(() => ImmutableQueue<IContractCondition>.Empty);


            var context = new InitialBoilerplateContext<ContractContext>(bundle.Object,
                                                                         Identity.Default,
                                                                         contractBundle.Object);
            var success = false;
            context.Do(_ => success = true);

            success.Should().BeTrue("because the action should be performed");
        }

        [TestMethod]
        [ExpectedException(typeof(ContractViolationException))]
        public void WillNotPerformActionWithFailedRequirement()
        {
            IImmutableErrorContext errorContext = new ImmutableErrorContext(LogProvider.Empty, TryCatchBlockProvider.Empty, ExceptionHandlerProvider.Empty);
            errorContext = errorContext.RegisterExceptionHandler<Exception>(ex => { });

            var permissions = new Mock<IPermissionsProvider>(MockBehavior.Strict);
            permissions.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(true);

            var bundle = new Mock<IContextBundle>(MockBehavior.Strict);
            bundle.Setup(x => x.Errors).Returns(() => errorContext);
            bundle.Setup(x => x.Permissions).Returns(permissions.Object);
            bundle.Setup(x => x.Copy(It.IsAny<IPermissionsProvider>(),
                                     It.IsAny<IImmutableErrorContext>(),
                                     It.IsAny<ITypeAccessProvider>(),
                                     It.IsAny<ITranslationProvider>(),
                                     It.IsAny<IValidationProvider>())).Returns(() => bundle.Object);

            Func<bool> precondition = () => false;
            var condition = new DefaultContractCondition(precondition, String.Empty);
            var preconditions = ImmutableQueue<IContractCondition>.Empty.Enqueue(condition);

            var contractBundle = new Mock<IContractBundle>(MockBehavior.Strict);
            contractBundle.Setup(x => x.Preconditions).Returns(() => preconditions);
            contractBundle.Setup(x => x.InstanceValidations).Returns(() => ImmutableQueue<Action>.Empty);
            contractBundle.Setup(x => x.PostconditionsOnReturn).Returns(() => ImmutableQueue<IContractCondition>.Empty);
            
            var context = new InitialBoilerplateContext<ContractContext>(bundle.Object,
                                                                         Identity.Default,
                                                                         contractBundle.Object);
            context.Do(_ => { });
        }

        [TestMethod]
        public void WillPerformActionWithPassedRequirement()
        {
            IImmutableErrorContext errorContext = new ImmutableErrorContext(LogProvider.Empty, TryCatchBlockProvider.Empty, ExceptionHandlerProvider.Empty);
            errorContext = errorContext.RegisterExceptionHandler<Exception>(ex => { });
            
            var permissions = new Mock<IPermissionsProvider>(MockBehavior.Strict);
            permissions.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(true);
            
            var bundle = new Mock<IContextBundle>(MockBehavior.Strict);
            bundle.Setup(x => x.Errors).Returns(() => errorContext);
            bundle.Setup(x => x.Permissions).Returns(permissions.Object);
            bundle.Setup(x => x.Copy(It.IsAny<IPermissionsProvider>(),
                                     It.IsAny<IImmutableErrorContext>(),
                                     It.IsAny<ITypeAccessProvider>(),
                                     It.IsAny<ITranslationProvider>(),
                                     It.IsAny<IValidationProvider>())).Returns(() => bundle.Object);

            Func<bool> precondition = () => true;
            var condition = new DefaultContractCondition(precondition, String.Empty);
            var preconditions = ImmutableQueue<IContractCondition>.Empty.Enqueue(condition);

            var contractBundle = new Mock<IContractBundle>(MockBehavior.Strict);
            contractBundle.Setup(x => x.Preconditions).Returns(() => preconditions);
            contractBundle.Setup(x => x.InstanceValidations).Returns(() => ImmutableQueue<Action>.Empty);
            contractBundle.Setup(x => x.PostconditionsOnReturn).Returns(() => ImmutableQueue<IContractCondition>.Empty);

            var context = new InitialBoilerplateContext<ContractContext>(bundle.Object,
                                                                         Identity.Default,
                                                                         contractBundle.Object);
            var success = false;
            context.Do(_ => success = true);

            success.Should().BeTrue("because the requirement should have passed");
        }

        [TestMethod]
        [ExpectedException(typeof(ContractViolationException))]
        public void WillNotPerformActionWithFailedValidation()
        {
            IImmutableErrorContext errorContext = new ImmutableErrorContext(LogProvider.Empty, TryCatchBlockProvider.Empty, ExceptionHandlerProvider.Empty);
            errorContext = errorContext.RegisterExceptionHandler<Exception>(ex => { });
            
            var permissions = new Mock<IPermissionsProvider>(MockBehavior.Strict);
            permissions.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(true);
            
            var bundle = new Mock<IContextBundle>(MockBehavior.Strict);
            bundle.Setup(x => x.Errors).Returns(() => errorContext);
            bundle.Setup(x => x.Permissions).Returns(permissions.Object);
            bundle.Setup(x => x.Copy(It.IsAny<IPermissionsProvider>(),
                                     It.IsAny<IImmutableErrorContext>(),
                                     It.IsAny<ITypeAccessProvider>(),
                                     It.IsAny<ITranslationProvider>(),
                                     It.IsAny<IValidationProvider>())).Returns(() => bundle.Object);

            var validations = ImmutableQueue<Action>.Empty.Enqueue(() => { throw new ContractViolationException(String.Empty); });

            var contractBundle = new Mock<IContractBundle>(MockBehavior.Strict);
            contractBundle.Setup(x => x.Preconditions).Returns(() => ImmutableQueue<IContractCondition>.Empty);
            contractBundle.Setup(x => x.InstanceValidations).Returns(() => validations);
            contractBundle.Setup(x => x.PostconditionsOnReturn).Returns(() => ImmutableQueue<IContractCondition>.Empty);

            var context = new InitialBoilerplateContext<ContractContext>(bundle.Object,
                                                                         Identity.Default,
                                                                         contractBundle.Object);
            context.Do(_ => { });
        }

        [TestMethod]
        public void WillPerformActionWithPassedValidation()
        {
            IImmutableErrorContext errorContext = new ImmutableErrorContext(LogProvider.Empty, TryCatchBlockProvider.Empty, ExceptionHandlerProvider.Empty);
            errorContext = errorContext.RegisterExceptionHandler<Exception>(ex => { });

            var permissions = new Mock<IPermissionsProvider>(MockBehavior.Strict);
            permissions.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(true);

            var bundle = new Mock<IContextBundle>(MockBehavior.Strict);
            bundle.Setup(x => x.Errors).Returns(() => errorContext);
            bundle.Setup(x => x.Permissions).Returns(permissions.Object);

            bundle.Setup(x => x.Copy(It.IsAny<IPermissionsProvider>(),
                                     It.IsAny<IImmutableErrorContext>(),
                                     It.IsAny<ITypeAccessProvider>(),
                                     It.IsAny<ITranslationProvider>(),
                                     It.IsAny<IValidationProvider>())).Returns(() => bundle.Object);

            var validations = ImmutableQueue<Action>.Empty.Enqueue(() => { });

            var contractBundle = new Mock<IContractBundle>(MockBehavior.Strict);
            contractBundle.Setup(x => x.Preconditions).Returns(() => ImmutableQueue<IContractCondition>.Empty);
            contractBundle.Setup(x => x.InstanceValidations).Returns(() => validations);
            contractBundle.Setup(x => x.PostconditionsOnReturn).Returns(() => ImmutableQueue<IContractCondition>.Empty);

            var context = new InitialBoilerplateContext<ContractContext>(bundle.Object,
                                                                         Identity.Default,
                                                                         contractBundle.Object);
            var success = false;
            context.Do(_ => success = true);

            success.Should().BeTrue("because the validation should have passed");
        }
    }
}
