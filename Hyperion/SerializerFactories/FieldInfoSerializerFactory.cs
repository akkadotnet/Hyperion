#region copyright
// -----------------------------------------------------------------------
//  <copyright file="FieldInfoSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Reflection;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public class FieldInfoSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type)
        {
            return type.GetTypeInfo().IsSubclassOf(typeof(FieldInfo));
        }

        public override bool CanDeserialize(Serializer serializer, Type type)
        {
            return CanSerialize(serializer, type);
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var os = new ObjectSerializer(type);
            typeMapping.TryAdd(type, os);
            ObjectReader reader = (stream, session) =>
            {
                var name = stream.ReadString(session);
                var owner = stream.ReadObject(session) as Type;

#if NET45
                var field = owner.GetTypeInfo().GetField(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return field;
#else
                return null;
#endif
            };
            ObjectWriter writer = (stream, obj, session) =>
            {
                var field = (FieldInfo)obj;
                var name = field.Name;
                var owner = field.DeclaringType;
                StringSerializer.WriteValueImpl(stream, name, session);
                stream.WriteObjectWithManifest(owner, session);
            };
            os.Initialize(reader, writer);

            return os;
        }
    }
}