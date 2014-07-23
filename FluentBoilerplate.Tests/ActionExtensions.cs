using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    internal static class ActionExtensions
    {
        public static Action<T> ToVerifiedAction<T>(this Action<T> action)
        {
            return x =>
                {
                    var hasBeenHit = false;

                };
        }
    }
}
