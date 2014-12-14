Feature: RestrictThreadsByWaitHandle
	In order to handle the threading model correctly per situation
	As a Developer
	I want to restrict the threads concurrently running in a section by using a WaitHandle

@Threading
Scenario: ManualResetEvent is waited for appropriately
	Given I have a contract that is waiting on a WaitHandle
	And the WaitHandle is a ManualResetEvent
	When I execute an action
	Then the thread should wait on the WaitHandle until signalled
