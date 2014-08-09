<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

void Main()
{
	//First off, we need a boilerplate to work with.
	var boilerplate = Boilerplate.New();
	
	//Contracts tend to be useful for verifying that the program state is expected, both before and after doing something.	
	DoWithRequirements(boilerplate, new object()); 		
}

public static void DoWithRequirements(IContext boilerplate, object value)
{
	//Let's state some basic requirements.
	boilerplate
		.BeginContract()
			.Require(() => value != null)
		.EndContract()
		.Do(context => { /* Take some action */ });
		
	//This creates a contract with a simple requirement, that the object parameter is not null.
	//The requirement will be verified prior to executing the contents of Do() and a ContractVerificationException will be thrown
	//if the contract requirements fail. You're welcome to throw your own custom exception as well, as below.
	
	boilerplate
		.BeginContract()
			.Require(() => value != null, () => new ArgumentException("Provided value must not be null", "value"))
		.EndContract()
		.Do(context => { /* Take some action */ });
	
	//You are also able to ensure that you are leaving the program state in an expected manner.
	//Check out the example on ensuring state!
}
