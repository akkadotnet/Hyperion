#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DefaultDictionarySerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public class DefaultDictionarySerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type) => IsInterface(type);

        private static bool IsInterface(Type type)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof (Dictionary<,>);
        }

        public override bool CanDeserialize(Serializer serializer, Type type) => IsInterface(type);

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var ser = new ObjectSerializer(type);
            typeMapping.TryAdd(type, ser);
            var elementSerializer = serializer.GetSerializerByType(typeof (DictionaryEntry));
            var preserveObjectReferences = serializer.Options.PreserveObjectReferences;
            ObjectReader reader = (stream, session) =>
            {
                var count = stream.ReadInt32(session);
                var instance = (IDictionary) Activator.CreateInstance(type, count);
                if (preserveObjectReferences)
                {
                    session.TrackDeserializedObject(instance);
                }

                for (var i = 0; i < count; i++)
                {
                    var entry = (DictionaryEntry) stream.ReadObject(session);
                    instance.Add(entry.Key, entry.Value);
                }
                return instance;
            };

            ObjectWriter writer = (stream, obj, session) =>
            {

                if (preserveObjectReferences)
                {
                    session.TrackSerializedObject(obj);
                }
                var dict = obj as IDictionary;
                // ReSharper disable once PossibleNullReferenceException
                Int32Serializer.WriteValueImpl(stream,dict.Count,session);
                foreach (DictionaryEntry item in dict)
                {
                    stream.WriteObject(item, typeof (DictionaryEntry), elementSerializer,
                        serializer.Options.PreserveObjectReferences, session);
                    // elementSerializer.WriteValue(stream,item,session);
                }
            };
            ser.Initialize(reader, writer);
            
            return ser;
        }
    }
}