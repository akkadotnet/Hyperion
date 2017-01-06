#region copyright
// -----------------------------------------------------------------------
//  <copyright file="EnumerableSerializerFactory.cs" company="Akka.NET Team">
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
    public class EnumerableSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type)
        {
            //TODO: check for constructor with IEnumerable<T> param

            var countProperty = type.GetTypeInfo().GetProperty("Count");
            if (countProperty == null || countProperty.PropertyType != typeof(int))
                return false;

            if (!type.GetTypeInfo().GetMethods().Any(IsAddMethod))
                return false;

            var isGenericEnumerable = GetEnumerableType(type) != null;
            if (isGenericEnumerable)
                return true;

            if (typeof(ICollection).GetTypeInfo().IsAssignableFrom(type))
                return true;

            return false;
        }

        private static bool IsAddMethod(MethodInfo methodInfo) => 
            (methodInfo.Name == "AddRange" || methodInfo.Name == "Add")
            && (methodInfo.ReturnType == typeof(void) || methodInfo.ReturnType == typeof(bool)) // sets return bool on Add
            && !methodInfo.IsStatic
            && HasValidParameters(methodInfo);

        private static bool HasValidParameters(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            return parameters.Length == 1;
        }

        public override bool CanDeserialize(Serializer serializer, Type type)
        {
            return CanSerialize(serializer, type);
        }

        private static Type GetEnumerableType(Type type)
        {
            return type
                .GetTypeInfo()
                .GetInterfaces()
                .Where(intType => intType.GetTypeInfo().IsGenericType && intType.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(intType => intType.GetTypeInfo().GetGenericArguments()[0])
                .FirstOrDefault();
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var x = new ObjectSerializer(type);
            typeMapping.TryAdd(type, x);
            var preserveObjectReferences = serializer.Options.PreserveObjectReferences;

            var elementType = GetEnumerableType(type) ?? typeof(object);
            var elementSerializer = serializer.GetSerializerByType(elementType);

            var countProperty = type.GetTypeInfo().GetProperty("Count");
            var addRange = type.GetTypeInfo().GetMethod("AddRange");
            var add = type.GetTypeInfo().GetMethod("Add");

            Func<object, int> countGetter = o => (int)countProperty.GetValue(o);


            ObjectReader reader = (stream, session) =>
            {
                var instance = Activator.CreateInstance(type);
                if (preserveObjectReferences)
                {
                    session.TrackDeserializedObject(instance);
                }

                var count = stream.ReadInt32(session);

                if (addRange != null)
                {
                    var items = Array.CreateInstance(elementType, count);
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        items.SetValue(value, i);
                    }
                    //HACK: this needs to be fixed, codegenerated or whatever

                    addRange.Invoke(instance, new object[] { items });
                    return instance;
                }
                if (add != null)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        add.Invoke(instance, new[] { value });
                    }
                }


                return instance;
            };

            ObjectWriter writer = (stream, o, session) =>
            {
                if (preserveObjectReferences)
                {
                    session.TrackSerializedObject(o);
                }
                Int32Serializer.WriteValueImpl(stream, countGetter(o), session);
                var enumerable = o as IEnumerable;
                // ReSharper disable once PossibleNullReferenceException
                foreach (var value in enumerable)
                {
                    stream.WriteObject(value, elementType, elementSerializer, preserveObjectReferences, session);
                }


            };
            x.Initialize(reader, writer);
            return x;
        }
    }
}