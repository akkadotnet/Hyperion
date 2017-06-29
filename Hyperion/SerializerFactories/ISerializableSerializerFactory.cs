﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ISerializableSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion
#if SERIALIZATION
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    // ReSharper disable once InconsistentNaming
    public class ISerializableSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type)
        {
            if (serializer.Options.IgnoreISerializable)
                return false;

            return typeof (ISerializable).IsAssignableFrom(type);
        }

        public override bool CanDeserialize(Serializer serializer, Type type)
        {
            if (serializer.Options.IgnoreISerializable)
                return false;

            return CanSerialize(serializer, type);
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var serializableSerializer = new ObjectSerializer(type);
            typeMapping.TryAdd(type, serializableSerializer);
            ObjectReader reader = (stream, session) =>
            {
                var dict = stream.ReadObject(session) as Dictionary<string, object>;
                var info = new SerializationInfo(type, new FormatterConverter());
                // ReSharper disable once PossibleNullReferenceException
                foreach (var item in dict)
                {
                    info.AddValue(item.Key, item.Value);
                }

                var ctor = type.GetConstructor(BindingFlagsEx.All, null,
                    new[] {typeof (SerializationInfo), typeof (StreamingContext)}, null);
                var instance = ctor.Invoke(new object[] {info, new StreamingContext()});
                var deserializationCallback = instance as IDeserializationCallback;
                deserializationCallback?.OnDeserialization(this);
                return instance;
            };

            ObjectWriter writer = (stream, o, session) =>
            {
                var info = new SerializationInfo(type, new FormatterConverter());
                var serializable = o as ISerializable;
                // ReSharper disable once PossibleNullReferenceException
                serializable.GetObjectData(info, new StreamingContext());
                var dict = new Dictionary<string, object>();
                foreach (var item in info)
                {
                    dict.Add(item.Name, item.Value);
                }
                stream.WriteObjectWithManifest(dict, session);
            };
            serializableSerializer.Initialize(reader, writer);

            return serializableSerializer;
        }
    }
}
#endif