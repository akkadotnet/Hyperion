#region copyright
// -----------------------------------------------------------------------
//  <copyright file="MarshaledValueSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2017-2017 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public class MarshalSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type) =>
            !serializer.Options.VersionTolerance && IsFullValueType(type, new HashSet<Type>());

        public override bool CanDeserialize(Serializer serializer, Type type) =>
            !serializer.Options.VersionTolerance && IsFullValueType(type, new HashSet<Type>());

        private bool IsFullValueType(Type type, HashSet<Type> acknowledged)
        {
            if (!type.IsValueType) return false;

            acknowledged.Add(type);

            var fields = type.GetFieldInfosForType();

            var result = true;
            for (int i = 0; i < fields.Length; i++)
            {
                var fieldType = fields[i].FieldType;
                result &= acknowledged.Contains(fieldType) || IsFullValueType(fieldType, acknowledged);
            }

            return result;
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type, ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var ser = new ObjectSerializer(type);
            typeMapping.TryAdd(type, ser);

            var size = Marshal.SizeOf(type);
            ObjectWriter writer = (stream, value, session) =>
            {
                var bin = new byte[size];

                var handle = GCHandle.Alloc(bin, GCHandleType.Pinned);
                var ptr = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(value, ptr, true);
                Marshal.Copy(ptr, bin, 0, size);

                stream.Write(bin, 0, size);
                handle.Free();
            };

            ObjectReader reader = (stream, session) =>
            {
                var bin = new byte[size];
                stream.Read(bin, 0, size);
                var handle = GCHandle.Alloc(bin, GCHandleType.Pinned);
                var ptr = handle.AddrOfPinnedObject();

                Marshal.Copy(bin, 0, ptr, size);

                var value = Marshal.PtrToStructure(ptr, type);
                handle.Free();
                return value;
            };
            ser.Initialize(reader, writer);

            return ser;
        }
    }
}