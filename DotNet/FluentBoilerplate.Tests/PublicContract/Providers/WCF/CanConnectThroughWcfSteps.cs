using FluentBoilerplate.Providers;
using FluentBoilerplate.Providers.WCF;
using FluentBoilerplate.Runtime.Providers;
using System;
using System.ServiceModel;
using TechTalk.SpecFlow;
using FluentAssertions;

namespace FluentBoilerplate.Tests.PublicContract.Providers.WCF
{
    [Binding]
    public class CanConnectThroughWcfSteps
    {
        private readonly TestContext testContext;

        public CanConnectThroughWcfSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [Given(@"there is a hosted WCF service using named pipes")]
        public void GivenThereIsAHostedWCFServiceUsingNamedPipes()
        {
            var host =  new ServiceHost(typeof(TestWcfService));
            host.Open();
            this.testContext.HostedWcfService = host;
        }

        [Given(@"I have a type provider for the named pipe endpoint")]
        public void GivenIHaveATypeProviderForTheNamedPipeEndpoint()
        {
            var service = new WcfServiceClient<ITestWcfService>("PipeClient");
            var provider = new WcfClientProvider(new [] { service });
            this.testContext.Access = new BasicTypeAccessProvider(PermissionsProvider.Empty, new[] { provider });
        }

        [Given(@"there is a hosted WCF service using TCP")]
        public void GivenThereIsAHostedWCFServiceUsingTCP()
        {
            this.testContext.HostedWcfService = new ServiceHost(typeof(TestWcfService));
        }

        [Given(@"I have a type provider for the TCP endpoint")]
        public void GivenIHaveATypeProviderForTheTCPEndpoint()
        {
            var service = new WcfServiceClient<ITestWcfService>("TcpClient");
            var provider = new WcfClientProvider(new[] { service });
            this.testContext.Access = new BasicTypeAccessProvider(PermissionsProvider.Empty, new[] { provider });
        }

        [When(@"I ask for the WCF service contract type through the type provider")]
        public void WhenIAskForTheWCFServiceContractTypeThroughTheTypeProvider()
        {
            try
            {
                var response = this.testContext.Access.TryAccess<ITestWcfService, bool>(this.testContext.Identity, service => service.GetTrue());                
                this.testContext.WcfCallWasSuccessful = response.IsSuccess;
            }
            catch (Exception ex)
            {
                this.testContext.WcfCallWasSuccessful = false;
                this.testContext.UnhandledException = ex;
            }
        }

        [Then(@"the WCF connection should be successful")]
        public void ThenTheWCFConnectionShouldBeSuccessful()
        {
            this.testContext.UnhandledException.Should().BeNull("because no exception should have been thrown");
            this.testContext.WcfCallWasSuccessful.Should().BeTrue("because the connection should have been successful");            
        }
    }
}
