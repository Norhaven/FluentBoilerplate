/*
   Copyright 2015 Chris Hannon

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class TimingContext
    {       
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly Visibility visibility;
        private readonly Visibility[] possibleTimingFlags = new[] { Visibility.Debug, Visibility.Info, Visibility.Warning, Visibility.Error };
        private bool hasTimingInformation;

        public TimeSpan Elapsed { get { return this.stopwatch.Elapsed; } }
        public bool HasTimingInformation { get { return this.hasTimingInformation; } }

        public TimingContext(Visibility visibility)
        {
            this.visibility = visibility;
        }

        public IImmutableQueue<TimeSpan> EnqueueElapsed(IImmutableQueue<TimeSpan> currentTimes)
        {
            if (!this.hasTimingInformation)
                return currentTimes;

            return currentTimes.Enqueue(this.Elapsed);
        }
        
        public void OpenAs(Visibility callerVisibility, Action call)
        {
            if (this.visibility == Visibility.None)
                call();

            if (!AnyFlagInSequenceIsPresentForBoth(callerVisibility, this.visibility, this.possibleTimingFlags))
                call();

            this.stopwatch.Start();
            try
            {
                call();
            }
            finally
            {
                this.stopwatch.Stop();
                this.hasTimingInformation = true;
            }
        }

        private bool AnyFlagInSequenceIsPresentForBoth(Visibility visibility, Visibility otherVisibility, params Visibility[] flags)
        {
            if (flags == null)
                return false;

            foreach (var flag in flags)
            {
                if (FlagIsPresentInBoth(visibility, otherVisibility, flag))
                    return true;
            }

            return false;
        }

        private bool FlagIsPresentInBoth(Visibility visibility, Visibility otherVisibility, Visibility flag)
        {
            return (visibility.HasFlag(flag) && otherVisibility.HasFlag(flag));
        }
    }
}
