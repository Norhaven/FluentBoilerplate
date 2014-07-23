using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Runtime.Providers.Logging;
using FluentBoilerplate.Runtime;
using System.Diagnostics;
using System.Text;
using FluentAssertions;

namespace FluentBoilerplate.Tests.Runtime.Providers.Logging.LogProviderTests
{
    [TestClass]
    public class Debug
    {
        [Log]
        public class DebugTest
        {
            public string NonLoggedValue { get; set; }

            [Log(Visibility=LogVisibility.Debug)]
            public string Value { get; set; }
        }
        
        public class TestTraceListener:TraceListener
        {
            private string expectedOutput;
            private string expectedCategory;
            private StringBuilder currentOutput = new StringBuilder();
            private StringBuilder currentCategory = new StringBuilder();

            public bool ReadTrace { get; private set; }

            public void ExpectCategory(string category)
            {
                this.expectedCategory = category;
            }
            public void ExpectOutput(string output)
            {
                this.expectedOutput = output;
            }

            public override void Write(string message, string category)
            {
                throw new NotImplementedException();
            }

            public override void Flush()
            {
                this.expectedOutput.Should().Be(this.currentOutput.ToString(), "because that's the output we were expecting");
                this.expectedCategory.Should().Be(this.currentCategory.ToString(), "because that's the category we were expecting");
                base.Flush();
            }

            public override void Write(string message)
            {
                throw new NotImplementedException();
            }
            public override void WriteLine(string message, string category)
            {
                this.ReadTrace = true;
                this.currentOutput.AppendLine(message);
                this.currentCategory.AppendLine(category);
                base.WriteLine(message, category);
            }
            public override void WriteLine(string message)
            {
                
            }
        }

        private LogProvider provider;
        private IFunctionGenerator generator;

        [TestInitialize]
        public void Initialize()
        {
            this.generator = new FunctionGenerator();
            this.provider = new LogProvider(this.generator, LogVisibility.Debug);
        }

        [TestMethod]
        public void LoggedTypeWithLogDebugWillWriteToLog()
        {
            var listener = new TestTraceListener();
            listener.ExpectCategory("[DEBUG]");
            listener.ExpectOutput("[DEBUG] FluentBoilerplate.Tests.Runtime.Providers.Logging.LogProviderTests+Debug.DebugTest = System.String \"Test Value\"");

            Trace.Listeners.Add(listener);
            var instance = new DebugTest
            {
                Value = "TestValue"
            };

            this.provider.Debug("Test message", instance);

            listener.ReadTrace.Should().BeTrue("because we should have received a log message");
        }
    }
}
