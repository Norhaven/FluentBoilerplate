/*
   Copyright 2015 Chris Hannon

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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;
using System.Collections.Immutable;

namespace FluentBoilerplate.Testing
{
    public sealed class TranslationVerifier<TFrom>
    {
        private readonly Lazy<IImmutableSet<PropertyInfo>> sourceProperties;

        public TranslationVerifier()
        {
            this.sourceProperties = new Lazy<IImmutableSet<PropertyInfo>>(() => GetProperties<TFrom>());            
        }

        public IEnumerable<string> VerifyWithTargetOf<TTo>()
        {
            var target = new HashSet<string>(from p in GetProperties<TTo>() select p.Name);
            
            var sourceMapped = new HashSet<string>();
            var pendingVerifications = ImmutableQueue<Tuple<PropertyInfo, MapsToAttribute>>.Empty;
            var unverifiedSourceMapped = new HashSet<string>();

            foreach (var property in this.sourceProperties.Value)
            {
                if (property.GetMethod == null || !property.GetMethod.IsPublic)
                    yield return CreateReadableMapFailure(FailureReason.SourcePropertyDoesNotHaveGet, property);

                var mapsTo = (from p in property.GetAttributesOf<MapsToAttribute>()
                              where p.MappedType == typeof(TTo)
                              select p).FirstOrDefault();

                if (mapsTo == null)
                {
                    unverifiedSourceMapped.Add(property.Name);
                }
                else
                {
                    //Treat the source property as having a verified mapping
                    //because it's currently known where it should ultimately go.
                    sourceMapped.Add(property.Name);
                    var pendingMap = new Tuple<PropertyInfo, MapsToAttribute>(property, mapsTo);
                    pendingVerifications = pendingVerifications.Enqueue(pendingMap);
                }
            }
            
            var verification = VerifyPropertyMap<TTo>(target.ToImmutableHashSet(),
                                                      sourceMapped.ToImmutableHashSet(),
                                                      unverifiedSourceMapped.ToImmutableHashSet(),
                                                      pendingVerifications);

            foreach (var messsage in verification)
                yield return messsage;
        }

        private IEnumerable<string> VerifyPropertyMap<TTo>(IImmutableSet<string> target,
                                                           IImmutableSet<string> sourceMapped,
                                                           IImmutableSet<string> unverifiedSourceMapped,
                                                           ImmutableQueue<Tuple<PropertyInfo, MapsToAttribute>> pendingVerifications)
        {
            var targetMapped = new HashSet<string>();

            //Wrap up any pending verifications first
            foreach (var map in pendingVerifications)
            {
                var targetPropertyName = map.Item2.PropertyName;
                if (target.Contains(targetPropertyName))
                {
                    var targetProperty = typeof(TTo).GetProperty(targetPropertyName);
                    if (targetProperty == null)
                        yield return CreateReadableMapFailure(FailureReason.SourcePropertyIsNotMapped, map.Item1);

                    if (targetProperty.SetMethod == null || !targetProperty.SetMethod.IsPublic)
                        yield return CreateReadableMapFailure(FailureReason.TargetPropertyDoesNotHaveSet, map.Item1, targetProperty);

                    if (targetMapped.Contains(targetPropertyName))
                        yield return CreateReadableMapFailure(FailureReason.TargetPropertyHasMultipleSources, targetProperty);

                    //TODO: Verify type compatibility

                    sourceMapped = sourceMapped.Add(map.Item1.Name);
                    targetMapped.Add(targetPropertyName);
                }
                else
                    yield return CreateReadableMapFailure(FailureReason.SourcePropertyIsNotMapped, map.Item1);
            }

            //Resolve the implicit mappings            
            var missingExpectedTargetProperties = target.Except(targetMapped).Except(unverifiedSourceMapped);
            var missingExpectedSourceProperties = unverifiedSourceMapped.Except(target);

            foreach (var name in missingExpectedTargetProperties)
            {
                var targetProperty = typeof(TTo).GetProperty(name);
                yield return CreateReadableMapFailure(FailureReason.TargetPropertyIsNotMapped, targetProperty);
            }

            foreach (var name in missingExpectedSourceProperties)
            {
                var sourceProperty = typeof(TFrom).GetProperty(name);
                yield return CreateReadableMapFailure(FailureReason.SourcePropertyIsNotMapped, sourceProperty);
            }

            var solidifiedMappings = unverifiedSourceMapped.Intersect(target);

            foreach(var name in solidifiedMappings)
            {
                //We could potentially have a mapping collision if someone already explicitly mapped a property
                //to one that was already being mapped, so make sure that's not the case.
                if (targetMapped.Contains(name))
                {
                    var targetProperty = typeof(TTo).GetProperty(name);
                    yield return CreateReadableMapFailure(FailureReason.TargetPropertyHasMultipleSources, targetProperty);
                }  
                else
                {
                    var targetProperty = typeof(TTo).GetProperty(name);
                    if (targetProperty.SetMethod == null || !targetProperty.SetMethod.IsPublic)
                        yield return CreateReadableMapFailure(FailureReason.TargetPropertyDoesNotHaveSet, targetProperty);

                    targetMapped.Add(name);
                }
            }
        }

        private enum FailureReason
        {
            SourcePropertyDoesNotHaveGet,
            SourcePropertyIsNotMapped,
            TargetPropertyDoesNotHaveSet,
            TargetPropertyIsNotMapped,
            TargetPropertyHasMultipleSources
        }

        private static string CreateReadableMapFailure(FailureReason reason, PropertyInfo property)
        {
            var propertyValues = new Tuple<string, string>(property.ReflectedType.Name, property.Name);
            switch(reason)
            {
                case FailureReason.SourcePropertyDoesNotHaveGet:
                case FailureReason.SourcePropertyIsNotMapped:
                     return CreateReadableMapFailure(reason, sourceProperty: propertyValues);
                case FailureReason.TargetPropertyDoesNotHaveSet:
                case FailureReason.TargetPropertyHasMultipleSources:
                case FailureReason.TargetPropertyIsNotMapped:
                     return CreateReadableMapFailure(reason, targetProperty:propertyValues);
                default:
                    throw new ArgumentException("Encountered unknown failure reason", "reason");
            }           
        }

        private static string CreateReadableMapFailure(FailureReason reason, PropertyInfo source, PropertyInfo target)
        {
            var sourceValues = new Tuple<string, string>(source.ReflectedType.Name, source.Name);
            var targetValues = new Tuple<string, string>(target.ReflectedType.Name, target.Name);

            return CreateReadableMapFailure(reason, sourceValues, targetValues);
        }

        private static string CreateReadableMapFailure(FailureReason reason,
                                                       Tuple<string,string> sourceProperty = null,
                                                       Tuple<string, string> targetProperty = null)
        {
            string header;

            var hasSource = sourceProperty != null;
            var hasTarget = targetProperty != null;

            if (hasSource && hasTarget)
            {
                header = "{0}.{1} -> {2}.{3}".WithValues(sourceProperty.Item1,
                                                             sourceProperty.Item2,
                                                             targetProperty.Item1,
                                                             targetProperty.Item2);
            }
            else if (hasSource)
            {
                header = "{0}.{1}".WithValues(sourceProperty.Item1, sourceProperty.Item2);
            }
            else if (hasTarget)
            {
                header = "{0}.{1}".WithValues(targetProperty.Item1, targetProperty.Item2);
            }
            else
            {
                throw new ArgumentException("At least one property must be provided to write a verification result");
            }
            
            string message;

            switch (reason)
            {
                case FailureReason.SourcePropertyDoesNotHaveGet: message = "Source property has no publicly exposed get method"; break;
                case FailureReason.SourcePropertyIsNotMapped: message = "Source property is not mapped to a target property"; break;
                case FailureReason.TargetPropertyDoesNotHaveSet: message = "Target property has no publicly exposed set method"; break;
                case FailureReason.TargetPropertyIsNotMapped: message = "Target property is not mapped from a source property"; break;
                case FailureReason.TargetPropertyHasMultipleSources: message = "Target property is mapped to from multiple sources"; break;
                default:
                    throw new ArgumentException("Unknown failure reason found during map verification", "reason");
            }

            return "{0} : {1}".WithValues(header, message);
        }

        private IImmutableSet<PropertyInfo> GetProperties<T>()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            return new HashSet<PropertyInfo>(properties).ToImmutableHashSet();
        }

        private IImmutableSet<PropertyInfo> FilterMappedProperties(IEnumerable<PropertyInfo> properties)
        {
            var mapping = new HashSet<PropertyInfo>();

            foreach (var property in properties)
            {
                var attributes = property.GetAttributesOf<MapsToAttribute>().ToArray();

                if (attributes.Length == 0)
                    continue;

                mapping.Add(property);
            }

            return mapping.ToImmutableHashSet();
        }
    }
}
