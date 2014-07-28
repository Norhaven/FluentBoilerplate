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

namespace FluentBoilerplate.Tests
{
    class SyntaxTestPlayground
    {
        public void Test()
        {
            IIdentity identity = null;
            var boilerplate = Boilerplate.New(identity);
            //SampleBoilerplate(boilerplate, null);
        }

        public class TestType
        {
            [MapsTo(typeof(Object), "Name")]
            [StringLength(MinLength=2)]
            public string Value { get; set; }
        }

        public class Example
        {
            public string Text { get; private set; }

            public void DoSomething(IContext boilerplate)
            {
                //boilerplate
                //    .BeginContract()
                //        .EnsureOnReturn(() => this.Text != null, "Text must be non-null on return")
                //        .EnsureOnThrow(() => this.Text == null, "Text must be null when an exception is thrown")
                //    .EndContract()
                //    .Do(context => /* Take some action */);
            }
        }
        //public static class KnownRights
        //{
        //    public static IRight CanPerformAction = new Right(1, "User can perform an action");
        //    public static IRight CanDoTerribleThings = new Right(2, "User can do terrible things");
        //}
        //public static class KnownRoles
        //{
        //    public static IRole BasicUser = new Role(1,
        //                                             "A user",
        //                                             new HashSet<IRight>
        //                                             {
        //                                                 KnownRights.CanPerformAction
        //                                             }.ToImmutableHashSet());

        //    public static IRole RestrictedUser = new Role(2,
        //                                                  "A user with limited access",
        //                                                  new HashSet<IRight>().ToImmutableHashSet());
        //}
        
        private void DoValidatedAction(IContext boilerplate)
        {           
            //boilerplate.BeginContract()
            //    .EndContract()
            //    .BeginContract()
            //    .Require(null)
            //    .Handles<Exception, int>("")
            //    .Handles<Exception>("")
            //    .Require()
            //    .EndContract()
            //    .Do((c,r) => {})
            //    .Get()
            //object returnValue = null;
            
            //var r = boilerplate
            //    .BeginContract()
            //        .Require(() => value != null, "Value cannot be null")
            //        .RequiresValidInstanceOf(value)
            //        .EnsureOnReturn(() => returnValue != null, "Return value must not be null")
            //        .EnsureOnThrow(() => returnValue != null, "HELP")
            //        .RequiresRights(null)
            //    .EndContract()
            //    .Get<int>(c => 
            //        {
            //            var result = c.As<object, int>(value);
            //        })
            //    .Result;
        }
    }
}
