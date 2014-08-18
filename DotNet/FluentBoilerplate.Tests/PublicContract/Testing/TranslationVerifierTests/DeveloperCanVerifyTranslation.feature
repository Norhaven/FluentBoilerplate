Feature: DeveloperCanVerifyTranslation
	In order to make it easier to develop translations between types
	As an API consumer
	I want to be able to verify that a type mapping is correct

@Testing
Scenario: The source and target types have no properties
	Given I have a source type with no properties
	And I have a target type with no properties
	When I verify the translation
	Then the translation verification should pass

@Testing
Scenario: The source type has no properties and the target type has properties
	Given I have a source type with no properties
	And I have a target type with properties
	When I verify the translation
	Then the translation verification should fail

@Testing
Scenario: The source type has properties and the target type has no properties
	Given I have a source type with properties
	And I have a target type with no properties
	When I verify the translation
	Then the translation verification should fail

@Testing
Scenario: The source type has mapped properties and the target type has properties
	Given I have a source type with mapped properties
	And I have a target type with properties
	When I verify the translation
	Then the translation verification should pass

@Testing
Scenario: The source type has mapped properties and the target type has no properties
	Given I have a source type with mapped properties
	And I have a target type with no properties
	When I verify the translation
	Then the translation verification should fail

