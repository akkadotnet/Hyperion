#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ByteSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.IO;

namespace Hyperion.ValueSerializers
{
    public class ByteSerializer : SessionIgnorantValueSerializer<byte>
    {
        public const byte Manifest = 4;
        public static readonly ByteSerializer Instance = new ByteSerializer();

        public ByteSerializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static void WriteValueImpl(Stream stream, byte b)
        {
            stream.WriteByte(b);
        }

        public static byte ReadValueImpl(Stream stream)
        {
            return (byte) stream.ReadByte();
        }
    }
}