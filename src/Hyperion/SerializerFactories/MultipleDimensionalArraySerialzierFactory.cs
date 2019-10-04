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
using System.IO;
using System.Linq;
using System.Reflection;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    ///  we don't support 4 dimensional array now
    internal sealed class MultipleDimensionalArraySerialzierFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type) =>
             /// wo don't support 4 dimensional array now
             type.IsArray && type.GetArrayRank() > 1 && type.GetArrayRank() < 4;

        public override bool CanDeserialize(Serializer serializer, Type type) => CanSerialize(serializer, type);

        private static void WriteValues(Array array, Stream stream, Type elementType, ValueSerializer elementSerializer, SerializerSession session)
        {
            for (var i = 0; i < array.Rank; i ++ )
            {
                Int32Serializer.WriteValueImpl(stream, array.GetLength(i), session);
            }
            var preserveObjectReferences = session.Serializer.Options.PreserveObjectReferences;
            foreach (var value in array)
            {
                stream.WriteObject(value, elementType, elementSerializer, preserveObjectReferences, session);
            }
        }


        private static Array ReadValues2D(Stream stream, DeserializerSession session, Array array)
        {
            for (var i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
            {
                for (var j = array.GetLowerBound(1); j <= array.GetUpperBound(1); j++)
                {
                    var value = stream.ReadObject(session);
                    array.SetValue(value, i, j);
                }
            }
            return array;
        }

        private static Array ReadValues3D(Stream stream, DeserializerSession session, Array array)
        {
            for (var i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
            {
                for (var j = array.GetLowerBound(1); j <= array.GetUpperBound(1); j++)
                {
                    for (var m = array.GetLowerBound(2); m <= array.GetUpperBound(2); m++)
                    {
                        var value = stream.ReadObject(session);
                        array.SetValue(value, i, j, m);
                    }

                }
            }
            return array;
        }


        private static ObjectReader CreateReader(bool preserveObjectReferences,int arrayRank, Type elementType)
        {
            if (arrayRank == 2)
            {
                ObjectReader reader = (stream, session) =>
                    {
                        var length1 = stream.ReadInt32(session);
                        var length2 = stream.ReadInt32(session);
                        var array = Array.CreateInstance(elementType, length1, length2);
                        if (preserveObjectReferences)
                        {
                            session.TrackDeserializedObject(array);
                        }
                        return ReadValues2D(stream, session, array);
                    };
                return reader;
            }
            if (arrayRank == 3)
            {
                ObjectReader reader = (stream, session) =>
                {
                    var length1 = stream.ReadInt32(session);
                    var length2 = stream.ReadInt32(session);
                    var length3 = stream.ReadInt32(session);
                    var array = Array.CreateInstance(elementType, length1, length2, length3);
                    if (preserveObjectReferences)
                    {
                        session.TrackDeserializedObject(array);
                    }
                    return ReadValues3D(stream, session, array);
                };
                return reader;
            }

            else
            {
                throw new UnsupportedTypeException(elementType, "we don't support 4 dimensional array now");
            }
        }


        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var arraySerializer = new ObjectSerializer(type);

            var elementType = 
                type.GetTypeInfo()
                .GetMethods()
                .Where(methodInfo => methodInfo.Name == "Get")
                .Select(methodInfo => methodInfo.ReturnType)
                .FirstOrDefault();

            var elementSerializer = serializer.GetSerializerByType(elementType);
            var preserveObjectReferences = serializer.Options.PreserveObjectReferences;

            var arrayRank = type.GetArrayRank();

            //TODO: code gen this part
            ObjectReader reader = CreateReader(preserveObjectReferences, arrayRank, elementType);

            ObjectWriter writer = (stream, arr, session) =>
            {
                if (preserveObjectReferences)
                {
                    session.TrackSerializedObject(arr);
                }

                WriteValues((Array)arr, stream, elementType, elementSerializer, session);
            };
            arraySerializer.Initialize(reader, writer);
            typeMapping.TryAdd(type, arraySerializer);
            return arraySerializer;
        }


    }
}