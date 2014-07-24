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
                                               typeof(void),
                                               new[] { typeof(T1), typeof(T2) });

                var generator = method.GetILGenerator();
                var writer = new ILWriter(generator);
                writeActionBody(writer);

                var concreteType = type.CreateType();
                assembly.Save(this.assemblySettings.NameWithExtension);

                var instance = Activator.CreateInstance(concreteType);
                var instanceMethod = concreteType.GetMethod("Call");
                return (p1, p2) => instanceMethod.Invoke(instance, new object[] { p1, p2 });
            }
            else
            {
                var method = new DynamicMethod(String.Empty, typeof(void), new[] { typeof(T1), typeof(T2) });
                var generator = method.GetILGenerator();
                var writer = new ILWriter(generator);
                writeActionBody(writer);
                return method.Create<Action<T1, T2>>();
            }
        }

        public Action<T> CreateAction<T>(Action<ILWriter> writeActionBody)
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
                                               typeof(void),
                                               new[] { typeof(T) });

                var generator = method.GetILGenerator();
                var writer = new ILWriter(generator);
                writeActionBody(writer);

                var concreteType = type.CreateType();
                assembly.Save(this.assemblySettings.NameWithExtension);

                var instance = Activator.CreateInstance(concreteType);
                var instanceMethod = concreteType.GetMethod("Call");
                return x => instanceMethod.Invoke(instance, new object[] { x });             
            }
            else
            {
                var method = new DynamicMethod(String.Empty, typeof(void), new[] { typeof(T) });
                var generator = method.GetILGenerator();
                var writer = new ILWriter(generator);
                writeActionBody(writer);
                return method.Create<Action<T>>();
            }
        }

        public Func<TIn, TOut> Create<TIn, TOut>(Action<ILWriter> writeFunctionBody)
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
                                               typeof(TOut),
                                               new[] { typeof(TIn) });

                var generator = method.GetILGenerator();
                var writer = new ILWriter(generator);
                writeFunctionBody(writer);

                var concreteType = type.CreateType();
                assembly.Save(this.assemblySettings.NameWithExtension);

                var instance = Activator.CreateInstance(concreteType);
                var instanceMethod = concreteType.GetMethod("Call");
                return x => (TOut)instanceMethod.Invoke(instance, new object[] { x });
                //return (Func<TIn, TOut>)method.CreateDelegate(typeof(Func<TIn, TOut>), instance);                
            }
            else
            {
                var method = new DynamicMethod(String.Empty, typeof(TOut), new[] { typeof(TIn) });
                var generator = method.GetILGenerator();
                var writer = new ILWriter(generator);
                writeFunctionBody(writer);
                return method.Create<Func<TIn, TOut>>();
            }
        }


        public Func<TIn1, TIn2, TOut> Create<TIn1, TIn2, TOut>(Action<ILWriter> writeFunctionBody)
        {
            //TODO: Consolidate the implementation of Create() above into common that any number of
            //      overloads could take advantage of and use that here.
            throw new NotImplementedException();
        }
    }
}
