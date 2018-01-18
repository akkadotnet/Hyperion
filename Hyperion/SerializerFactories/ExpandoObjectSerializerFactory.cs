﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ExpandoObjectSerializerFactory.cs" company="Akka.NET Team">
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
    internal sealed class ExpandoObjectSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type) => type == typeof(System.Dynamic.ExpandoObject);

        

        public override bool CanDeserialize(Serializer serializer, Type type) => CanSerialize(serializer,type);

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var preserveObjectReferences = serializer.Options.PreserveObjectReferences;
            var ser = new ObjectSerializer(type);
            typeMapping.TryAdd(type, ser);
            var elementSerializer = serializer.GetSerializerByType(typeof(DictionaryEntry));

            ObjectReader reader = (stream, session) =>
            {
               
                var instance = Activator.CreateInstance(type) as IDictionary<string,object>;

                if (preserveObjectReferences)
                {
                    session.TrackDeserializedObject(instance);
                }
                var count = stream.ReadInt32(session);
                for (var i = 0; i < count; i++)
                {
                    var entry = (KeyValuePair<string,object>)stream.ReadObject(session);
                    instance.Add(entry);
                }
                return instance;
            };

            ObjectWriter writer = (stream, obj, session) =>
            {
                if (preserveObjectReferences)
                {
                    session.TrackSerializedObject(obj);
                }
                var dict = obj as IDictionary<string,object>;
                // ReSharper disable once PossibleNullReferenceException
                Int32Serializer.WriteValueImpl(stream, dict.Count, session);
                foreach (var item in dict)
                {
                    stream.WriteObject(item, typeof(DictionaryEntry), elementSerializer,
                        serializer.Options.PreserveObjectReferences, session);
                    // elementSerializer.WriteValue(stream,item,session);
                }
            };
            ser.Initialize(reader, writer);

            return ser;
        }
    }
}