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
using System.Linq.Expressions;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public class EnumerableSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type)
        {
            // Stack<T> has IEnumerable<T> constructor, but reverses order of the stack, so can't be used.
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>))
                return false;

            var countProperty = type.GetTypeInfo().GetProperty("Count");
            if (countProperty == null || countProperty.PropertyType != typeof(int))
                return false;

            var hasEnumerableConstructor = GetEnumerableConstructor(type) != null;
            if (hasEnumerableConstructor)
                return true;
            
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

        private static ConstructorInfo GetEnumerableConstructor(Type type)
        {
            var enumerableType = GetEnumerableType(type);
            return enumerableType != null
              ? type.GetTypeInfo().GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(enumerableType) })
              : null;
        }

        private static Func<object, object> CompileCtorToDelegate(ConstructorInfo ctor, Type argType)
        {
            var arg = Expression.Parameter(typeof(object));
            var castArg = Expression.Convert(arg, argType);
            var call = Expression.New(ctor, new Expression[] { castArg });
            var castRes = Expression.Convert(call, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(castRes, arg);
            var compiled = lambda.Compile();
            return compiled;
        }

        private static Action<object, object> CompileMethodToDelegate(MethodInfo method, Type instanceType, Type argType)
        {
            var instance = Expression.Parameter(typeof(object));
            var arg = Expression.Parameter(typeof(object));
            var castInstance = Expression.Convert(instance, instanceType);
            var castArg = Expression.Convert(arg, argType);
            var call = Expression.Call(castInstance, method, new Expression[] { castArg });
            var lambda = Expression.Lambda<Action<object, object>>(call, instance, arg);
            var compiled = lambda.Compile();
            return compiled;
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
            var constructor = GetEnumerableConstructor(type);
            var construct = constructor != null 
                ? CompileCtorToDelegate(constructor, elementType.MakeArrayType()) 
                : null;
            var addRangeMethod = type.GetTypeInfo().GetMethod("AddRange");
            var addRange = construct == null && addRangeMethod != null
                 ? CompileMethodToDelegate(addRangeMethod, type, elementType.MakeArrayType())
                 : null;
            var addMethod = type.GetTypeInfo().GetMethod("Add");
            var add = construct == null && addRange == null && addMethod != null
                ? CompileMethodToDelegate(addMethod, type, elementType)
                : null;

            Func<object, int> countGetter = o => (int)countProperty.GetValue(o);
            
            ObjectReader reader = (stream, session) =>
            {
                var count = stream.ReadInt32(session);
                if (construct != null)
                {
                    var enumerable = Array.CreateInstance(elementType, count);
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        enumerable.SetValue(value, i);
                    }
                    var instanceFromConstructor = construct(enumerable);
                    if (preserveObjectReferences)
                    {
                        session.TrackDeserializedObject(instanceFromConstructor);
                    }
                    return instanceFromConstructor;
                }

                var instance = Activator.CreateInstance(type);
                if (addRange != null)
                {
                    var items = Array.CreateInstance(elementType, count);
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        items.SetValue(value, i);
                    }

                    addRange(instance, items);
                    return instance;
                }
                if (add != null)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        add(instance, value);
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