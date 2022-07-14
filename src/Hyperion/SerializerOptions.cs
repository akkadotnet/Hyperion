﻿#region copyright
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
        public static readonly SerializerOptions Default = new SerializerOptions(
            versionTolerance: false,
            preserveObjectReferences: false,
            surrogates: null,
            serializerFactories: null,
            knownTypes: null,
            ignoreISerializable: false,
            packageNameOverrides: null,
            disallowUnsafeTypes: true,
            typeFilter: null);

        internal static List<Func<string, string>> DefaultPackageNameOverrides()
        {
            return new List<Func<string, string>>
            {
#if NETFX
                str => str.Contains("System.Private.CoreLib,%core%")
                    ? str.Replace("System.Private.CoreLib,%core%", "mscorlib,%core%") 
                    : str
#else
                str => str.Contains("mscorlib,%core%")
                    ? str.Replace("mscorlib,%core%", "System.Private.CoreLib,%core%") 
                    : str
#endif
            };
        }

        private static Surrogate[] _emptySurrogate;
        internal static Surrogate[] EmptySurrogates
        {
            get
            {
                if (_emptySurrogate == null)
                    _emptySurrogate = new Surrogate[0];
                return _emptySurrogate;
            }
        }

        private static ValueSerializerFactory[] _defaultValueSerializerFactories;
        private static ValueSerializerFactory[] DefaultValueSerializerFactories
        {
            get
            {
                if(_defaultValueSerializerFactories == null)
                    _defaultValueSerializerFactories = new ValueSerializerFactory[]
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
                        new AggregateExceptionSerializerFactory(),
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
                return _defaultValueSerializerFactories;
            }
        }

        internal readonly bool IgnoreISerializable;
        internal readonly bool PreserveObjectReferences;
        internal readonly Surrogate[] Surrogates;
        internal readonly ValueSerializerFactory[] ValueSerializerFactories;
        internal readonly bool VersionTolerance;
        internal readonly Type[] KnownTypes;
        internal readonly Dictionary<Type, ushort> KnownTypesDict = new Dictionary<Type, ushort>();
        internal readonly List<Func<string, string>> CrossFrameworkPackageNameOverrides = DefaultPackageNameOverrides();
        internal readonly bool DisallowUnsafeTypes;
        internal readonly ITypeFilter TypeFilter;
        
        [Obsolete(message:"This constructor is deprecated and will be removed in the future, please use the one with the packageNameOverrides argument")]
        public SerializerOptions(
            bool versionTolerance = false, 
            bool preserveObjectReferences = false, 
            IEnumerable<Surrogate> surrogates = null, 
            IEnumerable<ValueSerializerFactory> serializerFactories = null, 
            IEnumerable<Type> knownTypes = null, 
            bool ignoreISerializable = false)
            : this(versionTolerance, preserveObjectReferences, surrogates, serializerFactories, knownTypes, ignoreISerializable, null)
        { }

        [Obsolete(message:"This constructor is deprecated and will be removed in the future, please use the one with the disallowUnsafeTypes argument")]
        public SerializerOptions(
            bool versionTolerance, 
            bool preserveObjectReferences, 
            IEnumerable<Surrogate> surrogates, 
            IEnumerable<ValueSerializerFactory> serializerFactories, 
            IEnumerable<Type> knownTypes, 
            bool ignoreISerializable, 
            IEnumerable<Func<string, string>> packageNameOverrides)
            : this(versionTolerance, preserveObjectReferences, surrogates, serializerFactories, knownTypes, ignoreISerializable, packageNameOverrides, true)
        { }
        
        [Obsolete(message:"This constructor is deprecated and will be removed in the future, please use the one with the typeFilter argument")]
        public SerializerOptions(
            bool versionTolerance, 
            bool preserveObjectReferences, 
            IEnumerable<Surrogate> surrogates, 
            IEnumerable<ValueSerializerFactory> serializerFactories, 
            IEnumerable<Type> knownTypes, 
            bool ignoreISerializable, 
            IEnumerable<Func<string, string>> packageNameOverrides,
            bool disallowUnsafeTypes)
            : this(versionTolerance, preserveObjectReferences, surrogates, serializerFactories, knownTypes, ignoreISerializable, packageNameOverrides, disallowUnsafeTypes, DisabledTypeFilter.Instance)
        { }
        
        public SerializerOptions(
            bool versionTolerance, 
            bool preserveObjectReferences, 
            IEnumerable<Surrogate> surrogates, 
            IEnumerable<ValueSerializerFactory> serializerFactories, 
            IEnumerable<Type> knownTypes, 
            bool ignoreISerializable, 
            IEnumerable<Func<string, string>> packageNameOverrides,
            bool disallowUnsafeTypes,
            ITypeFilter typeFilter)
        {
            VersionTolerance = versionTolerance;
            Surrogates = surrogates?.ToArray() ?? EmptySurrogates;

            //use the default factories + any user defined
	        ValueSerializerFactories = 
                serializerFactories?.Concat(DefaultValueSerializerFactories).ToArray() ??
		        DefaultValueSerializerFactories;

            KnownTypes = knownTypes?.ToArray() ?? new Type[] {};
            for (var i = 0; i < KnownTypes.Length; i++)
            {
                KnownTypesDict.Add(KnownTypes[i],(ushort)i);
            }

            PreserveObjectReferences = preserveObjectReferences;
            IgnoreISerializable = ignoreISerializable;

            if(packageNameOverrides != null)
                CrossFrameworkPackageNameOverrides.AddRange(packageNameOverrides);

            DisallowUnsafeTypes = disallowUnsafeTypes;
            TypeFilter = typeFilter ?? DisabledTypeFilter.Instance;
        }

        public SerializerOptions WithVersionTolerance(bool versionTolerance)
            => Copy(versionTolerance: versionTolerance);
        public SerializerOptions WithPreserveObjectReferences(bool preserveObjectReferences)
            => Copy(preserveObjectReferences: preserveObjectReferences);
        public SerializerOptions WithSurrogates(IEnumerable<Surrogate> surrogates)
            => Copy(surrogates: surrogates);
        public SerializerOptions WithSerializerFactory(IEnumerable<ValueSerializerFactory> serializerFactories)
            => Copy(serializerFactories: serializerFactories);
        public SerializerOptions WithKnownTypes(IEnumerable<Type> knownTypes)
            => Copy(knownTypes: knownTypes);
        public SerializerOptions WithIgnoreSerializable(bool ignoreISerializable)
            => Copy(ignoreISerializable: ignoreISerializable);
        public SerializerOptions WithPackageNameOverrides(IEnumerable<Func<string, string>> packageNameOverrides)
            => Copy(packageNameOverrides: packageNameOverrides);
        public SerializerOptions WithDisallowUnsafeType(bool disallowUnsafeType)
            => Copy(disallowUnsafeType: disallowUnsafeType);
        public SerializerOptions WithTypeFilter(ITypeFilter typeFilter)
            => Copy(typeFilter: typeFilter);

        private SerializerOptions Copy(
            bool? versionTolerance = null,
            bool? preserveObjectReferences = null,
            IEnumerable<Surrogate> surrogates = null,
            IEnumerable<ValueSerializerFactory> serializerFactories = null,
            IEnumerable<Type> knownTypes = null,
            bool? ignoreISerializable = null,
            IEnumerable<Func<string, string>> packageNameOverrides = null,
            bool? disallowUnsafeType = null,
            ITypeFilter typeFilter = null)
            => new SerializerOptions(
                versionTolerance ?? VersionTolerance,
                preserveObjectReferences ?? PreserveObjectReferences,
                surrogates ?? Surrogates,
                serializerFactories ?? ValueSerializerFactories,
                knownTypes ?? KnownTypes,
                ignoreISerializable ?? IgnoreISerializable,
                packageNameOverrides ?? CrossFrameworkPackageNameOverrides,
                disallowUnsafeType ?? DisallowUnsafeTypes,
                typeFilter ?? TypeFilter
            );
    }
}