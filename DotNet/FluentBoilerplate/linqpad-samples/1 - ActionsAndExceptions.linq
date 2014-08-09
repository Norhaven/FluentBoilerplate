<Query Kind="Statements">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>
//Everything is driven from an IContext implementation. You should get one.
var boilerplate = Boilerplate.New();

//Doing something within the context is very simple. If you don't want to return a result, you may call Do().
boilerplate.Do(context => { /* Take some action */ });

//If you would like to get a result back, you may call Get() and, optionally, use the Result property to get the result.
var text = boilerplate
.Get<string>
  (context => "I want to return this text")
  .Result;

  Console.WriteLine(text);

  //So why is that useful? Those actions can have a contract defined that applies to them.

  //For example, it might be nice to handle an ArgumentException that might be thrown when doing the action. For this, we need to create a contract.
  boilerplate
  .BeginContract()
  .Handles<ArgumentException>
    (ex => Console.WriteLine(ex.Message))
    .EndContract()
    .Do(context => { throw new ArgumentException("Something went wrong!"); });

    //Just like you would with a try/catch block, you can define any number of exception types and their handlers,
    // and the order they're defined in matches the order in which they're handled.
    boilerplate
    .BeginContract()
    .Handles<ArgumentException>
      (ex => Console.WriteLine(ex.Message))
      .Handles<Exception>(ex => Console.WriteLine(ex.Message))
	.EndContract()
	.Do(context => { throw new NotImplementedException(); });
	
//That's not all you can do, by a long shot.
//Check out some of the other samples!