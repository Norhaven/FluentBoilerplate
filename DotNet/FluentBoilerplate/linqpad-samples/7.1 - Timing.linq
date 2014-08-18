<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>

void Main()
{
	//Every so often you'd like to see how long a particular call is taking.
	//You could profile it, which is highly recommended, but you may just want
	//to time it with the System.Diagnostics.Stopwatch class in one or two locations.
	
	//You can include that in your boilerplate contract as well.
	var boilerplate = Boilerplate.New(visibility: Visibility.Debug);
	
	var timings = boilerplate
		.BeginContract()
			.IsTimedUnder(Visibility.Debug)
		.EndContract()
		.Do(_ => SomeExpensiveCall())
		.Do(_ => SomeExpensiveCall())
		.CallTimings;
		
	foreach (var timing in timings)
		Console.WriteLine(timing.TotalMilliseconds);
}

private static void SomeExpensiveCall()
{
	//Please don't really do this
	Thread.Sleep(500);
}