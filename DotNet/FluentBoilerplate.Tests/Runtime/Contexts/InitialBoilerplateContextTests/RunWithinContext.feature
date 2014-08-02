Feature: RunWithinContext
	In order to perform an action with supporting context functionality
	As an API consumer
	I want to create supporting context functionality

@Contexts
Scenario: No supporting functionality is present
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	And I perform an action that does not throw exceptions
	When I create the boilerplate
	And I have not added a contract	
	And I perform the action through the boilerplate context
	Then I should perform the action successfully

@Contexts
Scenario: Contract has requirements that pass
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	And I perform an action that does not throw exceptions
	When I create the boilerplate
	And I have added a contract that has requirements
	And all contract requirements pass
	And I perform the action through the boilerplate context
	Then I should perform the action successfully

@Contexts
Scenario: Contract has requirements that fail
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	And I perform an action that does not throw exceptions
	When I create the boilerplate	
	And I have added a contract that has requirements
	And a contract requirement fails
	And I perform the action through the boilerplate context
	Then a ContractViolationException should be thrown

@Contexts
Scenario: Contract has one exception handler
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	And I perform an action that throws ArgumentException
	When I create the boilerplate
	And I have added a contract that handles ArgumentException
	And I perform the action through the boilerplate context
	Then the exception should have been handled

@Contexts
Scenario: Contract has two exception handlers in descending order
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	And I perform an action that throws ArgumentException
	When I create the boilerplate
	And I have added a contract that handles ArgumentException and then Exception
	And I perform the action through the boilerplate context
	Then the ArgumentException exception handler should handle the exception

@Contexts
Scenario: Contract has two exception handlers in ascending order
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	And I perform an action that throws ArgumentException
	When I create the boilerplate
	And I have added a contract that handles Exception and then ArgumentException
	And I perform the action through the boilerplate context
	Then the Exception exception handler should handle the exception

@Contexts
Scenario: Contract has validations that fail
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	And I perform an action that does not throw exceptions
	When I create the boilerplate
	And I have added a contract that fails validation
	And I perform the action through the boilerplate context
	Then a ContractViolationException should be thrown

@Contexts
Scenario: Contract has validations that pass
	Given I have asked for a new boilerplate
	And I have no identity
	And I have no provider
	And I perform an action that does not throw exceptions
	When I create the boilerplate
	And I have added a contract that passes validation
	And I perform the action through the boilerplate context
	Then I should perform the action successfully