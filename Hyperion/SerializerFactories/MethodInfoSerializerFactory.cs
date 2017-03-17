﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="MethodInfoSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public class MethodInfoSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type)
        {
            return type.GetTypeInfo().IsSubclassOf(typeof(MethodInfo));
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
                var arguments = stream.ReadObject(session) as Type[];

#if NET45
                var method = owner.GetTypeInfo().GetMethod(
                    name,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, 
                    null,
                    CallingConventions.Any,
                    arguments,
                    null);
                return method;
#else
                var methods = owner.GetTypeInfo()
                    .GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public |
                                BindingFlags.NonPublic);
                var method = methods.FirstOrDefault(m => m.Name == name &&
                                                         m.GetParameters()
                                                             .Select(p => p.ParameterType)
                                                             .SequenceEqual(arguments));
                return method;
#endif
            };
            ObjectWriter writer = (stream, obj, session) =>
            {
                var method = (MethodInfo) obj;
                var name = method.Name;
                var owner = method.DeclaringType;
                var arguments = method.GetParameters().Select(p => p.ParameterType).ToArray();
                StringSerializer.WriteValueImpl(stream,name,session);
                stream.WriteObjectWithManifest(owner, session);
                stream.WriteObjectWithManifest(arguments, session);
            };
            os.Initialize(reader, writer);

            return os;
        }
    }
}