﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DoubleSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    internal sealed class DoubleSerializer : SessionAwareByteArrayRequiringValueSerializer<double>
    {
        public const byte Manifest = 13;
        public const int Size = sizeof(double);
        public static readonly DoubleSerializer Instance = new DoubleSerializer();

        public DoubleSerializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, double d, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(d, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static double ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToDouble(bytes, 0);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}