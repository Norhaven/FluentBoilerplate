using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime.Providers.Translation;

using FluentAssertions;
using FluentBoilerplate.Runtime;
using System.Configuration;
using System.IO;

namespace FluentBoilerplate.Tests.Runtime.Providers.Translation.TranslationProviderTests
{
    [TestClass]
    public class Translate : BasePEVerifyTest
    {
        public class DefaultType
        {
            public int Value { get; set; }
        }
        public class ByNameImplicitInteger
        {
            public int Value { get; set; }
        }
        public class ByNameImplicitLong
        {
            public long Value { get; set; }
        }
        public class ByNameImplicitShort
        {
            public short Value { get; set; }
        }
        public class ByNameImplicitString
        {
            public string Value { get; set; }
        }
        public class ByNameExplicitIntegerFrom
        {
            [MapsTo(typeof(ByNameExplicitIntegerTo), "DifferentValue")]
            public int Value { get; set; }
        }
        public class ByNameExplicitIntegerTo
        {
            public int DifferentValue { get; set; }
        }
        public class ByNameExplicitWideningFrom
        {
            [MapsTo(typeof(ByNameExplicitWideningTo), "DifferentValue")]
            public int Value { get; set; }
        }
        public class ByNameExplicitWideningTo
        {
            public long DifferentValue { get; set; }
        }
        public class ByNameExplicitNarrowingFrom
        {
            [MapsTo(typeof(ByNameExplicitNarrowingTo), "DifferentValue")]
            public int Value { get; set; }
        }
        public class ByNameExplicitNarrowingTo
        {
            public short DifferentValue { get; set; }
        }
        public class ByNameExplicitIncompatibleFrom
        {
            [MapsTo(typeof(ByNameExplicitIncompatibleTo), "DifferentValue")]
            public int Value { get; set; }
        }
        public class ByNameExplicitIncompatibleTo
        {
            public string DifferentValue { get; set; }
        }

        private ITranslationProvider provider;

        [TestInitialize]
        public void Initialize()
        {
            var functionGenerator = new FunctionGenerator();
            this.provider = new TranslationProvider(functionGenerator, shouldThrowExceptions: true);
        }

        [TestMethod]
        public void NullToClassWillSucceed()
        {
            var result = this.provider.Translate<object, object>(null);
            result.Should().BeNull("because a null instance becomes a null class instance");
        }
        
        [TestMethod]
        public void NullToInterfaceWillSucceed()
        {
            var result = this.provider.Translate<object, ITranslationProvider>(null);
            result.Should().BeNull("because a null instance becomes a null interface instance");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullToStructWillNotSucceed()
        {
            this.provider.Translate<object, int>(null);
        }

        [TestMethod]
        public void TypeToSameTypeWillSucceed()
        {
            var from = 5;
            var result = this.provider.Translate<int, int>(from);
            result.Should().Be(5, "because that's the original value");
        }

        [TestMethod]
        public void ByNameImplicitWillSucceed()
        {
            var instance = new ByNameImplicitInteger() { Value = 5 };
            var result = this.provider.Translate<ByNameImplicitInteger, DefaultType>(instance);
            result.Should().NotBeNull("because the translation should be valid");
            result.Value.Should().Be(5, "because that's the original value");
        }

        [TestMethod]
        public void ByNameImplicitWideningConversionWillSucceed()
        {
            var instance = new ByNameImplicitInteger() { Value = 5 };
            var result = this.provider.Translate<ByNameImplicitInteger, ByNameImplicitLong>(instance);
            result.Should().NotBeNull("because the translation should be valid");
            result.Value.Should().Be(5, "because that's the original value");
        }

        [TestMethod]
        [ExpectedException(typeof(PropertyTypeMismatchException))]
        public void ByNameImplicitNarrowingConversionWillNotSucceed()
        {
            var instance = new ByNameImplicitInteger() { Value = 5 };
            this.provider.Translate<ByNameImplicitInteger, ByNameImplicitShort>(instance);
        }

        [TestMethod]
        [ExpectedException(typeof(PropertyTypeMismatchException))]
        public void ByNameImplicitIncompatibleTypesWillNotSucceed()
        {
            var instance = new ByNameImplicitInteger() { Value = 5 };
            this.provider.Translate<ByNameImplicitInteger, ByNameImplicitString>(instance);
        }

        [TestMethod]
        public void ByNameExplicitWideningConversionWillSucceed()
        {
            var instance = new ByNameExplicitWideningFrom() { Value = 5 };
            var result = this.provider.Translate<ByNameExplicitWideningFrom, ByNameExplicitWideningTo>(instance);
            result.Should().NotBeNull("because the translation should be valid");
            result.DifferentValue.Should().Be(5, "because that's the original value");
        }

        [TestMethod]
        [ExpectedException(typeof(PropertyTypeMismatchException))]
        public void ByNameExplicitNarrowingConversionWillNotSucceed()
        {
            var instance = new ByNameExplicitNarrowingFrom() { Value = 5 };
            this.provider.Translate<ByNameExplicitNarrowingFrom, ByNameExplicitNarrowingTo>(instance);
        }

        [TestMethod]
        [ExpectedException(typeof(PropertyTypeMismatchException))]
        public void ByNameExplicitIncompatibleTypesWillNotSucceed()
        {
            var instance = new ByNameExplicitIncompatibleFrom() { Value = 5 };
            this.provider.Translate<ByNameExplicitIncompatibleFrom, ByNameExplicitIncompatibleTo>(instance);
        }
    }
}
