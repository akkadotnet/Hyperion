#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializerOptions.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Hyperion.SerializerFactories;

namespace Hyperion
{
    public class SerializerOptions
    {
        internal static List<CrossPlatformPackageNameOverride> DefaultPackageNameOverrides()
        {
            return new List<CrossPlatformPackageNameOverride>
            {
#if NET45
                new CrossPlatformPackageNameOverride(
                    fingerprint: "System.Private.CoreLib,%core%",
                    from: "System.Private.CoreLib,%core%",
                    to: "mscorlib,%core%")
#elif NETSTANDARD
                new CrossPlatformPackageNameOverride(
                    fingerprint: "mscorlib,%core%",
                    from: "mscorlib,%core%",
                    to: "System.Private.CoreLib,%core%")
#endif
            };
        }

        internal static readonly Surrogate[] EmptySurrogates = new Surrogate[0];
        

        private static readonly ValueSerializerFactory[] DefaultValueSerializerFactories =
        {
            new ConsistentArraySerializerFactory(), 
            new MethodInfoSerializerFactory(),
            new PropertyInfoSerializerFactory(), 
            new ConstructorInfoSerializerFactory(),
            new FieldInfoSerializerFactory(),
            new DelegateSerializerFactory(), 
            new ToSurrogateSerializerFactory(),
            new FromSurrogateSerializerFactory(),
            new FSharpMapSerializerFactory(), 
            new FSharpListSerializerFactory(), 
            //order is important, try dictionaries before enumerables as dicts are also enumerable
            new ExceptionSerializerFactory(), 
            new ImmutableCollectionsSerializerFactory(),
            new ExpandoObjectSerializerFactory(),
            new DefaultDictionarySerializerFactory(),
            new DictionarySerializerFactory(),
            new ArraySerializerFactory(),
            new MultipleDimensionalArraySerialzierFactory(),
#if SERIALIZATION
            new ISerializableSerializerFactory(), //TODO: this will mess up the indexes in the serializer payload
#endif
            new EnumerableSerializerFactory(),
            
        };

        internal readonly bool IgnoreISerializable;
        internal readonly bool PreserveObjectReferences;
        internal readonly Surrogate[] Surrogates;
        internal readonly ValueSerializerFactory[] ValueSerializerFactories;
        internal readonly bool VersionTolerance;
        internal readonly Type[] KnownTypes;
        internal readonly Dictionary<Type, ushort> KnownTypesDict = new Dictionary<Type, ushort>();
        internal readonly List<CrossPlatformPackageNameOverride> CrossFrameworkPackageNameOverrides =
            DefaultPackageNameOverrides();

        public SerializerOptions(bool versionTolerance = false, bool preserveObjectReferences = false, IEnumerable<Surrogate> surrogates = null, IEnumerable<ValueSerializerFactory> serializerFactories = null, IEnumerable<Type> knownTypes = null, bool ignoreISerializable = false, IEnumerable<CrossPlatformPackageNameOverride> packageNameOverrides = null)
        {
            VersionTolerance = versionTolerance;
            Surrogates = surrogates?.ToArray() ?? EmptySurrogates;

            //use the default factories + any user defined
	        ValueSerializerFactories = serializerFactories == null
		        ? DefaultValueSerializerFactories
		        : serializerFactories.Concat(DefaultValueSerializerFactories).ToArray();

            KnownTypes = knownTypes?.ToArray() ?? new Type[] {};
            for (var i = 0; i < KnownTypes.Length; i++)
            {
                KnownTypesDict.Add(KnownTypes[i],(ushort)i);
            }

            PreserveObjectReferences = preserveObjectReferences;
            IgnoreISerializable = ignoreISerializable;

            if(packageNameOverrides != null)
                CrossFrameworkPackageNameOverrides.AddRange(packageNameOverrides);
        }
    }
    public class CrossPlatformPackageNameOverride
    {
        public CrossPlatformPackageNameOverride(string fingerprint, string @from, string to)
        {
            Fingerprint = fingerprint;
            RenameFrom = @from;
            RenameTo = to;
        }

        public string Fingerprint { get; }
        public string RenameFrom { get; }
        public string RenameTo { get; }
    }
}