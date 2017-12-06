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

            if (!HasParameterlessConstructor(type))
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

        private static readonly BindingFlags allInstanceBindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

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

        private static bool HasParameterlessConstructor(Type type)
        {
            return type.GetTypeInfo()
                .GetConstructors(allInstanceBindings)
                .Any(ctor => !ctor.GetParameters().Any());
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
            BindingFlags bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var enumerableType = GetEnumerableType(type);
            var iEnumerableType = typeof(IEnumerable<>).MakeGenericType(enumerableType);
            return enumerableType != null
                ? type.GetTypeInfo()
                    .GetConstructors(bindings)
                    .Where(ctor => HasSingleParameterOfType(ctor, iEnumerableType))
                    .FirstOrDefault()
                : null;
        }

        private static bool HasSingleParameterOfType(MethodBase methodInfo, Type type)
        {
            var parameters = methodInfo.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == type;
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

            var enumerableConstructor = GetEnumerableConstructor(type);
            var addRangeMethod = type.GetTypeInfo().GetMethod("AddRange");
            var addMethod = type.GetTypeInfo().GetMethod("Add");

            Func<object, int> countGetter = o => (int)countProperty.GetValue(o);
            ObjectReader reader = null;
            if (enumerableConstructor != null)
            {
                var construct = CompileCtorToDelegate(enumerableConstructor, elementType.MakeArrayType());
                reader = (stream, session) =>
                {
                    var count = stream.ReadInt32(session);
                    var items = Array.CreateInstance(elementType, count);
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        items.SetValue(value, i);
                    }
                    var instance = construct(items);
                    if (preserveObjectReferences)
                    {
                        session.TrackDeserializedObject(instance);
                    }
                    return instance;
                };
            }
            else if (addRangeMethod != null)
            {
                var addRange = CompileMethodToDelegate(addRangeMethod, type, elementType.MakeArrayType());
                reader = (stream, session) =>
                {
                    var instance = Activator.CreateInstance(type, true);
                    var count = stream.ReadInt32(session);
                    var items = Array.CreateInstance(elementType, count);
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        items.SetValue(value, i);
                    }
                    addRange(instance, items);
                    return instance;
                };
            }
            else if (addMethod != null)
            {
                var add = CompileMethodToDelegate(addMethod, type, elementType);
                reader = (stream, session) =>
                {
                    var instance = Activator.CreateInstance(type, true);
                    var count = stream.ReadInt32(session);
                    for (var i = 0; i < count; i++)
                    {
                        var value = stream.ReadObject(session);
                        add(instance, value);
                    }
                    return instance;
                };
            }

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