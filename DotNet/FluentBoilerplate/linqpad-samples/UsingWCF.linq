<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceModel.dll</Reference>
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>FluentBoilerplate.Providers</Namespace>
  <Namespace>FluentBoilerplate.Providers.WCF</Namespace>
  <Namespace>System.ServiceModel</Namespace>
  <Namespace>System.ServiceModel.Channels</Namespace>
  <Namespace>System.ServiceModel.Description</Namespace>
</Query>

void Main()
{
	//This requires a little bit of setup prior to creating the boilerplate.
	
	//First, let's host the WCF service that we're going to talk to.
	HostService();
	
	//Now, we need something that can provide us an instance of the WCF service client.
	//Let's use the default WCF provider for this example.
	var wcfConnectionProvider = CreateWcfTypeProvider();
	
	//We also need something that can give us access to that provider.
	//Let's use the default access provider for this example and not require/restrict any permissions.
	var wcfAccessProvider = new TypeAccessProvider(wcfConnectionProvider);
		
	//When we create the boilerplate this time, let's do something different.
	var boilerplate = Boilerplate.New(accessProvider: wcfAccessProvider);
		
	//This allows the boilerplate to make use of your type access provider!
	//Let's create a connection to the service and get a value back.
	var value = boilerplate
					.Open<IExampleService>()
					.AndGet<bool>((context, client) => client.GetValue())
					.Result;
	
	Console.WriteLine(value);
}

private static Binding binding = new NetNamedPipeBinding();
private static EndpointAddress endpoint = new EndpointAddress("net.pipe://localhost/FluentBoilerplateExampleService");

private static ITypeProvider CreateWcfTypeProvider()
{
	var service = new WcfService<IExampleService>(binding, endpoint);
	return new WcfConnectionProvider(new [] { service });
}

private static void HostService()
{            
	var description = new ContractDescription("IExampleService");
    var binding = new NetNamedPipeBinding();	
	var host =  new ServiceHost(typeof(ExampleService));
    host.AddServiceEndpoint(new ServiceEndpoint(description, binding, endpoint));
    host.Open();
}

[ServiceContract]
public interface IExampleService
{
	[OperationContract]
	bool GetValue();
}

public class ExampleService:IExampleService
{
	public bool GetValue()
	{
		return true;
	}
}
