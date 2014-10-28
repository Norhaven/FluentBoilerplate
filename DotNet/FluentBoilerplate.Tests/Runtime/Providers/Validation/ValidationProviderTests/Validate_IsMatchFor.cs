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
    public class Validate_IsMatchFor
    {
        public class EmptyExpressionTest
        {
            [IsMatchFor("")]
            public string Text { get; set; }
        }

        public class NullExpressionTest
        {
            [IsMatchFor(null)]
            public string Text { get; set; }
        }

        public class DigitsOnlyExpressionTest
        {
            [IsMatchFor(@"\d+")]
            public string Text { get; set; }
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
        public void EmptyExpressionWillSucceedAtAllTimes()
        {
            var instance = new EmptyExpressionTest { Text = "hi" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a regular expression attribute applies to a string property");
            result.IsSuccess.Should().BeTrue("because an empty regular expression does not use any regular expressions");
        }

        [Test]
        public void NullExpressionWillBeTreatedAsEmptyExpression()
        {
            var instance = new NullExpressionTest { Text = "hi" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a regular expression attribute applies to a string property");
            result.IsSuccess.Should().BeTrue("because a null regular expression does not use any regular expressions");
        }             
        
        [Test]
        public void DigitsOnlyExpressionWillSucceedWhenOnlyDigitsArePresent()
        {
            var instance = new DigitsOnlyExpressionTest { Text = "1234" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a regular expression attribute applies to a string property");
            result.IsSuccess.Should().BeTrue("because the text contains only digits");
        }

        [Test]
        public void DigitsOnlyExpressionWillFailWhenNonDigitCharactersArePresent()
        {
            var instance = new DigitsOnlyExpressionTest { Text = "a" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a regular expression attribute applies to a string property");
            result.IsSuccess.Should().BeFalse("because the text contains non-digit characters");
        }
    }
}
