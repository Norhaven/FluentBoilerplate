﻿/*
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    public static class BoolExtensions
    {
        public static void FailTestIfTrue(this bool value)
        {
            if (value)
                throw new TestFailureException();
        }

        public static void FailTestIfFalse(this bool value)
        {
            if (!value)
                throw new TestFailureException();
        }

        [TestMethod]
        public void NewWithIdentityCreatesContextSuccessfully()
        {
            var context = Boilerplate.New(identity: Identity.Default);

            context.Should().NotBeNull("because you should always get a context back");
        }

        [TestMethod]
        public void NewWithTypeProviderCreatesContextSuccessfully()
        {

        }
    }
}
