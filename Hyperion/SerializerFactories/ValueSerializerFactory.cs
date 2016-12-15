#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ValueSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public abstract class ValueSerializerFactory
    {
        public abstract bool CanSerialize(Serializer serializer, Type type);
        public abstract bool CanDeserialize(Serializer serializer, Type type);

        public abstract ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping);
    }
}