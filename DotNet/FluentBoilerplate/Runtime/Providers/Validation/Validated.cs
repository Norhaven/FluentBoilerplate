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
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Providers.Validation
{
    internal sealed class Validated<T>
    {
        public T Value { get; private set; }
        public IEnumerable<IValidationResult> Results { get; private set; }
        public bool IsValid { get; private set; }

        internal Validated(T value, IEnumerable<IValidationResult> results)
        {
            this.Value = value;
            this.Results = results;
            this.IsValid = (results.All(r => r.IsSuccess));
        }
    }
}
