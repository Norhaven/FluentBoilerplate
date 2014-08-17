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
using System.Collections.Immutable;

using FluentBoilerplate;
using NUnit.Framework;
using System.Diagnostics;
using FluentBoilerplate.Providers;
using FluentBoilerplate.Providers.Database;
using System.Data;

namespace FluentBoilerplate.Tests
{
    [TestFixture]
    public class SyntaxTestPlayground
    {        
        [Test]
        public void Test()
        {
            var boilerplate = Boilerplate.New(visibility: Visibility.Debug);

            var timings = boilerplate
                .BeginContract()
                    .IsTimedUnder(Visibility.Debug)
                .EndContract()
                .Do(_ => Go())
                .Do(_ => Go())
                .Do(_ => Go())
                .CallTimings;

            foreach (var timing in timings)
                Console.WriteLine(timing.TotalMilliseconds);
            //var provider = new AdoNetConnectionProvider("FluentBoilerplate", DataSource.SQL);
            //var access = new TypeAccessProvider(provider);
            //var boilerplate = Boilerplate.New(accessProvider: access);
            //boilerplate.Open<IAdoNetConnection>().AndDo((_, connection) =>
            //    {
            //        var parameter = connection.CreateParameter("count", 5);
            //        var result = connection.ExecuteStoredProcedure("GetPeople", new Interpreter(), parameter);
            //        foreach (var person in result)
            //            Console.WriteLine(person.Name + " " + person.Description);
            //    });
            //var boilerplate = Boilerplate.New(accessProvider: new TypeAccessProvider(TypeAccessProvider.w))
            //var me = Identity.CurrentWindowsUser;
            //var context = Boilerplate.New(me);
            //context.BeginContract()
            //    .MustNotHaveRoles(ActiveDirectoryGroup.Administrators)
            //    .EndContract()
            //    .Do(c =>
            //    {
            //        Console.WriteLine("Did not have admin role");
            //    });
           
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            //Go();
            //stopwatch.Stop();
            //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            //stopwatch.Reset();
            //stopwatch.Start();

            //Go();
            //stopwatch.Stop();
            //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }
        private TType CreateType<TType>()
        {
            return Activator.CreateInstance<TType>();
        }
        private static void Go()
        {
            IIdentity identity; IBoilerplateContext boilerplate;
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
