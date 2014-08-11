<Query Kind="Statements">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

//You're welcome to create your own rights and roles and require the caller to have them (or not have them).
var basicRight = new Right(0, "Basic Right", "This is a basic right");
var basicRole = new Role(0, "Basic Role", "This is a basic role");

//So how do you know what permissions the caller has? A type that implements the FluentBoilerplate.IIdentity interface.
//Let's just grab a default version of this for the example. This identity isn't specifically allowed or restricted from anything.
var defaultIdentity = Identity.Default;

//This time, create a boilerplate that knows who is using it.
var boilerplate = Boilerplate.New(identity: defaultIdentity);

//You could require that the identity have those prior to executing an action.
//This particular example will fail the permissions check and throw a ContractViolationException because the default identity isn't permitted either of those.
boilerplate
	.BeginContract()
		.RequireRights(basicRight)
		.RequireRoles(basicRole)
	.EndContract()
	.Do(context => { throw new Exception(); });
	
//Alternately, you could make sure that the identity doesn't have either of those.
//This particular example will succeed because the default identity isn't permitted either of those.
boilerplate
	.BeginContract()
		.MustNotHaveRights(basicRight)
		.MustNotHaveRoles(basicRole)
	.EndContract()
	.Do(context => { /* Take some action */ });
	
//You're welcome to create an instance of Identity and specify its permissions or create your own implementation!