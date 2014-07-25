FluentBoilerplate
=================

Official releases will be in NuGet. All release information and the package will be located at https://www.nuget.org/packages/FluentBoilerplate/ and visible in NuGet searches.

This is still an alpha-quality work in progress. This means that things can and will break.

That said, you're more than welcome to report any bugs you may find, contribute, etc., keeping in mind that the codebase may be changing fairly quickly.

Enjoy!

=================

Getting Started

- Build the source (or get a release from NuGet, if available)
- Reference the built assembly in your project
- Add "using FluentBoilerplate;" to your file's using statements.

The Simplest Example
=================

Everything is driven from an IContext implementation. You should get one.

```C#
var boilerplate = Boilerplate.New();
```C#

Doing something within the context is very simple. If you don't want to return a result, you may do this.

```C#
boilerplate.Do(context => /* Take some action */);
```C#

If you'd like to get a result back, you may do this instead.

```C#
boilerplate.Get<int>(context => /* Take some action that returns an integer */);
```C#

After you get a result, you may want to use it.

```C#
var text = boilerplate
              .Get<string>(context => /* Take some action that returns a string */)
              .Result;
```C#

So why is that useful? Those actions can have a contract defined that applies to them.

For example, it might be nice to handle an ArgumentException that might be thrown when doing the action. For this, we need to create a contract.

```C#
boilerplate
    .BeginContract()
         .Handles<ArgumentException>("Top Level", ex => Console.WriteLine(ex.Message))
    .EndContract()
    .Do(context => /* Take some action */);
```C#

If your call to Do() throws an ArgumentException, it will now be caught and its message written to the console. Just like you would with a try/catch block, you can define any number of exception types and their handlers, and the order they're defined in matches the order in which they're handled.

=================

Additional information is forthcoming...

