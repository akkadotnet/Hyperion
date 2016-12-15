#region copyright
// -----------------------------------------------------------------------
//  <copyright file="UInt16Serializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class UInt16Serializer : SessionAwareByteArrayRequiringValueSerializer<ushort>
    {
        public const byte Manifest = 17;
        public const int Size = sizeof(ushort);
        public static readonly UInt16Serializer Instance = new UInt16Serializer();

        public UInt16Serializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, ushort u, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(u, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static void WriteValueImpl(Stream stream, ushort u, SerializerSession session)
        {
            WriteValueImpl(stream, u, session.GetBuffer(Size));
        }

        public static ushort ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}