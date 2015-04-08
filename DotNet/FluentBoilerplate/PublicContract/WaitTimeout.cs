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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents a duration for waiting.
    /// </summary>
    public struct WaitTimeout
    {
        /// <summary>
        /// Gets a timeout value used to specify an infinite waiting period.
        /// </summary>
        public static WaitTimeout Infinite { get { return new WaitTimeout(Timeout.Infinite); } }
        /// <summary>
        /// Gets a timeout value used to specify a waiting period defined in milliseconds.
        /// </summary>
        public static WaitTimeout Of(int milliseconds) { return new WaitTimeout(milliseconds); }
        /// <summary>
        /// Gets a timeout value used to specify a waiting period defined by a <see cref="System.TimeSpan"/> instance.
        /// </summary>
        public static WaitTimeout Of(TimeSpan timeSpan) { return new WaitTimeout(timeSpan.Milliseconds); }

        public static implicit operator WaitTimeout(int millisecondsWait)
        {
            return new WaitTimeout(millisecondsWait);
        }

        public static implicit operator int(WaitTimeout waitTimeout)
        {
            return waitTimeout.millisecondsWait;
        }

        public static implicit operator WaitTimeout(TimeSpan timespanWait)
        {
            return new WaitTimeout(timespanWait.Milliseconds);
        }

        public static implicit operator TimeSpan(WaitTimeout waitTimeout)
        {
            return TimeSpan.FromMilliseconds(waitTimeout.millisecondsWait);
        }

        private readonly int millisecondsWait;

        /// <summary>
        /// Gets the number of milliseconds in this waiting period.
        /// </summary>
        public int Milliseconds
        {
            get
            {
                //A default struct init is treated as an infinite timeout
                if (this.millisecondsWait == 0) 
                    return Timeout.Infinite;
                return this.millisecondsWait;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.TimeSpan"/> instance that represents this waiting period.
        /// </summary>
        public TimeSpan Span
        {
            get
            {
                //A default struct init is treated as an infinite timeout
                if (this.millisecondsWait == 0)
                    return Timeout.InfiniteTimeSpan;
                return TimeSpan.FromMilliseconds(this.millisecondsWait);
            }
        }

        private WaitTimeout(int millisecondsWait)
        {
            this.millisecondsWait = millisecondsWait;
        }
    }
}
