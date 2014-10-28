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

using FluentBoilerplate.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Providers.ErrorHandling;
using FluentBoilerplate.Runtime.Providers.Logging;
using FluentBoilerplate.Runtime.Providers.Validation;
using System.Text.RegularExpressions;

namespace FluentBoilerplate.Runtime
{
    internal static class KnownMetadata
    {
        public static class Properties
        {
            public static readonly PropertyInfo String_Length = typeof(string).GetProperty("Length");
            
            public static readonly PropertyInfo CustomLoggableMember_OwningType = typeof(LogProvider.CustomLoggableMember).GetProperty("OwningType", BindingFlags.Instance | BindingFlags.Public);
            public static readonly PropertyInfo CustomLoggableMember_MemberName = typeof(LogProvider.CustomLoggableMember).GetProperty("MemberName", BindingFlags.Instance | BindingFlags.Public);
            public static readonly PropertyInfo CustomLoggableMember_MemberType = typeof(LogProvider.CustomLoggableMember).GetProperty("MemberType", BindingFlags.Instance | BindingFlags.Public);
            public static readonly PropertyInfo CustomLoggableMember_MemberValue = typeof(LogProvider.CustomLoggableMember).GetProperty("MemberValue", BindingFlags.Instance | BindingFlags.Public);

        }
        public static class Methods
        {
            public static readonly MethodInfo Action_Invoke = typeof(Action).GetMethod("Invoke", Type.EmptyTypes);

            public static readonly MethodInfo Log_LogCustomNameValuePairs = typeof(LogProvider).GetMethod("LogCustomNameValuePairs", new[] { typeof(IEnumerable<LogProvider.CustomLoggableMember>) });

            public static readonly MethodInfo ValidationResult_Failure = typeof(ValidationResult).GetMethod("Failure");
            public static readonly MethodInfo ValidationResult_Success = typeof(ValidationResult).GetMethod("Success");
            public static readonly MethodInfo ValidationResult_NotApplicable = typeof(ValidationResult).GetMethod("NotApplicable");

            public static readonly MethodInfo IExceptionAwareAction_Do = typeof(IExceptionAwareAction).GetMethod("Do");
            public static readonly MethodInfo IExceptionAwareAction_HandleException = typeof(IExceptionAwareAction).GetMethod("HandleException");

            public static readonly MethodInfo Object_ToString = typeof(object).GetMethod("ToString");

            public static readonly MethodInfo Regex_IsMatch = typeof(Regex).GetMethod("IsMatch", new[] { typeof(string), typeof(string) });

            public static MethodInfo ICustomValidator_OfType(Type type) { return typeof(ICustomValidator<>).MakeGenericType(type).GetMethod("Validate", new[] { type }); }
        }        

        public static class Constructors
        {
            public static readonly ConstructorInfo ValidationResult_New = typeof(ValidationResult).GetConstructor(new[] { typeof(ValidationKind), typeof(string), typeof(bool), typeof(bool) });
        }
    }

    public static class KnownMetadata<T>
    {
        public static class Constructors
        {
            public static readonly ConstructorInfo IList_New = typeof(IList<T>).GetConstructor(Type.EmptyTypes);
        }

        public static class Methods
        {
            public static readonly MethodInfo IList_Add = typeof(IList<T>).GetMethod("Add", new[] { typeof(T) });
            public static readonly MethodInfo Queue_Enqueue = typeof(Queue<T>).GetMethod("Enqueue", new[] { typeof(T) });
            public static readonly MethodInfo Action_Invoke = typeof(Action<T>).GetMethod("Invoke", new[] { typeof(T) });
            public static readonly MethodInfo Func_Invoke = typeof(Func<T>).GetMethod("Invoke", Type.EmptyTypes);            
        }
    }
}
