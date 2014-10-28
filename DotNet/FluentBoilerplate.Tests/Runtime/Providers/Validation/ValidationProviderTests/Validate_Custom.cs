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
    public class Validate_Custom
    {
        public class NotNullStringValidator:ICustomValidator<string>
        {
            public bool Validate(string instance)
            {
                if (instance != null) return true;
                else return false;
            }
        }

        public class GoodTypeTest
        {
            [CustomValidation(typeof(NotNullStringValidator))]
            public string Text { get; set; }
        }

        public class IncompatibleTypeTest
        {
            [CustomValidation(typeof(NotNullStringValidator))]
            public long Value { get; set; }
        }

        public class IncorrectValidatorTest
        {
            [CustomValidation(typeof(object))]
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
        public void NonNullStringWithGoodTypeWillPass()
        {
            var instance = new GoodTypeTest { Text = String.Empty };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a custom string validator applies to a string property");
            result.IsSuccess.Should().BeTrue("because the text was not null");
        }

        [Test]
        public void NullStringWithGoodTypeWillFail()
        {
            var instance = new GoodTypeTest { Text = null };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a custom string validator applies to a string property");
            result.IsSuccess.Should().BeFalse("because the text was null");
        }             
        
        [Test]
        [ExpectedException(typeof(IncorrectValidationAttributeException))]
        public void IncompatiblePropertyTypeWillFailWithException()
        {
            var instance = new IncompatibleTypeTest { Value = 5 };
            this.provider.Validate(instance);
        }
        
        [Test]
        [ExpectedException(typeof(IncorrectValidationAttributeException))]
        public void IncorrectValidatorWillThrowException()
        {
            var instance = new IncorrectValidatorTest { Text = String.Empty };
            this.provider.Validate(instance);
        }
    }
}
