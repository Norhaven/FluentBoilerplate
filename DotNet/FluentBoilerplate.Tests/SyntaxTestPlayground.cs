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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

using FluentBoilerplate;
using NUnit.Framework;
using System.Diagnostics;

namespace FluentBoilerplate.Tests
{
    [TestFixture]
    public class SyntaxTestPlayground
    {

        [Test]
        public void Test()
        {
            var adrole = new Role(0, "Administrators", "", null, PermissionsSource.ActiveDirectory);
            var me = Identity.CurrentWindowsUser;
            var context = Boilerplate.New(me);
            context.BeginContract()
                .MustNotHaveRoles(adrole)
                .EndContract()
                .Do(c =>
                {
                    Debugger.Break();
                });

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Go();
            stopwatch.Stop();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
            stopwatch.Start();

            Go();
            stopwatch.Stop();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        private static void Go()
        {
            IIdentity identity; IContext boilerplate;
            identity = null;
            boilerplate = Boilerplate.New(identity);

            boilerplate
                .BeginContract()
                    .Handles<ArgumentException>(ex => System.Diagnostics.Debug.WriteLine("Caught ArgumentException"))
                    .Handles<Exception>(ex => System.Diagnostics.Debug.WriteLine("Caught Exception"))
                .EndContract()
                .Do(context =>
                {
                    throw new Exception();
                });
        }
    }
}
