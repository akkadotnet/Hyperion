#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DictionarySerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    internal sealed class DictionarySerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type) => IsInterface(type);

        private static bool IsInterface(Type type)
        {
            return type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(t => t.GetTypeInfo().IsGenericType && t.GetTypeInfo().GetGenericTypeDefinition() == typeof (IDictionary<,>))
                .Any(isDict => isDict);
        }

        public override bool CanDeserialize(Serializer serializer, Type type) => IsInterface(type);

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var preserveObjectReferences = serializer.Options.PreserveObjectReferences;
            var ser = new ObjectSerializer(type);
            typeMapping.TryAdd(type, ser);
            var dictionaryTypes = GetKeyValuePairType(type);
            var elementSerializer = serializer.GetSerializerByType(dictionaryTypes.KeyValuePairType);

            ObjectReader reader = (stream, session) =>
            {
                object instance;
                try
                {
                    instance = Activator.CreateInstance(type, true); // IDictionary<TKey, TValue>
                } catch(Exception) {
                    instance = Activator.CreateInstance(type); // IDictionary<TKey, TValue>
                }

                if (preserveObjectReferences)
                {
                    session.TrackDeserializedObject(instance);
                }
                var count = stream.ReadInt32(session);
                for (var i = 0; i < count; i++)
                {
                    var entry = stream.ReadObject(session); // KeyValuePair<TKey, TValue>
                    
                    // Get entry.Key and entry.Value
                    var key = dictionaryTypes.KeyValuePairType.GetProperty(nameof(KeyValuePair<object, object>.Key)).GetValue(entry, null);
                    var value = dictionaryTypes.KeyValuePairType.GetProperty(nameof(KeyValuePair<object, object>.Value)).GetValue(entry, null);
                    
                    // Same as: instance.Add(key, value)
                    dictionaryTypes.DictionaryInterfaceType
                        .GetMethod(nameof(IDictionary<object, object>.Add), new []{ dictionaryTypes.KeyType, dictionaryTypes.ValueType })
                        .Invoke(instance, new [] { key, value });
                }
                
                return instance;
            };

            ObjectWriter writer = (stream, obj, session) =>
            {
                if (preserveObjectReferences)
                {
                    session.TrackSerializedObject(obj);
                }
                
                var dict = obj as IEnumerable; // IDictionary<T, V> is IEnumerable<KeyValuePair<T, V>>
                var count = dict.Cast<object>().Count();
                // ReSharper disable once PossibleNullReferenceException
                Int32Serializer.WriteValueImpl(stream, count, session);
                foreach (var item in dict)
                {
                    stream.WriteObject(item, dictionaryTypes.KeyValuePairType, elementSerializer, serializer.Options.PreserveObjectReferences, session);
                }
            };
            ser.Initialize(reader, writer);
            
            return ser;
        }

        private GenericDictionaryTypes GetKeyValuePairType(Type dictImplementationType)
        {
            var dictInterface = dictImplementationType.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof (IDictionary<,>));
            var keyType = dictInterface.GetGenericArguments()[0];
            var valueType = dictInterface.GetGenericArguments()[1];
            return new GenericDictionaryTypes()
            {
                KeyType = keyType,
                ValueType = valueType,
                KeyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType),
                DictionaryInterfaceType = typeof(IDictionary<,>).MakeGenericType(keyType, valueType)
            };
        }

        class GenericDictionaryTypes
        {
            public Type KeyType { get; set; }
            public Type ValueType { get; set; }
            public Type KeyValuePairType { get; set; }
            public Type DictionaryInterfaceType { get; set; }
        }
    }
}