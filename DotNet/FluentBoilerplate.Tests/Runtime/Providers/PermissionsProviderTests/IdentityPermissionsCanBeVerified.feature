Feature: IdentityPermissionsCanBeVerified
	In order to restrict user code execution to users that have permission
	As an API developer
	I want to be able to verify a user's permissions

@Providers
Scenario: Identity has no permissions and provider has no permissions
	Given I have an identity with no permissions
	And I have a permissions provider with no permissions
	When I verify identity permissions through the provider
	Then the identity should have permission

@Providers
Scenario: Identity has no permissions and provider has required permissions
	Given I have an identity with no permissions
	And I have a permissions provider with required permissions
	When I verify identity permissions through the provider
	Then the identity should not have permission

@Providers
Scenario: Identity has permissions and provider has no permissions
	Given I have an identity with permissions
	And I have a permissions provider with no permissions
	When I verify identity permissions through the provider
	Then the identity should have permission

@Providers
Scenario: Identity has required permissions and provider has required permissions
	Given I have an identity with permissions
	And I have a permissions provider with required permissions
	When I verify identity permissions through the provider
	Then the identity should have permission