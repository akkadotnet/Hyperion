#region copyright
// -----------------------------------------------------------------------
//  <copyright file="LinkedListSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public sealed class LinkedListSerializerFactory : ValueSerializerFactory
    {
        private static readonly Type LinkedListType = typeof(LinkedList<>);

        public override bool CanSerialize(Serializer serializer, Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == LinkedListType;

        public override bool CanDeserialize(Serializer serializer, Type type) => CanSerialize(serializer, type);

        private static void WriteValues<T>(LinkedList<T> list, Stream stream, Type elementType, ValueSerializer elementSerializer, SerializerSession session)
        {
            Int32Serializer.WriteValueImpl(stream, list.Count, session);
            var preserveObjectReferences = session.Serializer.Options.PreserveObjectReferences;
            foreach (var value in list)
            {
                stream.WriteObject(value, elementType, elementSerializer, preserveObjectReferences, session);
            }
        }

        private static void ReadValues<T>(int length, Stream stream, DeserializerSession session, LinkedList<T> list)
        {
            for (var i = 0; i < length; i++)
            {
                var value = (T)stream.ReadObject(session);
                list.AddLast(value);
            }
        }

        private static Type GetLinkedListType(Type type)
        {
            return type.GetTypeInfo().GetGenericArguments()[0];
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var elementType = GetLinkedListType(type);
            var elementSerializer = serializer.GetSerializerByType(elementType);
            var preserveObjectReferences = serializer.Options.PreserveObjectReferences;
            //TODO: code gen this part
            ObjectReader reader = (stream, session) =>
            {
                var length = stream.ReadInt32(session);
                var array = Activator.CreateInstance(type);
                if (preserveObjectReferences)
                {
                    session.TrackDeserializedObject(array);
                }

                ReadValues(length, stream, session, (dynamic)array);

                return array;
            };

            ObjectWriter writer = (stream, arr, session) =>
            {
                if (preserveObjectReferences)
                {
                    session.TrackSerializedObject(arr);
                }

                WriteValues((dynamic)arr, stream, elementType, elementSerializer, session);

            };

            var ser = new ObjectSerializer(type);
            ser.Initialize(reader, writer);
            typeMapping.TryAdd(type, ser);

            return ser;
        }
    }
}