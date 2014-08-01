Feature: BoilerplateCreation
	In order to avoid complicated boilerplate creation and types that are internal
	As an API consumer
	I want to be able to create a boilerplate easily

@Contexts
Scenario: No customizations are present
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	When I create the boilerplate
	Then I should receive an instance of a boilerplate context with the default identity and no additional type access

@Contexts
Scenario: Custom identity is present
	Given I have asked for a new boilerplate
	And I have an identity with no permissions
	And I have no provider
	When I create the boilerplate
	Then I should receive an instance of a boilerplate context with the custom identity and no additional type access

@Contexts
Scenario: Custom type access provider is present
	Given I have asked for a new boilerplate
	And I have no identity
	And I have a provider with no permissions
	When I create the boilerplate
	Then I should receive an instance of a boilerplate context with the default identity and the custom type access

@Contexts
Scenario: Custom identity and custom type access provider are present
	Given I have asked for a new boilerplate
	And I have an identity with permissions
	And I have a provider with required permissions
	When I create the boilerplate
	Then I should receive an instance of a boilerplate context with the custom identity and the custom type access