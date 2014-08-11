<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

void Main()
{
	//First off, we need a boilerplate to work with.
	var boilerplate = Boilerplate.New();
	
	//Contracts tend to be useful for verifying that the program state is expected, both before and after doing something.
	var statefulInstance = new Stateful();
	statefulInstance.DoWithEnsuring(boilerplate);
}

public class Stateful
{
	public object State { get; private set; }
	
	public void DoWithEnsuring(IBoilerplateContext boilerplate)
	{	
		//Let's make sure that the state is still going to be valid after we get done.
		boilerplate
			.BeginContract()
				.EnsureOnReturn(() => this.State != null)
				.EnsureOnThrow(() => this.State != null)
			.EndContract()
			.Do(context => 
				{ 
					this.State = new object(); //Set this.State to null and the contract will fail
				});
			
		//This creates a contract which ensures that the State property isn't null after your action has completed.
		//This will be verified after executing the contents of Do() and a ContractVerificationException will be thrown
		//if the contract requirements fail. You're welcome to throw your own custom exception as well, as below.
		
		boilerplate
			.BeginContract()
				.EnsureOnReturn(() => this.State != null, () => new IncorrectStateException())
				.EnsureOnThrow(() => this.State != null, (ex) => new IncorrectStateException(ex))
			.EndContract()
			.Do(context => 
				{ 				
					this.State = new object(); //Set this.State to null and the contract will fail
				});
				
		//This example covers basic require/ensure conditions. 
		//You may also include type-specific validation in your contract! Check out that example next.
	}
}

public class IncorrectStateException:Exception
{
	public IncorrectStateException():base(){}
	public IncorrectStateException(Exception innerException):base(String.Empty,innerException)
	{
	}
}