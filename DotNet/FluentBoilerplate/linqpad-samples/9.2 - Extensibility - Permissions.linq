<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>FluentBoilerplate.Providers</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
</Query>

void Main()
{
	//Permissions are about what the identity is or isn't allowed to do. The default permissions provider
	//will handle rights/roles verification for both the default permissions and Active Directory permissions,
	//but perhaps you have a custom authentication scheme that you'd like to use instead.
	
	//Let's create our own permissions provider now.
	UsePermissionsProvider();
}

public class CustomPermissionsProvider:IPermissionsProvider
{
	//Implements IPermissionsProvider
    public bool HasNoRequirements { get; private set; }
    public bool HasNoRestrictions { get; private set; }    
    public bool HasRequiredRights { get; private set; }
    public bool HasRequiredRoles { get; private set; }
    public bool HasRestrictedRights { get; private set; }
  	public bool HasRestrictedRoles { get; private set; }
	
    public bool HasPermission(IIdentity identity)
	{
		//Verify the identity permissions however you want.
		//Let's allow everything!
		return true;
	}
	
    public IPermissionsProvider Merge(IImmutableSet<IRole> requiredRoles = null,
                               IImmutableSet<IRole> restrictedRoles = null,
                               IImmutableSet<IRight> requiredRights = null,
                               IImmutableSet<IRight> restrictedRights = null)
	{
		//Merging the current permissions with the new permissions omitted for brevity
		return this;
	}
	
	public CustomPermissionsProvider (IImmutableSet<IRole> requiredRoles = null,
                                IImmutableSet<IRole> restrictedRoles = null,
                                IImmutableSet<IRight> requiredRights = null,
                                IImmutableSet<IRight> restrictedRights = null)
	{
		//Setting everything omitted for brevity
		
	}
}

private static void UsePermissionsProvider()
{
	//Okay, finally done. How do we use it?
	var customPermissionsProvider = new CustomPermissionsProvider();
	var boilerplate = Boilerplate.New(permissionsProvider: customPermissionsProvider);
	
	//That's all there is to it!
	//Now all permissions verifications will go through your custom permissions provider.
}