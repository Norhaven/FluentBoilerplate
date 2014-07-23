using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Providers.Validation;
using FluentBoilerplate.Runtime;
using FluentAssertions;

namespace FluentBoilerplate.Tests.Runtime.Providers.Validation.ValidationProviderTests
{
    [TestClass]
    public class Validate_StringLength
    {
        public class MinLengthTest
        {
            [StringLength(MinLength = 2)]
            public string Value { get; set; }
        }

        public class MaxLengthTest
        {
            [StringLength(MaxLength = 4)]
            public string Value { get; set; }
        }

        public class IncompatibleTest
        {
            [StringLength(MinLength = 2)]
            public int Value { get; set; }
        }

        private IValidationProvider provider;

        [TestInitialize]
        public void Initialize()
        {
            var settings = new FunctionGenerator.PhysicalAssemblySettings("Test", "dll", AppDomain.CurrentDomain.BaseDirectory);
            var functionGenerator = new FunctionGenerator(settings);
            this.provider = new ValidationProvider(functionGenerator, shouldThrowExceptions: true);
        }

        [TestMethod]
        public void StringUnderMinLengthWillFail()
        {
            var instance = new MinLengthTest { Value = "a" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a string length attribute applies to a string property");
            result.IsSuccess.Should().BeFalse("because the string is less than the minimum length");
        }

        [TestMethod]
        public void StringAboveMaxLengthWillSucceedWhenMaxIsZero()
        {
            var instance = new MaxLengthTest { Value = "aaaaa" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a string length attribute applies to a string property");
            result.IsSuccess.Should().BeFalse("because the string is greater than the maximum length");
        }

        [TestMethod]
        public void StringAboveMaxLengthWillFail()
        {
            var instance = new MaxLengthTest { Value = "aaaaa" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a string length attribute applies to a string property");
            result.IsSuccess.Should().BeFalse("because the string is greater than the maximum length");
        }

        [TestMethod]
        public void StringWithinMinimumAndMaximumRangeWillSucceed()
        {
            var instance = new MinLengthTest { Value = "aaaa" };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeTrue("because a string length attribute applies to a string property");
            result.IsSuccess.Should().BeTrue("because the string is within the min/max bounds");
        }

        [TestMethod]
        public void IncompatiblePropertyTypeWillReceiveNotApplicableResult()
        {
            var instance = new IncompatibleTest { Value = 5 };
            var result = this.provider.Validate(instance);

            result.Should().NotBeNull("because we should always get a result");
            result.IsApplicable.Should().BeFalse("because this is not a string property");
            result.IsSuccess.Should().BeFalse("because it could not be validated");
       
        }
    }
}
