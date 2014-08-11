<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>FluentBoilerplate.Providers</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
</Query>

void Main()
{
	//Type access is a little more difficult thing to think about. What does it actually mean to "access a type"?
	
	//In short, it means that you have an instance of a particular type (or types) and the user can request access to
	//those instances. They may or may not be qualified to access any of those instances, so we need a gatekeeper.
	
	//FluentBoilerplate.Providers.ITypeProvider is how the actual type instance is provided to the user.
	//FluentBoilerplate.Providers.ITypeAccessProvider is the gatekeeper for any number of type providers.
	
	//For example, if you looked carefully at the WCF sample, you'd see that it's a custom implementation 
	//of ITypeProvider that's geared solely towards creating and managing WCF connections and their types (i.e. contract interface).
	
	//FluentBoilerplate.Providers.TypeAccessProvider is used, which is a default implementation that will just 
	//use the identity permissions to verify whether the user can open a WCF connection.
	
	//It doesn't have to be WCF. You could create database connections, you could use this as a factory, whatever seems appropriate to you.
	//So how does it get used?
	
	//Let's create a quick implementation of our own factory type provider.
	UseFactoryTypeProvider();
}

public class FactoryTypeProvider : ITypeProvider
{
	//Implementation of ITypeProvider
	public IImmutableSet<Type> ProvidableTypes { get; private set; }
	
    public void Use<TType>(Action<TType> useType) 
	{
		if (!this.ProvidableTypes.Contains(typeof(TType)))
			return;
			
		var instance = CreateType<TType>();
		useType(instance);
	}
	
    public TResult Use<TType, TResult>(Func<TType, TResult> useType) 
	{
		if (!this.ProvidableTypes.Contains(typeof(TType)))
			return default(TResult);
			
		var instance = CreateType<TType>();
		return useType(instance);
	}
	
	public FactoryTypeProvider(IImmutableSet<Type> providableTypes)
	{
		this.ProvidableTypes = providableTypes;
	}
	
	private TType CreateType<TType>() 
	{
		return Activator.CreateInstance<TType>();
	}
}

class CustomType
{
}
class OtherCustomType
{
}
private static void UseFactoryTypeProvider()
{
	//Okay, let's create it!
	var knownTypes = new Type[] { typeof(CustomType), typeof(OtherCustomType) };
	var provider = new FactoryTypeProvider(knownTypes.ToImmutableHashSet());
	
	//If you'd like to just use the default access provider, we can do that.
	//You could alternately use a custom IPermissionsProvider with the TypeAccessProvider
	//to impose your own permissions verification for this as well.
	var defaultAccessProvider = new TypeAccessProvider(provider);
	var boilerplate = Boilerplate.New(accessProvider: defaultAccessProvider);
	
	//There is a method on your boilerplate context called Open<T>() and this is where type access happens.
	boilerplate.Open<CustomType>().AndDo((context, value) => Console.WriteLine(value));
	
	//What if you want to control your own access?
	UseCustomTypeAccessProvider();
}

//A type access provider is required to return an IResponse or IResponse<TResult> from the access attempts.
//There might be any number of reasons why the access wasn't given, so a response will tell the caller
//specifically whether it succeeded, give the result of the operation (if any), and any additional
//information with an implementation of IResult.

//For this example, let's just use a skeleton implementation and assume that you know what you'd do if you wrote it.
public class CustomResponse:IResponse
{
	public IResultCode Result { get; set; }
	public bool IsSuccess { get; set; }
}

public class CustomResponse<TResult>:CustomResponse, IResponse<TResult>
{
	public TResult Content { get; set; }
}

//So let's create a quick type access provider that just ignores permissions and provides the type if it can.
public class CustomTypeAccessProvider:ITypeAccessProvider
{
	private readonly IImmutableQueue<ITypeProvider> providers = ImmutableQueue<ITypeProvider>.Empty;
	
    public ITypeAccessProvider AddProvider(ITypeProvider provider)
	{
		var elevatedProviders = this.providers.Enqueue(provider);
		return new CustomTypeAccessProvider(elevatedProviders);
	}
	
    public IResponse<TResult> TryAccess<TType, TResult>(IIdentity identity, Func<TType, TResult> useType)
	{
		var provider = FindProviderForType(typeof(TType));
		if (provider == null)
			throw new ArgumentException("No provider found for the requested type");
		var result = provider.Use<TType, TResult>(instance => useType(instance));
		return new CustomResponse<TResult> { IsSuccess = true, Content = result };
	}
	
    public IResponse TryAccess<TType>(IIdentity identity, Action<TType> useType)
	{
		var provider = FindProviderForType(typeof(TType));
		if (provider == null)
			throw new ArgumentException("No provider found for the requested type");
		provider.Use<TType>(instance => useType(instance));
		return new CustomResponse { IsSuccess = true };
	}
	
	public CustomTypeAccessProvider(IImmutableQueue<ITypeProvider> providers)
	{
		this.providers = providers;
	}
	
	private ITypeProvider FindProviderForType(Type type)
	{
		var current = this.providers;
		while(!current.IsEmpty)
		{
			ITypeProvider provider;
			current = current.Dequeue(out provider);
			if (provider.ProvidableTypes.Contains(type))
				return provider;				
		}
		
		return null;
	}
}

private static void UseCustomTypeAccessProvider()
{
	//Whew! These keep getting longer and longer. 
	//So how is this used?
	var knownTypes = new Type[] { typeof(CustomType), typeof(OtherCustomType) };
	var provider = new FactoryTypeProvider(knownTypes.ToImmutableHashSet());
	ITypeAccessProvider customAccessProvider = new CustomTypeAccessProvider(ImmutableQueue<ITypeProvider>.Empty);
	customAccessProvider = customAccessProvider.AddProvider(provider);
	
	//And that goes into the boilerplate, just like before!
	var boilerplate = Boilerplate.New(accessProvider: customAccessProvider);
	
	//Now this will use your access provider as well as your type provider.
	boilerplate.Open<OtherCustomType>().AndDo((context, value) => Console.WriteLine(value));
}