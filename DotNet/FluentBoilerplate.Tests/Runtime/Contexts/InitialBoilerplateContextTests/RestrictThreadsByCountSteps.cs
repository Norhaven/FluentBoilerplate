using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace FluentBoilerplate.Tests.Runtime.Contexts.InitialBoilerplateContextTests
{
    [Binding]
    public class RestrictThreadsByCountSteps
    {
        private readonly TestContext testContext;

        public RestrictThreadsByCountSteps(TestContext testContext)
        {
            this.testContext = testContext;
        }

        [Given(@"I have restricted the thread count to (.*)")]
        public void GivenIHaveRestrictedTheThreadCountTo(int count)
        {
            this.testContext.Boilerplate =
                Boilerplate.New(visibility: Visibility.Debug)
                .BeginContract()
                    .Restrict.Threads.ToMaxOf(count)
                .EndContract();
        }

        [When(@"I execute an action using (.*) threads")]
        public void WhenIExecuteAnActionUsingThreads(int count)
        {
            //TODO: Make this whole thing more flexible and don't rely on hardcoded 1 for assumed max value
            //var verifier = new ThreadSynchronizationVerifier(1); 
            //var tasks = new List<Task>();
            //for(var i = 0; i < count; i++)
            //{
            //    var task = Task.Run(() => this.testContext.Boilerplate.Do(context =>
            //        {
            //            verifier.Verify();
            //        }));
            //    tasks.Add(task);
            //}

            //Task.WaitAll(tasks.ToArray());
        }

        [Then(@"only (.*) threads at a time may use the restricted section")]
        public void ThenOnlyThreadsAtATimeMayUseTheRestrictedSection(int count)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
