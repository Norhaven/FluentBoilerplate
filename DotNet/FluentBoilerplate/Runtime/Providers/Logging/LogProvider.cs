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
using FluentBoilerplate.Runtime.Extensions;
using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics;
using FluentBoilerplate;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Providers;

namespace FluentBoilerplate.Runtime.Providers.Logging
{
#if DEBUG
    public 
#else
    internal
#endif
        sealed class LogProvider : CacheProvider<Type, Action<object, Action<LogProvider.CustomLoggableMember>>>, ILogProvider
    {
        public static ILogProvider Empty { get { return new LogProvider(FunctionGenerator.Default, LogVisibility.All); } }

        private const string DebugCategory = "DEBUG";
        private const string InfoCategory = "INFO";
        private const string WarningCategory = "WARNING";
        private const string ErrorCategory = "ERROR";

        private readonly LogVisibility visibility;
        private readonly IFunctionGenerator functionGenerator;

        public LogProvider(IFunctionGenerator functionGenerator, LogVisibility visibility)
        {
            this.functionGenerator = functionGenerator;
            this.visibility = visibility;
        }

        private bool VisibilityIs(LogVisibility possibleVisibility)
        {
            return this.visibility.HasFlag(possibleVisibility);
        }

        private string CreateCategory(params string[] categories)
        {
            return String.Format("[{0}]", String.Join(", ", categories));
        }
        
        public void Info(string message, params object[] instances)
        {
            if (!VisibilityIs(LogVisibility.Error))
                return;

            Trace.WriteLine(message, CreateCategory(InfoCategory));

            WriteInstancesToLog(instances);
        }

        public void Error(string message, Exception exception, params object[] instances)
        {
            if (!VisibilityIs(LogVisibility.Error))
                return;
            
            Trace.WriteLine(message, CreateCategory(ErrorCategory));

            if (exception != null)
            {
                Trace.WriteLine(String.Format("Type:{1}{0}Message:{2}{0}Stack Trace:{3}{0}",
                                              Environment.NewLine,
                                              exception.GetType().FullName,
                                              exception.Message,
                                              exception.StackTrace));
            }

            WriteInstancesToLog(instances);
        }

        public void Warning(string message, params object[] instances)
        {
            if (!VisibilityIs(LogVisibility.Warning))
                return;

            Trace.WriteLine(message, CreateCategory(WarningCategory));

            WriteInstancesToLog(instances);
        }

        public void Debug(string message, params object[] instances)
        {
            if (!VisibilityIs(LogVisibility.Debug))
                return;

            Trace.WriteLine(message, CreateCategory(DebugCategory));

            WriteInstancesToLog(instances);
        }

        private void WriteInstancesToLog(object[] instances)
        {            
            if (!instances.HasContents())
                return;

            foreach (var instance in instances)
            {
                var log = GetTypeLogger(instance.GetType());
                log(instance, this.LogCustomNameValuePairs);
            }
        }

        private Action<object, Action<CustomLoggableMember>> GetTypeLogger(Type type)
        {
            return GetOrAdd(type, _ => CreateTypeLogger(type));
        }

        internal Action<object, Action<CustomLoggableMember>> CreateTypeLogger(Type type)
        {
            var logAttributes = type.GetAttributesOf<LogAttribute>();

            if (!logAttributes.Any())
                return null;

            var loggableMembers = type.GetMembers(BindingFlags.FlattenHierarchy | 
                                                  BindingFlags.Public | 
                                                  BindingFlags.NonPublic | 
                                                  BindingFlags.Instance);

            if (!loggableMembers.HasContents())
                return null;

            return this.functionGenerator.CreateAction<object, Action<CustomLoggableMember>>(writer => WriteLogBody(writer, type, loggableMembers));
        }

        private bool CanWriteToLog(LogVisibility memberVisibility)
        {
            if (HasSameVisibility(LogVisibility.Debug, memberVisibility) ||
                HasSameVisibility(LogVisibility.Error, memberVisibility) ||
                HasSameVisibility(LogVisibility.Info, memberVisibility) ||
                HasSameVisibility(LogVisibility.Warning, memberVisibility))
            {
                return true;
            }

            return false;
        }

        private bool HasSameVisibility(LogVisibility visibility, LogVisibility memberVisibility)
        {
            return this.VisibilityIs(visibility) && memberVisibility.HasFlag(visibility);
        }

