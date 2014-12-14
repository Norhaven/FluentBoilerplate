<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
</Query>

void Main()
{
	//After all of this, it might occur to you that some visibility into things might be nice.
	//That's where logging comes in. It relies on specific attributes that you can place on your objects.
	
	//Currently, logging occurs at various points in the boilerplate and at different visibility levels.
	//The default log provider will write to Trace, so feel free to add your own TraceListener to hook into it.
	
	//More ways of logging are planned!
}

[Log] //Indicates that this type can be logged
public class Loggable
{
	//This usage indicates that this property will be logged under all conditions
	[Log] 
	public string Name { get; set; }
	
	//This usage indicates that this property will be called "Trace" when logged and will only be logged when watching for debug or info messages.
	[Log(Name = "Trace", Visibility = Visibility.Debug | Visibility.Info)]
	public string TraceData { get; set; }
	
	//No attribute indicates that this property will not be logged.
	public int Value { get; set; }
	
	//If you would like to be specific about it, though, this usage is also acceptable.
	[Log(Ignore=true)]
	public long SpecialValue { get; set; }
}

