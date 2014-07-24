using FluentBoilerplate.Runtime.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    class SyntaxTestPlayground
    {
        public void Test()
        {
            IIdentity identity = null;
            var boilerplate = Boilerplate.New(identity);
            SampleBoilerplate(boilerplate, null);
        }

        public class TestType
        {
            [MapsTo(typeof(Object), "Name")]
            [StringLength(MinLength=2)]
            public string Value { get; set; }
        }

        private void SampleBoilerplate(IBoilerplateContext boilerplate, object value)
        {
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
