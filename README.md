FluentBoilerplate
=================

All official release information / released builds is located at https://www.nuget.org/packages/FluentBoilerplate/.

**This is a beta-quality work in progress. This means that things could break or behave in unexpected ways**

That said, you're more than welcome to report any bugs you may find, contribute, etc.

Enjoy!

Getting Started
=================

- Build the source (or get a release from NuGet, if available)
- Reference the built assembly in your project

The Simplest Example
=================

The only namespace you should care about is FluentBoilerplate. Add that to your file's using statements.

```C#
using FluentBoilerplate;
```

Everything is driven from an IContext implementation. You should get one.

```C#
var boilerplate = Boilerplate.New();
```

Doing something within the context is very simple. If you don't want to return a result, you may do this.

```C#
boilerplate.Do(context => /* Take some action */);
```

If you'd like to get a result back, you may do this instead.

```C#
boilerplate.Get<int>(context => /* Take some action that returns an integer */);
```

After you get a result, you may want to use it.

```C#
var text = boilerplate
              .Get<string>(context => /* Take some action that returns a string */)
              .Result;
```

So why is that useful? Those actions can have a contract defined that applies to them.

For example, it might be nice to handle an ArgumentException that might be thrown when doing the action. For this, we need to create a contract.

```C#
boilerplate
    .BeginContract()
         .Handles<ArgumentException>("Top Level", ex => Console.WriteLine(ex.Message))
    .EndContract()
    .Do(context => /* Take some action */);
```

If your call to Do() throws an ArgumentException, it will now be caught and its message written to the console. Just like you would with a try/catch block, you can define any number of exception types and their handlers, and the order they're defined in matches the order in which they're handled.

Contract Examples
=================

You may be saying to yourself "Self, this sounds sort of like Code Contracts", and you wouldn't be wrong. This follows along the same lines, in that it's very often ideal to be able to express pre and post conditions for a given action.

Let's require that a parameter is not null before we do something.

```C#
public static void DoSomething(IContext boilerplate, string text)
{
    boilerplate
        .BeginContract()
             .Requires(text != null, "The parameter 'text' must not be null")
        .EndContract()
        .Do(context => /* Take some action */);
}
```

When the call to Do() is performed, the contract will be validated. If the parameter is null, a ContractViolationException will be thrown with the given message.

You're welcome to throw your own exceptions as well.

```C#
public static void DoSomething(IContext boilerplate, string text)
{
    boilerplate
        .BeginContract()
             .Requires(text != null, () => new ArgumentException("text", "The parameter must not be null"))
        .EndContract()
        .Do(context => /* Take some action */);
}
```

Preconditions, like the ones above, are useful because you can verify that things are in a necessary state before you operate on it. Postconditions, on the other hand, are useful for ensuring that _you_ aren't leaving things in a bad state for anyone else.

Let's make sure that we're leaving the state appropriately.

```C#
public class Example
{
    public string Text { get; private set; }

    public void DoSomething(IContext boilerplate)
    {
        boilerplate
            .BeginContract()
                .EnsureOnReturn(() => this.Text != null, "Text must be non-null on return")
                .EnsureOnThrow(() => this.Text == null, "Text must be null when an exception is thrown")
            .EndContract()
            .Do(context => /* Take some action */);
    }
}
```

There are two kinds of postconditions. 

- EnsureOnReturn() makes sure that the state is appropriate when returning from the action. 
- EnsureOnThrow() makes sure that the state is appropriate when the action threw an exception.

Permissions
=================

You might want to be a little more specific in terms of what a particular caller is allowed or restricted from doing.

Let's say we have several rights that a given caller may or may not have.

```C#
public static class KnownRights
{
    public static IRight CanPerformAction = new Right(1, "User can perform an action");
    public static IRight CanDoTerribleThings = new Right(2, "User can do terrible things");
}
```

We'd like to make sure that the caller is allowed to perform an action, but not do anything terrible.

