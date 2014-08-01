Feature: ExceptionHandlersAreStoredTogether
	In order to keep exception handlers strongly typed but easy to store
	As an API consumer
	I want to make the handlers more or less specific

@Components
Scenario: Exception handler will handle specific exception when the handler is specific
	Given I have created an exception handler for ArgumentException
	And I have made the exception handler specific
	When I throw an ArgumentException within the exception handler
	Then the exception handler should handle the exception
	#TODO: Fix this, should probably be testing from provider level instead
@Components
Scenario: Exception handler will handle specific exception when the handler is non-specific
	Given I have created an exception handler for ArgumentException
	And I have made the exception handler non-specific
	When I an ArgumentException reaches the exception handler
	Then the exception handler should handle the exception
