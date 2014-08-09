<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

public class VerifiableType
{
	[StringLength(Minimum = 2, Maximum = 5)]
	[NotNull]
	public string Text { get; set; }
}

void Main()
{
	//First off, we need a boilerplate to work with.
	var boilerplate = Boilerplate.New();
	
	var verifiable = new VerifiableType();
	
	verifiable.Text = "Text";
	
	//Any of the following will cause a validation failure
	
	//verifiable.Text = String.Empty; //Too short
	//verifiable.Text = "This is too long";
	//verifiable.Text = null;
	
	boilerplate
		.BeginContract()
			.RequireValidInstanceOf(verifiable)
		.EndContract()
		.Do(context => { /* Take some action */ });
		
	//The attributes placed on the Text property of the VerifiableType class determine how the instance should be validated.
	//A validation failure will result in a ContractViolationException.
	//Any property within the validated type with validation attributes will be checked.
	
	//More validation attributes are planned for future releases!
	
	//You may also want to provide type conversions! Check that example out next.
}