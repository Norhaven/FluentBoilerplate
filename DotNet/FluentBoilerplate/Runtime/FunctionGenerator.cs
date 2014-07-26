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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;

namespace FluentBoilerplate.Runtime
{
    internal sealed class FunctionGenerator : IFunctionGenerator
    {
        internal sealed class PhysicalAssemblySettings
        {
            public string Name { get; private set; }
            public string Extension { get; private set; }
            public string DirectoryPath { get; private set; }

            public string NameWithExtension { get { return String.Format("{0}.{1}", this.Name, this.Extension); } }
            public string FullPath { get { return Path.Combine(this.DirectoryPath, this.NameWithExtension); } }

            public PhysicalAssemblySettings(string name, string extension, string directoryPath)
            {
                this.Name = name;
                this.Extension = extension;
                this.DirectoryPath = directoryPath;
            }
        }

        public static IFunctionGenerator Default { get { return new FunctionGenerator(); } }

        private readonly bool createPhysicalAssembly;
        private readonly PhysicalAssemblySettings assemblySettings;

        internal FunctionGenerator(PhysicalAssemblySettings assemblySettings = null)
        {
            this.createPhysicalAssembly = assemblySettings != null;
            this.assemblySettings = assemblySettings;
        }

        public Action<T1, T2> CreateAction<T1, T2>(Action<ILWriter> writeActionBody)
        {
            return Create<Action<T1, T2>>(typeof(void), new[] { typeof(T1), typeof(T2) }, writeActionBody);
        }

        public Action<T> CreateAction<T>(Action<ILWriter> writeActionBody)
        {
            return Create<Action<T>>(typeof(void), new[] { typeof(T) }, writeActionBody);
        }

        public Func<TIn, TOut> Create<TIn, TOut>(Action<ILWriter> writeFunctionBody)
        {
            return Create<Func<TIn, TOut>>(typeof(TOut), new[] { typeof(TIn) }, writeFunctionBody);
        }
        
        public Func<TIn1, TIn2, TOut> Create<TIn1, TIn2, TOut>(Action<ILWriter> writeFunctionBody)
        {
            return Create<Func<TIn1, TIn2, TOut>>(typeof(TOut), new[] { typeof(TIn1), typeof(TIn2) }, writeFunctionBody);
        }

        private TFunction Create<TFunction>(Type returnType, Type[] parameterTypes, Action<ILWriter> writeFunctionBody)
        {
            if (this.createPhysicalAssembly)
            {
                if (File.Exists(this.assemblySettings.FullPath))
                    File.Delete(this.assemblySettings.FullPath);

                var assemblyName = new AssemblyName(this.assemblySettings.Name);
                var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, this.assemblySettings.DirectoryPath);

                var module = assembly.DefineDynamicModule("OutputModule", this.assemblySettings.NameWithExtension);

                var type = module.DefineType("GeneratedFunction", TypeAttributes.Public | TypeAttributes.Class);

                var method = type.DefineMethod("Call",
                                               MethodAttributes.Public | MethodAttributes.Static,
                                               returnType,
                                               parameterTypes);

                var generator = method.GetILGenerator();
                var writer = new ILWriter(generator);
                writeFunctionBody(writer);

                var concreteType = type.CreateType();
                assembly.Save(this.assemblySettings.NameWithExtension);

                var instance = Activator.CreateInstance(concreteType);
                var call = concreteType.GetMethod("Call");

                //TFunction is typed so the easiest way to call it in a typed manner is to just
                //funnel everything through a passthrough typed method.

                var callingMethod = new DynamicMethod(String.Empty, returnType, parameterTypes);
                var callingGenerator = callingMethod.GetILGenerator();
                var callingWriter = new ILWriter(callingGenerator);

                callingWriter.LoadParameterRange(0, parameterTypes.Length);
                callingWriter.StaticMethodCall(call);
                callingWriter.Return();

                return callingMethod.Create<TFunction>();           
            }
            else
            {
                var method = new DynamicMethod(String.Empty, returnType, parameterTypes);
                var generator = method.GetILGenerator();
                var writer = new ILWriter(generator);
                writeFunctionBody(writer);
                return method.Create<TFunction>();
            }
        }
    }
}
