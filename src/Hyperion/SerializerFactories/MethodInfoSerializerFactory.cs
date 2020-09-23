#region copyright
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
    internal sealed class MethodInfoSerializerFactory : ValueSerializerFactory
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
            if (serializer.Options.KnownTypesDict.TryGetValue(type, out var index))
            {
                var wrapper = new KnownTypeObjectSerializer(os, index);
                typeMapping.TryAdd(type, wrapper);
            }
            else
                typeMapping.TryAdd(type, os);
            ObjectReader reader = (stream, session) =>
            {
                var name = stream.ReadString(session);
                var owner = stream.ReadObject(session) as Type;
                var parameterTypes = stream.ReadObject(session) as Type[];
                var method = owner.GetMethodExt(name,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    parameterTypes);
                if (method.IsGenericMethodDefinition) {
                    var genericTypeArguments = stream.ReadObject(session) as Type[];
                    method = method.MakeGenericMethod(genericTypeArguments);
                }

                return method;
            };
            ObjectWriter writer = (stream, obj, session) =>
            {
                var method = (MethodInfo) obj;
                var name = method.Name;
                var owner = method.DeclaringType;
                StringSerializer.WriteValueImpl(stream, name, session);
                stream.WriteObjectWithManifest(owner, session);
                var arguments = method.GetParameters().Select(p => p.ParameterType).ToArray();
                stream.WriteObjectWithManifest(arguments, session);
                if (method.IsGenericMethod) {
                    // we use the parameter types to find the method above but, if generic, we need to store the generic type arguments as well
                    // in order to MakeGenericType
                    var genericTypeArguments = method.GetGenericArguments();
                    stream.WriteObjectWithManifest(genericTypeArguments, session);
                }
            };
            os.Initialize(reader, writer);

            return os;
        }
    }
}