        private void WriteLogBody(ILWriter writer, Type type, MemberInfo[] loggableMembers)
        {
            var localTypedInstance = writer.DeclareLocal(type);
            writer.LoadFirstParameter();
            writer.Cast(typeof(object), type);
            writer.SetLocal(localTypedInstance);

            foreach (var member in loggableMembers)
            {
                var logAttributes = member.GetCustomAttributes<LogAttribute>().ToArray();

                if (logAttributes.Length == 0)
                    continue;

                //TODO: Support multiple attributes per member
                var attribute = logAttributes[0];

                if (attribute.Ignore)
                    continue;

                if (CanWriteToLog(attribute.Visibility))
                {
                    LocalBuilder localValue;
                    LocalBuilder localLogItem = writer.DeclareLocal<CustomLoggableMember>();

                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            var field = (FieldInfo)member;
                            localValue = writer.DeclareLocal(field.FieldType);

                            writer.LoadVariable(localTypedInstance);
                            writer.LoadField(field);
                            writer.SetLocal(localValue);
                            break;
                        case MemberTypes.Property:
                            var property = (PropertyInfo)member;
                            localValue = writer.DeclareLocal(property.PropertyType);

                            writer.LoadVariable(localTypedInstance);
                            writer.GetPropertyValue(property);
                            writer.SetLocal(localValue);
                            break;
                        default:
                            var message = String.Format("Encountered loggable member '{0}' that was not supported in IL generation", member.Name);
                            System.Diagnostics.Debug.Fail(message);
                            throw new ILGenerationException(message);
                    }

                    writer.New<CustomLoggableMember>();
                    writer.SetLocal(localLogItem);

                    writer.LoadVariable(localLogItem);
                    writer.LoadInt32((int)attribute.Visibility);
                    writer.SetPropertyValue(typeof(CustomLoggableMember).GetProperty("MemberVisibility"));

                    writer.LoadVariable(localLogItem);
                    writer.TypeOf(type);
                    writer.Cast(localValue.LocalType, typeof(Type));
                    writer.SetPropertyValue(KnownMetadata.Properties.CustomLoggableMember_OwningType);

                    writer.LoadVariable(localLogItem);
                    writer.LoadString(member.Name);
                    writer.SetPropertyValue(KnownMetadata.Properties.CustomLoggableMember_MemberName);

                    writer.LoadVariable(localLogItem);
                    writer.TypeOf(localValue.LocalType);
                    writer.Cast(localValue.LocalType, typeof(Type));
                    writer.SetPropertyValue(KnownMetadata.Properties.CustomLoggableMember_MemberType);

                    writer.LoadVariable(localLogItem);
                    writer.LoadVariable(localValue);
                    writer.SetPropertyValue(KnownMetadata.Properties.CustomLoggableMember_MemberValue);

                    writer.LoadSecondParameter();
                    writer.LoadVariable(localLogItem);
                    writer.ActionDelegateMethodCall<CustomLoggableMember>();
                }
            }

            writer.Return();
        }

        private Type GetMemberType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property: return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Field: return ((FieldInfo)member).FieldType;
                default:
                    var message = String.Format("Encountered member '{0}' that was of undetermined type", member.Name);                    
                    throw new ILGenerationException(message);
            }
        }

        public sealed class CustomLoggableMember
        {
            public LogVisibility MemberVisibility { get; set; }
            public Type OwningType { get; set; }
            public string MemberName { get; set; }
            public Type MemberType { get; set; }
            public object MemberValue { get; set; }
            public CustomLoggableMember() { }
        }

        public void LogCustomNameValuePairs(CustomLoggableMember member)
        {
            var visibilities = new List<string>();

            if (member.MemberVisibility.HasFlag(LogVisibility.Debug))
                visibilities.Add("DEBUG");

            if (member.MemberVisibility.HasFlag(LogVisibility.Info))
                visibilities.Add("INFO");

            if (member.MemberVisibility.HasFlag(LogVisibility.Warning))
                visibilities.Add("WARNING");

            if (member.MemberVisibility.HasFlag(LogVisibility.Error))
                visibilities.Add("ERROR");

            var category = String.Format("[{0}]", String.Join(", ", visibilities));

            var message = String.Format("{0}.{1} = {2} {3}",
                                        member.OwningType.FullName,
                                        member.MemberName,
                                        member.MemberType.FullName,
                                        member.MemberValue);

            Trace.WriteLine(message, category);
        }
    }
}
