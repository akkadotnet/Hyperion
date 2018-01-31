﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="FromSurrogateSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    internal sealed class FromSurrogateSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type) => false;

        public override bool CanDeserialize(Serializer serializer, Type type)
        {
            var surrogate = serializer.Options.Surrogates.FirstOrDefault(s => s.To.GetTypeInfo().IsAssignableFrom(type));
            return surrogate != null;
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var surrogate = serializer.Options.Surrogates.FirstOrDefault(s => s.To.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()));
            var objectSerializer = new ObjectSerializer(type);
            // ReSharper disable once PossibleNullReferenceException
            var fromSurrogateSerializer = new FromSurrogateSerializer(surrogate.FromSurrogate, objectSerializer);
            typeMapping.TryAdd(type, fromSurrogateSerializer);


            serializer.CodeGenerator.BuildSerializer(serializer, objectSerializer);
            return fromSurrogateSerializer;
        }
    }
}