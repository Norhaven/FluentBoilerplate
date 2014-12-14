<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
</Query>
void Main()
{
  //Multi-threading can be a difficult thing to deal with and
  //you may end up with locking code that you have to be very careful about.

  //There are a few ways that you can alleviate your difficulties with FluentBoilerplate.
  //The simplest way is by using the Atomic<T> type to provide a quick lock around variable access.
  var atomicInt = Atomic<int>.New(5);
	
	//If you would like to do this through the boilerplate instead, you're also welcome to do that.
	var boilerplate = Boilerplate.New();
	var secondAtomicInt = boilerplate.Use(20).AsAtomic();
	
	//Both of these constitute a read that is locked around.
	int value = atomicInt;
	var explicitValue = atomicInt.Value;
	
	//This writes to the variable within a lock.
	atomicInt.Value = 10;
		
	//You may also want to group several atomics together into a transaction,
	//where all of them must be locked in order to proceed.
	boilerplate
		.BeginContract()
			.Restrict.Threads.ByTransaction
				.Of(atomicInt)
				.And(secondAtomicInt)
				.OrderedBy.Default
		.EndContract()
		.Do(context =>
		{
			//The only callers who will reach this point are those who have taken a lock
			//on both atomicInt and secondAtomicInt.
		});
		
	//You may have noticed that part where it says .OrderedBy.Default and wondered about it,
	//so let's go into that a little bit.
	
	//When taking locks on multiple instances, you absolutely MUST lock on them in the same order
	//every single time. If you don't, deadlocks can easily creep in and make your life difficult.
	
	//For example, one lock sequence locks A and then B.
	//Another lock sequence locks B and then A.
	//One thread may take the A lock and another thread may take the B lock, which leaves them both 
	//waiting for the other one to release their lock so execution can proceed.
	
	//The .OrderedBy call allows you to specify the order in which you would like the locks in this
	//particular transaction to take place. There are two options: Default and ParameterOrder.
	
	//Default means that the boilerplate will figure out the order based on the atomic locations
	//that were provided and keep the lock order consistent across all uses of those locations.
	
	//ParameterOrder means that the order in which you specified the atomic locations is also
	//the order in which they will be locked. In the example above, this would always
	//lock atomicInt and then secondAtomicInt.	
}