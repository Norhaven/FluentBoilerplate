using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    internal sealed class ThreadSynchronizationVerifier
    {
        private readonly object verifierLock = new object();
        private readonly Semaphore verifier;
        private readonly int maxCount;
        private int currentCount;

        public ThreadSynchronizationVerifier(int maxCount)
        {
            this.maxCount = maxCount;
            this.verifier = new Semaphore(this.maxCount, this.maxCount);
        }

        public void Verify()
        {
            lock(verifierLock)
            {
                if (this.currentCount >= this.maxCount)
                    throw new InvalidOperationException("The thread synchronization upstream is failing to restrict threads, reached or exceeded the max thread count");

                if (this.currentCount < this.maxCount)
                    this.currentCount++;

                var signalled = this.verifier.WaitOne();
                if (!signalled)
                    throw new InvalidOperationException("The semaphore and the current count should be in sync but they're not");

                Monitor.Wait(this.verifierLock, WaitTimeout.Of(500).Milliseconds);

                this.verifier.Release();
                this.currentCount--;
            }
        }
    }
}
