#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Int64Serializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class Int64Serializer : SessionAwareByteArrayRequiringValueSerializer<long>
    {
        public const byte Manifest = 2;
        public const int Size = sizeof(long);
        public static readonly Int64Serializer Instance = new Int64Serializer();

        public Int64Serializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, long l, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(l, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static long ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToInt64(bytes, 0);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}