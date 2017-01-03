#region copyright
// -----------------------------------------------------------------------
//  <copyright file="CharSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class CharSerializer : SessionAwareByteArrayRequiringValueSerializer<char>
    {
        public const byte Manifest = 15;
        public const int Size = sizeof(char);
        public static readonly CharSerializer Instance = new CharSerializer();

        public CharSerializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static char ReadValueImpl(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            return BitConverter.ToChar(bytes, 0);
        }

        public static void WriteValueImpl(Stream stream, char ch, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(ch, bytes);
            stream.Write(bytes, 0, Size);
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}