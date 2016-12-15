#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Int16Serializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class Int16Serializer : SessionAwareByteArrayRequiringValueSerializer<short>
    {
        public const byte Manifest = 3;
        public const int Size = sizeof(short);
        public static readonly Int16Serializer Instance = new Int16Serializer();

        public Int16Serializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, short sh, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(sh, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static short ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToInt16(bytes, 0);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}