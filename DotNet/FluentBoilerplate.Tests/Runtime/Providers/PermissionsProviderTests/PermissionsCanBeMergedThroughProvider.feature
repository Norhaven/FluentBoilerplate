Feature: PermissionsCanBeMergedThroughProvider
	In order to easily elevate permissions that are provided
	As an API developer
	I want to merge sets of permissions 

@Providers
Scenario: No permissions merged with no permissions
	Given I have a permissions provider with no permissions	
	When I merge the permissions provider with a set of no permissions
	Then the permissions provider should have no permissions

@Providers
Scenario: No permissions merged with required rights
	Given I have a permissions provider with no permissions	
	When I merge the permissions provider with a set of required rights
	Then the permissions provider should have merged the required rights

@Providers
Scenario: Required rights merged with required rights
	Given I have a permissions provider with required permissions
	When I merge the permissions provider with a set of required rights
	Then the permissions provider should have merged the required rights