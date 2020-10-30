#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ImmutableCollectionsSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    internal sealed class ImmutableCollectionsSerializerFactory : ValueSerializerFactory
    {
        private const string ImmutableCollectionsNamespace = "System.Collections.Immutable";
        private const string ImmutableCollectionsAssembly = "System.Collections.Immutable";

        public override bool CanSerialize(Serializer serializer, Type type)
        {
            if (type.Namespace == null || !type.Namespace.Equals(ImmutableCollectionsNamespace)) return false;
            var isGenericEnumerable = GetEnumerableType(type) != null;
            if (isGenericEnumerable)
                return true;

            return false;
        }

        public override bool CanDeserialize(Serializer serializer, Type type) => CanSerialize(serializer, type);

        private static Type GetEnumerableType(Type type)
        {
            return type
                .GetTypeInfo()
                .GetInterfaces()
                .Where(intType => intType.GetTypeInfo().IsGenericType && intType.GetTypeInfo().GetGenericTypeDefinition() == typeof (IEnumerable<>))
                .Select(intType => intType.GetTypeInfo().GetGenericArguments()[0])
                .FirstOrDefault();
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var x = new ObjectSerializer(type);
            if (serializer.Options.KnownTypesDict.TryGetValue(type, out var index))
            {
                var wrapper = new KnownTypeObjectSerializer(x, index);
                typeMapping.TryAdd(type, wrapper);
            }
            else
                typeMapping.TryAdd(type, x);
            var preserveObjectReferences = serializer.Options.PreserveObjectReferences;

            var elementType = GetEnumerableType(type) ?? typeof (object);
            var elementSerializer = serializer.GetSerializerByType(elementType);

            var typeName = type.Name;
            var genericSufixIdx = typeName.IndexOf('`');
            typeName = genericSufixIdx != -1 ? typeName.Substring(0, genericSufixIdx) : typeName;
            var creatorType =
                Type.GetType(ImmutableCollectionsNamespace + "." + typeName + ", " + ImmutableCollectionsAssembly);


            var createRangeMethodInfo = creatorType != null
                ? creatorType.GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(methodInfo => methodInfo.Name == "CreateRange" && methodInfo.GetParameters().Length == 1)
                : null;

            // If the element type is a generic type and the method located to create the collection instance requires more than one generic type parameter
            // we need to obtain the generic arguments of the element type.
            var genericTypes = elementType.GetTypeInfo().IsGenericType && createRangeMethodInfo != null && createRangeMethodInfo.GetGenericArguments().Length > 1
                ? elementType.GetTypeInfo().GetGenericArguments()
                : new[] { elementType };

            // if creatorType == null it means that type is probably an interface
            // we propagate null to create mock serializer - it won't be used anyway

            var stackTypeDef = Type.GetType(ImmutableCollectionsNamespace + ".IImmutableStack`1, " + ImmutableCollectionsAssembly, true);
            var stackInterface = stackTypeDef.MakeGenericType(genericTypes[0]);

            var isStack = stackInterface.IsAssignableFrom(type);

            var createRange = createRangeMethodInfo != null
                ? createRangeMethodInfo.MakeGenericMethod(genericTypes)
                : null;

            ObjectWriter writer = (stream, o, session) =>
            {
                var enumerable = o as ICollection;
                if (enumerable == null)
                {
                    // object can be IEnumerable but not ICollection i.e. ImmutableQueue
                    var e = (IEnumerable) o;
                    var list = e.Cast<object>().ToList();//

                    enumerable = list;
                }
                Int32Serializer.WriteValueImpl(stream,enumerable.Count,session);
                foreach (var value in enumerable)
                {
                    stream.WriteObject(value, elementType, elementSerializer, preserveObjectReferences, session);
                }
                if (preserveObjectReferences)
                {
                    session.TrackSerializedObject(o);
                }
            };
            ObjectReader reader;

            if (isStack)
            {
                // if we are dealing with stack, we need to apply arguments in reverse order
                reader = (stream, session) =>
                {
                    var count = stream.ReadInt32(session);
                    var items = Array.CreateInstance(elementType, count);
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        items.SetValue(value, count - i - 1);
                    }

                    var instance = createRange.Invoke(null, new object[] { items });
                    if (preserveObjectReferences)
                    {
                        session.TrackDeserializedObject(instance);
                    }
                    return instance;
                };
            }
            else
            {
                reader = (stream, session) =>
                {
                    var count = stream.ReadInt32(session);
                    var items = Array.CreateInstance(elementType, count);
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        items.SetValue(value, i);
                    }

                    var instance = createRange.Invoke(null, new object[] { items });
                    if (preserveObjectReferences)
                    {
                        session.TrackDeserializedObject(instance);
                    }
                    return instance;
                };
            }
            x.Initialize(reader, writer);
            return x;
        }
    }
}