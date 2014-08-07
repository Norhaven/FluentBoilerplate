Feature: IdentityPermissionsCanBeVerifiedWithActiveDirectory
	In order to restrict user code execution to users that have permission
	As an API developer
	I want to be able to verify a user's permissions through Active Directory

@Providers
Scenario: Windows identity requires and has required application directory permissions
	Given I have created a Windows user named "FluentBoilerplateTestUser"
	And I have created an Active Directory group named "FluentBoilerplateTestGroup"
	And I have added the Windows user to the Active Directory group
	And I am using the application directory as the Active Directory context
	And I require access to an action by group "FluentBoilerplateTestGroup"
	And I perform an action that does not throw exceptions
	When I perform the action through the boilerplate context
	Then I should perform the action successfully

@Providers
Scenario: Windows identity requires and does not have required application directory permissions
	Given I have created a Windows user named "FluentBoilerplateTestUser"
	And I have created an Active Directory group named "FluentBoilerplateTestGroup"
	And I have not added the Windows user to the Active Directory group
	And I am using the application directory as the Active Directory context
	And I require access to an action by group "FluentBoilerplateTestGroup"
	And I perform an action that does not throw exceptions
	When I perform the action through the boilerplate context
	Then I should perform the action successfully

@Providers
Scenario: Windows identity restricts and has restricted application directory permissions
	Given I have created a Windows user named "FluentBoilerplateTestUser"
	And I have created an Active Directory group named "FluentBoilerplateTestGroup"
	And I have added the Windows user to the Active Directory group
	And I am using the application directory as the Active Directory context
	And I restrict access to an action by group "FluentBoilerplateTestGroup"
	And I perform an action that does not throw exceptions
	When I perform the action through the boilerplate context
	Then I should perform the action successfully

@Providers
Scenario: Windows identity restricts and does not have restricted application directory permissions
	Given I have created a Windows user named "FluentBoilerplateTestUser"
	And I have created an Active Directory group named "FluentBoilerplateTestGroup"
	And I have not added the Windows user to the Active Directory group
	And I am using the application directory as the Active Directory context
	And I restrict access to an action by group "FluentBoilerplateTestGroup"
	And I perform an action that does not throw exceptions
	When I perform the action through the boilerplate context
	Then I should perform the action successfully