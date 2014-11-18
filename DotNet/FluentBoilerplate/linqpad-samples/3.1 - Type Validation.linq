<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

public class VerifiableType
{
	[StringLength(Minimum = 2, Maximum = 5)]
	[NotNull]
	public string Text { get; set; }
  
  [IntegerRange(Minimum=3, Maximum=42)]
  public int Value { get; }

  [IsMatchFor(@"\d+\w+")]
  public string Description { get; }
}

void Main()
{
	//First off, we need a boilerplate to work with.
	var boilerplate = Boilerplate.New();
	
	var verifiable = new VerifiableType();
	
	verifiable.Text = "Text";
  verifiable.Value = 15;
  verifiable.Description = "123Test";
	
	//Any of the following will cause a validation failure
	
	//verifiable.Text = String.Empty; //Too short
	//verifiable.Text = "This is too long";
	//verifiable.Text = null;
  //verifiable.Value = 2;
  //verifiable.Value = 43;
  //verifiable.Description = "123";
  //verifiable.Description = "Test";
	
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