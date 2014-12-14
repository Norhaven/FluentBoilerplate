<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>
void Main()
{
  //You may get to a part of your code that you need to have a tighter control
  //on the number of threads concurrently accessing it. That's very easy to do.
  var boilerplate = Boilerplate.New();

  boilerplate
    .BeginContract()
      .Restrict.Threads.ToMaxOf(5)
    .EndContract()
    .Do(context =>
    {
      //This section is effectively throttled to a maximum
      //of five concurrently executing threads.
    });

  //If you would like to just maintain an exclusive lock around the section,
  //just use .ToMaxOf(1) and you're on your way.

  //You may just have a WaitHandle that you'd rather delegate control to instead.
  var resetEvent = new ManualResetEvent(false);

  boilerplate
    .BeginContract()
      .Restrict.Threads.ByWaitingFor(resetEvent, WaitTimeout.Of(1000))
    .EndContract()
    .Do(context =>
    {
      //All threads attempting to enter this section will have to wait
      //for the ManualResetEvent to be signalled before proceeding.
    });
}





