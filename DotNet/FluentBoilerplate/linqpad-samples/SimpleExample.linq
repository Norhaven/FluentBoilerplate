<Query Kind="Statements">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

//Everything is driven from an IContext implementation. You should get one.
var boilerplate = Boilerplate.New();
//Doing something within the context is very simple. If you don't want to return a result, you may do this.
boilerplate.Do(context =>  /* Take some action */	string.Empty.ToString());
//If you'd like to get a result back, you may do this instead.

boilerplate.Get<int>(context => /* Take some action that returns an integer */ 1);
//After you get a result, you may want to use it.

var text = boilerplate
              .Get<string>(context => /* Take some action that returns a string */ "string")
              .Result;
//So why is that useful? Those actions can have a contract defined that applies to them.

//For example, it might be nice to handle an ArgumentException that might be thrown when doing the action. For this, we need to create a contract.
boilerplate
    .BeginContract()
         .Handles<ArgumentException>(ex => Console.WriteLine(ex.Message))
    .EndContract()
    .Do(context => /* Take some action */ string.Empty.ToString());
//If your call to Do() throws an ArgumentException, it will now be caught and its message written to the console.
//Just like you would with a try/catch block, you can define any number of exception types and their handlers,
// and the order they're defined in matches the order in which they're handled.