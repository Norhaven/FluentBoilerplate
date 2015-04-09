<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

void Main()
{
	//Everything is driven from an IBoilerplateContext implementation. You should get one.
	var boilerplate = Boilerplate.New();
	
	//Doing something within the context is very simple. If you don't want to return a result, you may call Do().
	boilerplate.Do(context => { /* Take some action */ });
	
	//If you would like to get a result back, you may call Get() and, optionally, use the Result property to get the result.
	var text = boilerplate
					.Get<string>(context => "I want to return this text")
					.Result;
	
	Console.WriteLine(text);
	
	//So why is that useful? Those actions can have a contract defined that applies to them.
	
	//For example, it might be nice to handle an ArgumentException that might be thrown when doing the action. For this, we need to create a contract.
	boilerplate
		.BeginContract()
	  		.Handles<ArgumentException>(ex => Console.WriteLine(ex.Message))
	  	.EndContract()
	  	.Do(context => { throw new ArgumentException("Something went wrong!"); });
	
	//Just like you would with a try/catch block, you can define any number of exception types and their handlers,
	//and the order they're defined in matches the order in which they're handled.
	boilerplate
	  	.BeginContract()
		    .Handles<ArgumentException>(ex => Console.WriteLine(ex.Message))
	      	.Handles<Exception>(ex => Console.WriteLine(ex.Message))
		.EndContract()
		.Do(context => { throw new NotImplementedException(); });
		
	//You may have noticed that both the Do() and Get() calls have a context parameter.
	//This provides you with your current boilerplate context, minus the contract.
	//You can use this to continue the boilerplate and create additional contracts.
	boilerplate
		.BeginContract()
			.Handles<Exception>(ex => Console.WriteLine(ex.Message))
		.EndContract()
		.Do(context =>
		{
			var fileName = GetFileName();
			var formattedFileName = 
				context
					.BeginContract()
						.Handles<FormatException>(ex => Console.WriteLine(ex.Message))
					.EndContract()
					.Get<string>(_ => String.Format("{0}.txt", fileName))
					.Result;
			
			Console.WriteLine(formattedFileName);
		});
	
	//That's a little bit cumbersome though. We're trying to reduce the concentration of boilerplate, not add to it!
	//You may prefer to split your operations out into separate methods, each with their own contract (if any),
	//for easier segregation of operations with the bonus of keeping your code complexity low.
	
	//For example, let's see the code above in that style instead.
	DoAllActionsSafely(boilerplate);
	
  //Perhaps you'd like to retry the action a few times if a particular exception happens.
  //That's also pretty easy!
  
  boilerplate
    .BeginContract()
      .Handles<ArgumentException>(ex => Console.WriteLine(ex.Message)).WithRetryOf(3)
    .EndContract()
    .Do(_ => 
      { 
        Console.WriteLine("Tried to do an action"); 
        throw new ArgumentException("Encountered a problem doing this action");
      });
      
  //You can also customize the interval between retries and apply a strategy to backing off subsequent retries.
  
	//That's not all you can do, by a long shot.
	//Check out some of the other samples!
}

private static string GetFileName()
{
	return "SomeFileName";
}
private static void WriteExceptionToConsole(Exception exception)
{
	Console.WriteLine(exception.Message);
}

private static void DoAllActionsSafely(IBoilerplateContext boilerplate)
{
	boilerplate
		.BeginContract()
			.Handles<Exception>(WriteExceptionToConsole)
		.EndContract()
		.Do(FormatFileName);
}

private static void FormatFileName(IBoilerplateContext boilerplate)
{
	var fileName = GetFileName();
	var formattedFileName = GetFormattedFileName(boilerplate, fileName);
	
	Console.WriteLine(formattedFileName);			
}

private static string GetFormattedFileName(IBoilerplateContext boilerplate, string fileName)
{
	return boilerplate
		.BeginContract()
			.Handles<FormatException>(WriteExceptionToConsole)
		.EndContract()
		.Get<string>(_ => String.Format("{0}.txt", fileName))
		.Result;
}