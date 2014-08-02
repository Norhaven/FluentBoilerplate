Feature: RunWithErrorHandling
	In order to execute user code in a safer environment
	As an API developer
	I want to use custom exception handlers

@Contexts
Scenario: No error handling is present
	Given I have created an error context
	And I have not added any exception handlers
	And I perform an action that does not throw exceptions
	When I perform the action through the error context
	Then I should perform the action successfully

@Contexts
Scenario: Handler for thrown exception is present
	Given I have created an error context
	And I have added an exception handler for ArgumentException
	And I perform an action that throws ArgumentException
	When I perform the action through the error context
	Then the exception should have been handled

@Contexts
Scenario: Handler for thrown exception is not present
	Given I have created an error context
	And I have added an exception handler for ArgumentException
	And I perform an action that throws IndexOutOfRangeException
	When I perform the action through the error context
	Then the exception should not have been handled
