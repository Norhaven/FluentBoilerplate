using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    [TestClass]
    public class Try
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
