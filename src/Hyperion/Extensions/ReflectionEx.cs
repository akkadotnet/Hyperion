﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ReflectionEx.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hyperion.Extensions
{
    internal static class BindingFlagsEx
    {
        public const BindingFlags All = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    }

    internal static class ReflectionEx
    {
        public static readonly Assembly CoreAssembly = typeof(int).GetTypeInfo().Assembly;

        public static FieldInfo[] GetFieldInfosForType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var fieldInfos = new List<FieldInfo>();
            var current = type;
            while (current != null)
            {
                var tfields =
                    current
                        .GetTypeInfo()
                        .GetFields(BindingFlagsEx.All)
#if SERIALIZATION
                        .Where(f => !f.IsDefined(typeof(NonSerializedAttribute)))
#endif
                        .Where(f => !f.IsDefined(typeof(IgnoreAttribute)))
                        .Where(f => !f.IsStatic)
                        .Where(f => f.FieldType != typeof(IntPtr))
                        .Where(f => f.FieldType != typeof(UIntPtr))
                        .Where(f => f.Name != "_syncRoot"); //HACK: ignore these 

                fieldInfos.AddRange(tfields);
                current = current.GetTypeInfo().BaseType;
            }
            var fields = fieldInfos.OrderBy(f => f.Name).ToArray();
            return fields;
        }
    }
}