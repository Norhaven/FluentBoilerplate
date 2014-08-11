<Query Kind="Statements">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

var everyone = ActiveDirectoryGroup.Everyone;

//So how do you know what permissions the caller has? In this case, you won't get this information up front.
//Let's use the current Windows user as the identity.
var windowsIdentity = Identity.CurrentWindowsUser;

//Create the boilerplate using the Windows identity
var boilerplate = Boilerplate.New(identity: windowsIdentity);
	
//Active Directory groups are treated like roles for the purposes of the boilerplate.
//This particular example will succeed or fail based on who you are and what permissions you have.
boilerplate
	.BeginContract()
		.RequireRoles(everyone)
	.EndContract()
	.Do(context => { /* Take some action */ });	
	
//Several common Active Directory groups are available from the ActiveDirectoryGroup type, but you're welcome to
//specify whichever groups you'd like to by creating a new instance.
var customGroup = new ActiveDirectoryGroup("SomeCustomGroup");

//Only machine level Active Directory authentication is supported currently, 
//but application directory and domain level authentication are planned!