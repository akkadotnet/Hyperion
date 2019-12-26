#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TypeEx.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Hyperion.Extensions
{
    internal static class TypeEx
    {
        //Why not inline typeof you ask?
        //Because it actually generates calls to get the type.
        //We prefetch all primitives here
        public static readonly Type SystemObject = typeof(object);
        public static readonly Type Int32Type = typeof(int);
        public static readonly Type Int64Type = typeof(long);
        public static readonly Type Int16Type = typeof(short);
        public static readonly Type UInt32Type = typeof(uint);
        public static readonly Type UInt64Type = typeof(ulong);
        public static readonly Type UInt16Type = typeof(ushort);
        public static readonly Type ByteType = typeof(byte);
        public static readonly Type SByteType = typeof(sbyte);
        public static readonly Type BoolType = typeof(bool);
        public static readonly Type DateTimeType = typeof(DateTime);
        public static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);
        public static readonly Type StringType = typeof(string);
        public static readonly Type GuidType = typeof(Guid);
        public static readonly Type FloatType = typeof(float);
        public static readonly Type DoubleType = typeof(double);
        public static readonly Type DecimalType = typeof(decimal);
        public static readonly Type CharType = typeof(char);
        public static readonly Type ByteArrayType = typeof(byte[]);
        public static readonly Type TypeType = typeof(Type);
        public static readonly Type RuntimeType = Type.GetType("System.RuntimeType");

        public static bool IsHyperionPrimitive(this Type type)
        {
            return type == Int32Type ||
                   type == Int64Type ||
                   type == Int16Type ||
                   type == UInt32Type ||
                   type == UInt64Type ||
                   type == UInt16Type ||
                   type == ByteType ||
                   type == SByteType ||
                   type == DateTimeType ||
                   type == DateTimeOffsetType ||
                   type == BoolType ||
                   type == StringType ||
                   type == GuidType ||
                   type == FloatType ||
                   type == DoubleType ||
                   type == DecimalType ||
                   type == CharType;
            //add TypeSerializer with null support
        }

#if NETSTANDARD16
    //HACK: the GetUnitializedObject actually exists in .NET Core, its just not public
        private static readonly Func<Type, object> getUninitializedObjectDelegate = (Func<Type, object>)
            typeof(string)
                .GetTypeInfo()
                .Assembly
                .GetType("System.Runtime.Serialization.FormatterServices")
                ?.GetTypeInfo()
                ?.GetMethod("GetUninitializedObject", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                ?.CreateDelegate(typeof(Func<Type, object>));

        public static object GetEmptyObject(this Type type)
        {
            return getUninitializedObjectDelegate(type);
        }
#else
        public static object GetEmptyObject(this Type type) => System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#endif

        public static bool IsOneDimensionalArray(this Type type)
        {
            return type.IsArray && type.GetArrayRank() == 1;
        }

        public static bool IsOneDimensionalPrimitiveArray(this Type type)
        {
            return type.IsArray && type.GetArrayRank() == 1 && type.GetElementType().IsHyperionPrimitive();
        }

        private static readonly ConcurrentDictionary<ByteArrayKey, Type> TypeNameLookup =
            new ConcurrentDictionary<ByteArrayKey, Type>(ByteArrayKeyComparer.Instance);

        public static byte[] GetTypeManifest(IReadOnlyCollection<byte[]> fieldNames)
        {
            IEnumerable<byte> result = new[] { (byte)fieldNames.Count };
            foreach (var name in fieldNames)
            {
                var encodedLength = BitConverter.GetBytes(name.Length);
                result = result.Concat(encodedLength);
                result = result.Concat(name);
            }
            var versionTolerantHeader = result.ToArray();
            return versionTolerantHeader;
        }

        private static Type GetTypeFromManifestName(Stream stream, DeserializerSession session)
        {
            var bytes = stream.ReadLengthEncodedByteArray(session);
            var byteArr = ByteArrayKey.Create(bytes);
            return TypeNameLookup.GetOrAdd(byteArr, b =>
            {
                var shortName = StringEx.FromUtf8Bytes(b.Bytes, 0, b.Bytes.Length);
#if NET45
                if (shortName.Contains("System.Private.CoreLib,%core%"))
                {
                    shortName = shortName.Replace("System.Private.CoreLib,%core%", "mscorlib,%core%");
                }
#endif
#if NETSTANDARD
                if (shortName.Contains("mscorlib,%core%"))
                {
                    shortName = shortName.Replace("mscorlib,%core%", "System.Private.CoreLib,%core%");
                }
#endif

                var typename = ToQualifiedAssemblyName(shortName);
                return Type.GetType(typename, true);
            });
        }
        public static Type GetTypeFromManifestFull(Stream stream, DeserializerSession session)
        {
            var type = GetTypeFromManifestName(stream, session);
            session.TrackDeserializedType(type);
            return type;
        }

        public static Type GetTypeFromManifestVersion(Stream stream, DeserializerSession session)
        {
            var type = GetTypeFromManifestName(stream, session);

            var fieldCount = stream.ReadByte();
            for (var i = 0; i < fieldCount; i++)
            {
                var fieldName = stream.ReadLengthEncodedByteArray(session);

            }

            session.TrackDeserializedTypeWithVersion(type, null);
            return type;
        }

        public static Type GetTypeFromManifestIndex(int typeId, DeserializerSession session)
        {

            var type = session.GetTypeFromTypeId(typeId);
            return type;
        }

        public static bool IsNullable(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetNullableElement(this Type type)
        {
            return type.GetTypeInfo().GetGenericArguments()[0];
        }

        public static bool IsFixedSizeType(this Type type)
        {
            return type == Int16Type ||
                   type == Int32Type ||
                   type == Int64Type ||
                   type == BoolType ||
                   type == UInt16Type ||
                   type == UInt32Type ||
                   type == UInt64Type ||
                   type == CharType;
        }

        public static int GetTypeSize(this Type type)
        {
            if (type == Int16Type)
                return sizeof(short);
            if (type == Int32Type)
                return sizeof (int);
            if (type == Int64Type)
                return sizeof (long);
            if (type == BoolType)
                return sizeof (bool);
            if (type == UInt16Type)
                return sizeof (ushort);
            if (type == UInt32Type)
                return sizeof (uint);
            if (type == UInt64Type)
                return sizeof (ulong);
            if (type == CharType)
                return sizeof(char);

            throw new NotSupportedException();
        }
        private static readonly string CoreAssemblyQualifiedName = 1.GetType().AssemblyQualifiedName;
        private static readonly string CoreAssemblyName = GetCoreAssemblyName();

        private static readonly Regex cleanAssemblyVersionRegex = new Regex(
            "(, Version=([\\d\\.]+))?(, Culture=[^,\\] \\t]+)?(, PublicKeyToken=(null|[\\da-f]+))?",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private static string GetCoreAssemblyName()
        {
            var part = CoreAssemblyQualifiedName.Substring(CoreAssemblyQualifiedName.IndexOf(", Version", StringComparison.Ordinal));
            return part;
        }

        public static string GetShortAssemblyQualifiedName(this Type self)
        {
            var name = self.AssemblyQualifiedName;
            name = name.Replace(CoreAssemblyName, ",%core%");
            name = cleanAssemblyVersionRegex.Replace(name, string.Empty);
            return name;
        }

        public static string ToQualifiedAssemblyName(string shortName)
        {
            var res = shortName.Replace(",%core%", CoreAssemblyName);
            return res;
        }

        /// <summary>
        /// Search for a method by name, parameter types, and binding flags.  
        /// Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType,
                                              string name,
                                              BindingFlags bindingFlags,
                                              params Type[] parameterTypes)
        {
            MethodInfo matchingMethod = null;

            // Check all methods with the specified name, including in base classes
            GetMethodExt(ref matchingMethod, thisType, name, bindingFlags, parameterTypes);

            // If we're searching an interface, we have to manually search base interfaces
            if (matchingMethod == null && thisType.GetTypeInfo().IsInterface)
            {
                foreach (Type interfaceType in thisType.GetInterfaces())
                    GetMethodExt(ref matchingMethod,
                        interfaceType,
                        name,
                        bindingFlags,
                        parameterTypes);
            }

            return matchingMethod;
        }

        private static void GetMethodExt(ref MethodInfo matchingMethod,
                                         Type type,
                                         string name,
                                         BindingFlags bindingFlags,
                                         params Type[] parameterTypes)
        {
            // Check all methods with the specified name, including in base classes
            foreach (MethodInfo methodInfo in type.GetTypeInfo().GetMember(name,
                MemberTypes.Method,
                bindingFlags))
            {
                // Check that the parameter counts and types match, 
                // with 'loose' matching on generic parameters
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length == parameterTypes.Length)
                {
                    int i = 0;
                    for (; i < parameterInfos.Length; ++i)
                    {
                        if (!parameterInfos[i].ParameterType
                                              .IsSimilarType(parameterTypes[i]))
                            break;
                    }
                    if (i == parameterInfos.Length)
                    {
                        if (matchingMethod == null)
                            matchingMethod = methodInfo;
                        else
                            throw new AmbiguousMatchException(
                                "More than one matching method found!");
                    }
                }
            }
        }

        /// <summary>
        /// Special type used to match any generic parameter type in GetMethodExt().
        /// </summary>
        public class T
        { }

        /// <summary>
        /// Determines if the two types are either identical, or are both generic 
        /// parameters or generic types with generic parameters in the same
        ///  locations (generic parameters match any other generic paramter,
        /// and concrete types).
        /// </summary>
        private static bool IsSimilarType(this Type thisType, Type type)
        {
            // Ignore any 'ref' types
            if (thisType.IsByRef)
                thisType = thisType.GetElementType();
            if (type.IsByRef)
                type = type.GetElementType();

            // Handle array types
            if (thisType.IsArray && type.IsArray)
                return thisType.GetElementType().IsSimilarType(type.GetElementType());

            // If the types are identical, or they're both generic parameters 
            // or the special 'T' type, treat as a match
            if (thisType == type || thisType.IsGenericParameter || thisType == typeof(T) || type.IsGenericParameter || type == typeof(T))
                return true;

            // Handle any generic arguments
            if (thisType.GetTypeInfo().IsGenericType && type.GetTypeInfo().IsGenericType)
            {
                Type[] thisArguments = thisType.GetGenericArguments();
                Type[] arguments = type.GetGenericArguments();
                if (thisArguments.Length == arguments.Length)
                {
                    for (int i = 0; i < thisArguments.Length; ++i)
                    {
                        if (!thisArguments[i].IsSimilarType(arguments[i]))
                            return false;
                    }
                    return true;
                }
            }

            return false;
        }
    }
}