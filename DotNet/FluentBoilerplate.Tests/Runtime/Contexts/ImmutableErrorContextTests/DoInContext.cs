using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Runtime.Contexts;
using FluentBoilerplate.Runtime.Providers.Logging;
using FluentBoilerplate.Providers;
using Moq;
using FluentAssertions;
using FluentBoilerplate.Contexts;

namespace FluentBoilerplate.Tests.Runtime.Contexts.ImmutableErrorContextTests
{
    [TestClass]
    public class DoInContext
    {
        [TestMethod]
        public void WillPassWhenNoHandlersArePresent()
        {
            var success = false;
            var tryCatch = Mock.Strict<ITryCatchBlock>();
            tryCatch.Setup(x => x.Try(It.IsAny<Action>())).Callback<Action>(action => action());
                        
            var tryCatchProvider = Mock.Strict<ITryCatchBlockProvider>();
            tryCatchProvider.Setup(x => x.GetTryCatchFor(It.IsAny<IExceptionHandlerProvider>())).Returns(tryCatch.Object);

            var handlerProvider = Mock.Strict<IExceptionHandlerProvider>();
            
            var error = new ImmutableErrorContext(LogProvider.Empty, tryCatchProvider.Object, handlerProvider.Object);

            error.DoInContext(context => { success = true; });

            success.Should().BeTrue("because the action should have been run");
        }
    }
}
