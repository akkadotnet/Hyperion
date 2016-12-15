#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Int32Serializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class Int32Serializer : SessionAwareByteArrayRequiringValueSerializer<int>
    {
        public const byte Manifest = 8;
        public const int Size = sizeof(int);
        public static readonly Int32Serializer Instance = new Int32Serializer();

        public Int32Serializer()
            : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, int i, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(i, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static void WriteValueImpl(Stream stream, int i, SerializerSession session)
        {
            var bytes = session.GetBuffer(Size);
            WriteValueImpl(stream, i, bytes);
        }

        public static int ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToInt32(bytes, 0);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}