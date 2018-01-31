﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="FloatSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    internal sealed class FloatSerializer : SessionAwareByteArrayRequiringValueSerializer<float>
    {
        public const byte Manifest = 12;
        public const int Size = sizeof(float);
        public static readonly FloatSerializer Instance = new FloatSerializer();

        public FloatSerializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, float f, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(f, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static float ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToSingle(bytes, 0);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}