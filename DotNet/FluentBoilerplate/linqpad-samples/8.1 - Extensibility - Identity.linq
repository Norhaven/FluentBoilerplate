<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
</Query>

void Main()
{
	//So, identity. This is all about who is currently using the code, and it's very likely
	//that you have a different idea about the user than I do. You're welcome to use the
	//default identity (FluentBoilerplate.Identity), but you may outgrow it and need 
	//something that's better suited to your purposes.
	
	//At that point, you should implement FluentBoilerplate.IIdentity yourself.
	//Let's do that below and then use it.
	UseCustomIdentity();
}

public class CustomIdentity:IIdentity
{
	//Implements IIdentity
 	public string Name { get; private set; }        
    public IImmutableSet<IRole> PermittedRoles { get; private set; }        
    public IImmutableSet<IRole> DeniedRoles { get; private set; }
    public IImmutableSet<IRight> PermittedRights { get; private set; }
    public IImmutableSet<IRight> DeniedRights { get; private set; }
	
	public IIdentity Copy(IImmutableSet<IRole> permittedRoles = null, 
                          IImmutableSet<IRole> deniedRoles = null, 
                          IImmutableSet<IRight> permittedRights = null, 
                          IImmutableSet<IRight> deniedRights = null)
	{
		return new CustomIdentity(this.UserId, this.Description, this.Name, this.PermittedRoles, this.DeniedRoles, this.PermittedRights, this.DeniedRights);
	}
	
	//Custom implementation
	public string Description { get; private set; }
	public int UserId { get; private set; }
	
	public CustomIdentity(int userId,
					      string description,
						  string name,
						  IImmutableSet<IRole> permittedRoles = null, 
                          IImmutableSet<IRole> deniedRoles = null, 
                          IImmutableSet<IRight> permittedRights = null, 
                          IImmutableSet<IRight> deniedRights = null)
	{
		//Setting everything omitted for brevity
	}
}

private static void UseCustomIdentity()
{
	//Whew. Now that we have a custom identity with some extra stuff in it, how do we use it?
	var customIdentity = new CustomIdentity(1, "A custom user", "Bob");
	var boilerplate = Boilerplate.New(identity: customIdentity);
	
	//That's all there is to it. The boilerplate will use your custom identity for all permissions verifications,
	//and you get the extra freedom to use your own object as an identity.
}