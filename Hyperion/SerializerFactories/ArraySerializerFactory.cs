#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ArraySerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public class ArraySerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type) => type.IsOneDimensionalArray();

        public override bool CanDeserialize(Serializer serializer, Type type) => CanSerialize(serializer, type);

        private static void WriteValues<T>(T[] array, Stream stream, Type elementType, ValueSerializer elementSerializer, SerializerSession session)
        {
            Int32Serializer.WriteValueImpl(stream, array.Length, session);
            var preserveObjectReferences = session.Serializer.Options.PreserveObjectReferences;
            foreach (var value in array)
            {
                stream.WriteObject(value, elementType, elementSerializer, preserveObjectReferences, session);
            }
        }
        private static void ReadValues<T>(int length, Stream stream, DeserializerSession session, T[] array)
        {
            for (var i = 0; i < length; i++)
            {
                var value = (T)stream.ReadObject(session);
                array[i] = value;
            }
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var arraySerializer = new ObjectSerializer(type);

            var elementType = type.GetElementType();
            var elementSerializer = serializer.GetSerializerByType(elementType);
            var preserveObjectReferences = serializer.Options.PreserveObjectReferences;
            //TODO: code gen this part
            ObjectReader reader = (stream, session) =>
            {
                var length = stream.ReadInt32(session);
                var array = Array.CreateInstance(elementType, length); //create the array
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
            arraySerializer.Initialize(reader, writer);
            typeMapping.TryAdd(type, arraySerializer);
            return arraySerializer;
        }
    }
}