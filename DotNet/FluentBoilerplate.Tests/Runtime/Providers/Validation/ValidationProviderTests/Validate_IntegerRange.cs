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
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Providers.Validation;
using FluentBoilerplate.Runtime;
using FluentAssertions;

namespace FluentBoilerplate.Tests.Runtime.Providers.Validation.ValidationProviderTests
{
    [TestFixture]
    public class Validate_IntegerRange
    {
        public class MinLengthTest
        {
            [IntegerRange(Minimum = 2)]
            public long Value { get; set; }
        }

        public class MaxLengthTest
        {
            [IntegerRange(Maximum = 4)]
            public long Value { get; set; }
        }

        public class IncompatibleTest
        {
            [IntegerRange(Minimum = 2)]
            public string Value { get; set; }
        }

        public class ConvertibleTest
        {
            [IntegerRange(Minimum = 2, Maximum = 4)]
            public int Value { get; set; }
        }

        public class MinimumGreaterThanMaximumTest
        {
            [IntegerRange(Minimum = 4, Maximum = 2)]
            public long Value { get; set; }
        }

        private IValidationProvider provider;

        [SetUp]
        public void Initialize()
        {
            var settings = new FunctionGenerator.PhysicalAssemblySettings("Test", "dll", AppDomain.CurrentDomain.BaseDirectory);
            var functionGenerator = new FunctionGenerator(settings);
            this.provider = new ValidationProvider(functionGenerator, shouldThrowExceptions: true);
        }

        [Test]
        public void Int64UnderMinValueWillFail()
        {
            var instance = new MinLengthTest { Value = 1 };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because an integer range attribute applies to an Int64 property");
            result.IsSuccess.Should().BeFalse("because the Int64 value is less than the minimum value");
        }

        [Test]
        public void Int64AboveMaxValueWillFail()
        {
            var instance = new MaxLengthTest { Value = 5 };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because an integer range attribute applies to an Int64 property");
            result.IsSuccess.Should().BeFalse("because the Int64 value is greater than the maximum value");
        }             
        
        [Test]
        public void IncompatiblePropertyTypeWillReceiveNotApplicableResult()
        {
            var instance = new IncompatibleTest { Value = "" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeFalse("because this is not an integer property");
            result.IsSuccess.Should().BeFalse("because it could not be validated");
        }

        [Test]
        public void ConvertiblePropertyTypeWillSucceed()
        {
            var instance = new ConvertibleTest { Value = 3 };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because an integer range attribute applies to an Int32 property");
            result.IsSuccess.Should().BeTrue("because the value is within the range");
        }

        [Test]
        public void ConvertiblePropertyTypeBelowMinimumWillFail()
        {
            var instance = new ConvertibleTest { Value = 1 };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because an integer range attribute applies to an Int32 property");
            result.IsSuccess.Should().BeFalse("because the value is below the minimum value");
        }

        [Test]
        [ExpectedException(typeof(IncorrectValidationAttributeException))]
        public void MinimumAboveMaximumWillFailWithException()
        {
            var instance = new MinimumGreaterThanMaximumTest { Value = 1 };
            this.provider.Validate(instance);
        }
    }
}
