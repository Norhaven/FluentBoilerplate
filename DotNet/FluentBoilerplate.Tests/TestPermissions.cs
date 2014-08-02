/*
   Copyright 2014 Chris Hannon

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    public sealed class TestPermissions
    {
        public static readonly IRight CanUseBasicThings = new Right(1, "Can use basic things");

        public static readonly IImmutableSet<IRight> BasicRights = new IRight[] { TestPermissions.CanUseBasicThings }.ToImmutableHashSet();
        public static readonly IImmutableSet<IRight> NoRights = new IRight[0].ToImmutableHashSet();
    }
}
