Feature: CanConnectThroughWcf
	In order to connect to a WCF service
	As an API consumer
	I want to have a WCF type access provider

@Providers
Scenario: Connecting to a WCF service over named pipes
	Given there is a hosted WCF service using named pipes
	And I have a type provider for the named pipe endpoint
	When I ask for the WCF service contract type through the type provider
	Then the WCF connection should be successful