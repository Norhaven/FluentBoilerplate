using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Extensions
{
    internal static class NullableExtensions
    {
        public static T GetValueOrDefault<T>(this T? value) where T:struct
        {
            if (value == null)
                return default(T);
            return value.Value;
        }
    }
}
