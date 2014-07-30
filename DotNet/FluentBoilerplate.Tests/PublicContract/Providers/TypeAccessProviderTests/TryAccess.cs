using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Runtime.Contexts;
using System.Collections.Generic;
using System.Collections.Immutable;
using FluentBoilerplate.Providers;
using FluentAssertions;
using Moq;

namespace FluentBoilerplate.Tests.PublicContract.Providers.TypeAccessProviderTests
{
    [TestClass]
    public class TryAccess
    {       
        public class TestTypeAccessProvider:TypeAccessProvider
        {
            public TestTypeAccessProvider(IPermissionsProvider provider, IEnumerable<Type> types)
                : base(provider, types)
            {
            }

            protected override TType CreateInstanceOf<TType>()
            {
                if (typeof(TType) == typeof(int))
                    return (TType)(object)1;
                return default(TType);
            }
        }

        #region TryAccess<TType>

        [TestMethod]
        public void HasProviderWithRequiredPermissionsButNoIdentityPermissionsWillFail()
        {
            var permissionsProvider = Mock.Strict<IPermissionsProvider>();
            permissionsProvider.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(false);

            var typeAccessProvider = new TestTypeAccessProvider(permissionsProvider.Object, new[] { typeof(int) });
            var success = false;
            var response = typeAccessProvider.TryAccess<int>(Identity.Default, value => { success = true; });

            success.Should().BeFalse("because the identity didn't have permission");
            response.Should().NotBeNull("because a null response should never happen");
            response.IsSuccess.Should().BeFalse("because the identity didn't have permission");
            response.Result.Should().BeNull("because there isn't a result code associated with this");
        }

        [TestMethod]
        public void HasNoProviderAndTypeIsMissingWillFail()
        {
            var typeAccessProvider = new TestTypeAccessProvider(null, Type.EmptyTypes);
            var success = false;
            var response = typeAccessProvider.TryAccess<int>(Identity.Default, value => { success = true; });

            success.Should().BeFalse("because the type was not known to the provider");
            response.Should().NotBeNull("because a null response should never happen");
            response.IsSuccess.Should().BeFalse("because the type was not known to the provider");
            response.Result.Should().BeNull("because there isn't a result code associated with this");
        }

        [TestMethod]
        public void HasProviderWithRequiredPermissionsAndIdentityHasRequiredPermissionsWillPass()
        {
            var permissionsProvider = Mock.Strict<IPermissionsProvider>();
            permissionsProvider.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(true);

            var typeAccessProvider = new TestTypeAccessProvider(permissionsProvider.Object, new[] { typeof(int) });
            var success = false;
            var response = typeAccessProvider.TryAccess<int>(Identity.Default, value => { success = true; });

            success.Should().BeTrue("because the permissions and known type were correct");
            response.Should().NotBeNull("because a null response should never happen");
            response.IsSuccess.Should().BeTrue("because the permissions and known type were correct");
            response.Result.Should().BeNull("because there isn't a result code associated with this");
        }

        #endregion TryAccess<TType>

        #region TryAccess<TType, TResult>

        [TestMethod]
        public void Result_HasProviderWithRequiredPermissionsButNoIdentityPermissionsWillFail()
        {
            var permissionsProvider = Mock.Strict<IPermissionsProvider>();
            permissionsProvider.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(false);

            var typeAccessProvider = new TestTypeAccessProvider(permissionsProvider.Object, new[] { typeof(int) });           
            var response = typeAccessProvider.TryAccess<int, bool>(Identity.Default, value => true);

            response.Should().NotBeNull("because a null response should never happen");
            response.Content.Should().BeFalse("because a value was never returned");
            response.IsSuccess.Should().BeFalse("because the identity didn't have permission");
            response.Result.Should().BeNull("because there isn't a result code associated with this");
        }

        [TestMethod]
        public void Result_HasNoProviderAndTypeIsMissingWillFail()
        {
            var typeAccessProvider = new TestTypeAccessProvider(null, Type.EmptyTypes);
            var response = typeAccessProvider.TryAccess<int, bool>(Identity.Default, value => true);
            
            response.Should().NotBeNull("because a null response should never happen");
            response.Content.Should().BeFalse("because a value was never returned");
            response.IsSuccess.Should().BeFalse("because the the type wasn't know to the provider");
            response.Result.Should().BeNull("because there isn't a result code associated with this");
        }

        [TestMethod]
        public void Result_HasProviderWithRequiredPermissionsAndIdentityHasRequiredPermissionsWillPass()
        {
            var permissionsProvider = Mock.Strict<IPermissionsProvider>();
            permissionsProvider.Setup(x => x.HasPermission(It.IsAny<IIdentity>())).Returns(true);

            var typeAccessProvider = new TestTypeAccessProvider(permissionsProvider.Object, new[] { typeof(int) });
            var response = typeAccessProvider.TryAccess<int, bool>(Identity.Default, value => true);

            response.Should().NotBeNull("because a null response should never happen");
            response.IsSuccess.Should().BeTrue("because the permissions and known type were correct");
            response.Result.Should().BeNull("because there isn't a result code associated with this");
        }

        #endregion TryAccess<TType, TResult>
    }
}
