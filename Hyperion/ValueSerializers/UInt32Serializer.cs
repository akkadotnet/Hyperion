#region copyright
// -----------------------------------------------------------------------
//  <copyright file="UInt32Serializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class UInt32Serializer : SessionAwareByteArrayRequiringValueSerializer<uint>
    {
        public const byte Manifest = 18;
        public const int Size = sizeof(uint);
        public static readonly UInt32Serializer Instance = new UInt32Serializer();

        public UInt32Serializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, uint u, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(u, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static uint ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}