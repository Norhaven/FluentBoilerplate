Feature: RestrictThreadsByCount
	In order to handle the threading model correctly per situation
	As a Developer
	I want to restrict the number of threads concurrently running in a section 

@Threading
Scenario: Restrict threads to one
	Given I have restricted the thread count to 1	
	When I execute an action using 5 threads
	Then only 1 threads at a time may use the restricted section
	
@Threading
Scenario: Restrict threads to five
	Given I have restricted the thread count to 5
	When I execute an action using 10 threads
	Then only 5 threads at a time may use the restricted section