```C#
private void DoSomething(IContext boilerplate)
{
    boilerplate
        .BeginContract()
            .RequiresRights(KnownRights.CanPerformAction)
            .MustNotHaveRights(KnownRights.CanDoTerribleThings)
        .EndContract()
        .Do(context => { /* Take some action */ });
}
```

You can also do the same thing at a roles level. Here are some arbitrary roles that we've just defined.

```C#
public static class KnownRoles
{
    public static IRole BasicUser = new Role(1,
                                             "A user",
                                             new HashSet<IRight>
                                             {
                                                 KnownRights.CanPerformAction
                                             }.ToImmutableHashSet());

    public static IRole RestrictedUser = new Role(2,
                                                  "A user with limited access",
                                                  new HashSet<IRight>().ToImmutableHashSet());
}
```

You could then include roles in your contract.

```C#
private void DoSomething(IContext boilerplate)
{
    boilerplate
        .BeginContract()
            .RequiresRoles(KnownRoles.BasicUser)
            .MustNotHaveRoles(KnownRoles.RestrictedUser)
        .EndContract()
        .Do(context => { /* Take some action */ });
}
```

How do we know what rights/roles the caller has, though?

When you create a context, one of the parameters you may optionally specify is an IIdentity instance. This represents the current caller that the context will operate under, and includes sets of rights/roles that they are permitted and explicitly denied. You are welcome to use an instance of the Identity class, which implements IIdentity, or write your own.


Validation
=================

The contract also offers a slightly more targeted approach to preconditions, by validating that a specific instance meets all of its requirements (in particular, its properties).

There are currently two attributes that may be applied to properties for validation, with more planned.

```C#
public class SomeType
{
    [NotNull]
    [StringLength(MinLength=3, MaxLength=10)]
    public string Text { get; }
}
```

The [NotNull] attribute does what it says. During validation, that property must not be null.
The [StringLength] attribute enforces length requirements on a string property. MinLength is the inclusive lower bounds, meaning that the string must be three or more characters in length. MaxLength is the inclusive upper bounds, meaning that the string must be ten or less characters in length.

Let's validate an instance of SomeType.

```C#
private void DoValidatedAction(IContext boilerplate, SomeType instance)
{
    boilerplate
        .BeginContract()
            .RequiresValidInstanceOf(instance)
        .EndContract()
        .Do(context => { /* Take some action */ });
```

When the Do() method executes, the validation will be performed. Validation is a precondition and will run prior to the Do() method execution, but it will run after any other preconditions.

This gives us an execution path of:

- Rights/Roles validations
- Requirements preconditions
- Instance validations

- Do()

- Postconditions

Translation
=================

At some point in your program, you may want to treat an instance as another type. You could create a manual translation layer, or use reflection, but it would be nice if the context could just figure it out.

Let's say you have a few classes.

```C#
public class From
{
    public string Text { get; set; }
}

public class To
{
    public string Text { get; set; }
    public string Description { get; set; }
}
        
```

Translation is fairly easy.

```C#
public void DoSomething(IContext boilerplate, From fromInstance)
{
    fromInstance.Text = "Hello";
    var toInstance = boilerplate.Use(fromInstance).As<To>();
    
    Console.WriteLine(toInstance.Text); //Prints out "Hello"
}
```

The context will translate the properties based on the name and type of the property. If you would like to customize the translation mappings, use the [MapsTo] attribute on the specific properties.

Let's redo the From class with an explicit mapping.

```C#
public class From
{
    [MapsTo(typeof(To), "Description"]
    public string Text { get; set; }
}
```

Now let's change the method a little bit.

```C#
public void DoSomething(IContext boilerplate, From fromInstance)
{
    fromInstance.Text = "Hello";
    var toInstance = boilerplate.Use(fromInstance).As<To>();
    
    Console.WriteLine(toInstance.Description); //Prints out "Hello"
}
```

Provided Type Usage
=================

Additional information is forthcoming...

