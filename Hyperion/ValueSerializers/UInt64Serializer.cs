#region copyright
// -----------------------------------------------------------------------
//  <copyright file="UInt64Serializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class UInt64Serializer : SessionAwareByteArrayRequiringValueSerializer<ulong>
    {
        public const byte Manifest = 19;
        public const int Size = sizeof(ulong);
        public static readonly UInt64Serializer Instance = new UInt64Serializer();

        public UInt64Serializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, ulong ul, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(ul, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static ulong ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}