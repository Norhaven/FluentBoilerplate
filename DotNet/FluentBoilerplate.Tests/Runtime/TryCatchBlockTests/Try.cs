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
using Moq;
using FluentAssertions;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime;
using System.Collections.Generic;

using System.Collections.Immutable;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;

namespace FluentBoilerplate.Tests.Runtime.TryCatchBlockTests
{
    [TestFixture]
    public class Try
    {
        [Test]
        public void ThrownExceptionWillBeHandled()
        {
            var message = "Error message";

            var handledExceptionTypes = ImmutableQueue<Type>.Empty.Enqueue(typeof(Exception));

            var handler = new Mock<IExceptionHandler<Exception>>(MockBehavior.Strict);
            handler.Setup(x => x.Handle(It.Is<Exception>(p => p.Message == message)));

            var handlerProvider = new Mock<IExceptionHandlerProvider>(MockBehavior.Strict);
            handlerProvider.Setup(x => x.TryGetHandler<Exception>()).Returns(handler.Object);
            handlerProvider.Setup(x => x.HandledTypesInCatchOrder).Returns(handledExceptionTypes);
            handlerProvider.Setup(x => x.HandledExceptionTypes).Returns(handledExceptionTypes.ToImmutableHashSet());

            var generator = new FunctionGenerator();
            var provider = new TryCatchBlockProvider(generator);
            var tryCatch = provider.GetTryCatchFor(handlerProvider.Object);

            tryCatch.Try(() => { throw new Exception(message); });
        }

        [Test]
        public void ThrownArgumentExceptionWithTwoBlocksWillBeHandled()
        {
            var message = "Error message";

            var handledExceptionTypes = ImmutableQueue<Type>.Empty
                .Enqueue(typeof(ArgumentException))
                .Enqueue(typeof(Exception));

            var handler = new Mock<IExceptionHandler<ArgumentException>>(MockBehavior.Strict);
            handler.Setup(x => x.Handle(It.Is<ArgumentException>(p => p.Message == message)));

            var handlerProvider = new Mock<IExceptionHandlerProvider>(MockBehavior.Strict);
            handlerProvider.Setup(x => x.TryGetHandler<ArgumentException>()).Returns(handler.Object);
            handlerProvider.Setup(x => x.HandledTypesInCatchOrder).Returns(handledExceptionTypes);
            handlerProvider.Setup(x => x.HandledExceptionTypes).Returns(handledExceptionTypes.ToImmutableHashSet());

            var generator = new FunctionGenerator();
            var provider = new TryCatchBlockProvider(generator);
            var tryCatch = provider.GetTryCatchFor(handlerProvider.Object);

            tryCatch.Try(() => { throw new ArgumentException(message); });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrownArgumentNullExceptionWithArgumentExceptionBlockWillNotHandled()
        {
            var message = "Error message";

            var handledExceptionTypes = ImmutableQueue<Type>.Empty
                .Enqueue(typeof(ArgumentNullException));

            var handler = new Mock<IExceptionHandler<ArgumentNullException>>(MockBehavior.Strict);

            var handlerProvider = new Mock<IExceptionHandlerProvider>(MockBehavior.Strict);            
            handlerProvider.Setup(x => x.HandledTypesInCatchOrder).Returns(handledExceptionTypes);
            handlerProvider.Setup(x => x.HandledExceptionTypes).Returns(handledExceptionTypes.ToImmutableHashSet());

            var generator = new FunctionGenerator();
            var provider = new TryCatchBlockProvider(generator);
            var tryCatch = provider.GetTryCatchFor(handlerProvider.Object);

            tryCatch.Try(() => { throw new ArgumentException(message); });
        }

        [Test]
        public void BlocksAreWrittenInTheCorrectOrder()
        {
            var message = "Error message";

            var handledExceptionTypes = ImmutableQueue<Type>.Empty
                .Enqueue(typeof(Exception))
                .Enqueue(typeof(ArgumentException));

            var handler = new Mock<IExceptionHandler<Exception>>(MockBehavior.Strict);
            handler.Setup(x => x.Handle(It.IsAny<Exception>()));

            var handlerProvider = new Mock<IExceptionHandlerProvider>(MockBehavior.Strict);
            handlerProvider.Setup(x => x.TryGetHandler<Exception>()).Returns(handler.Object);
            handlerProvider.Setup(x => x.HandledTypesInCatchOrder).Returns(handledExceptionTypes);
            handlerProvider.Setup(x => x.HandledExceptionTypes).Returns(handledExceptionTypes.ToImmutableHashSet());

            var generator = new FunctionGenerator();
            var provider = new TryCatchBlockProvider(generator);
            var tryCatch = provider.GetTryCatchFor(handlerProvider.Object);

            tryCatch.Try(() => { throw new ArgumentException(message); });
        }

        [Test]
        public void ExceptionHandlerCanReturnResult()
        {
            var message = "Error message";

            var handledExceptionTypes = ImmutableQueue<Type>.Empty
                .Enqueue(typeof(Exception));

            var handler = new Mock<IExceptionHandler<Exception, int>>(MockBehavior.Strict);
            handler.Setup(x => x.HandleWithResult(It.Is<Exception>(p => p.Message == message))).Returns(5);

            var handlerProvider = new Mock<IExceptionHandlerProvider>(MockBehavior.Strict);
            handlerProvider.Setup(x => x.TryGetHandler<Exception, int>()).Returns(handler.Object);
            handlerProvider.Setup(x => x.HandledTypesInCatchOrder).Returns(handledExceptionTypes);
            handlerProvider.Setup(x => x.HandledExceptionTypes).Returns(handledExceptionTypes.ToImmutableHashSet());

            var generator = new FunctionGenerator();
            var provider = new TryCatchBlockProvider(generator);
            var tryCatch = provider.GetTryCatchFor(handlerProvider.Object);

            var result = tryCatch.Try<int>(() => { throw new Exception(message); });

            result.Should().Be(5, "because that's what the handler returned");
        }

        [Test]
        public void TryBlockBodyCanReturnResult()
        {
            var handledExceptionTypes = ImmutableQueue<Type>.Empty
                .Enqueue(typeof(Exception));

            var handler = new Mock<IExceptionHandler<Exception, int>>(MockBehavior.Strict);

            var handlerProvider = new Mock<IExceptionHandlerProvider>(MockBehavior.Strict);
            handlerProvider.Setup(x => x.HandledTypesInCatchOrder).Returns(handledExceptionTypes);
            handlerProvider.Setup(x => x.HandledExceptionTypes).Returns(handledExceptionTypes.ToImmutableHashSet());

            var generator = new FunctionGenerator();
            var provider = new TryCatchBlockProvider(generator);
            var tryCatch = provider.GetTryCatchFor(handlerProvider.Object);

            var result = tryCatch.Try<int>(() => 5);

            result.Should().Be(5, "because that's what the handler returned");
        }
    }
}
