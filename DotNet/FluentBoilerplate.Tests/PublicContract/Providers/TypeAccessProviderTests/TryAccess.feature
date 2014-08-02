Feature: TryAccess
	In order to access a custom type easily
	As an API consumer
	I want to have a type access provider with the capability to attempt accessing the type

@Providers
Scenario: Provider with required permissions and identity with no permissions
	Given I want to access a custom type
	And I have an access provider with required permissions
	And I have an identity with no permissions
	When I try to access the type
	Then I should fail to access the type

@Providers
Scenario: Provider with no permissions and identity with no permissions
	Given I want to access a custom type
	And I have an access provider with no permissions
	And I have an identity with no permissions
	When I try to access the type
	Then I should be able to access the type

@Providers
Scenario: Provider with no permissions and identity with permissions
	Given I want to access a custom type
	And I have an access provider with no permissions
	And I have an identity with permissions
	When I try to access the type
	Then I should be able to access the type

@Providers
Scenario: Provider with permissions and identity with permissions
	Given I want to access a custom type
	And I have an access provider with required permissions
	And I have an identity with permissions
	When I try to access the type
	Then I should be able to access the type

@Providers
Scenario: Provider is null
	Given I want to access a custom type
	And I have no provider
	And I have an identity with no permissions
	When I try to access the type
	Then I should fail to access the type

@Providers
Scenario: Result provider with required permissions and identity with no permissions
	Given I want to access a custom type and get a result
	And I have an access provider with required permissions
	And I have an identity with no permissions
	When I try to access the type
	Then I should fail to access the type

@Providers
Scenario: Result provider with no permissions and identity with no permissions
	Given I want to access a custom type and get a result
	And I have an access provider with no permissions
	And I have an identity with no permissions
	When I try to access the type
	Then I should be able to access the type

@Providers
Scenario: Result provider with no permissions and identity with permissions
	Given I want to access a custom type and get a result
	And I have an access provider with no permissions
	And I have an identity with permissions
	When I try to access the type
	Then I should be able to access the type

@Providers
Scenario: Result provider with permissions and identity with permissions
	Given I want to access a custom type and get a result
	And I have an access provider with required permissions
	And I have an identity with permissions
	When I try to access the type
	Then I should be able to access the type

@Providers
Scenario: Result provider is null
	Given I want to access a custom type and get a result
	And I have no provider
	And I have an identity with no permissions
	When I try to access the type
	Then I should fail to access the type