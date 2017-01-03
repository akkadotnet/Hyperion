#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SByteSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.IO;

namespace Hyperion.ValueSerializers
{
    public class SByteSerializer : SessionIgnorantValueSerializer<sbyte>
    {
        public const byte Manifest = 20;
        public static readonly SByteSerializer Instance = new SByteSerializer();

        public SByteSerializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        public static unsafe void WriteValueImpl(Stream stream, sbyte @sbyte)
        {
            stream.WriteByte(*(byte*) &@sbyte);
        }

        public static unsafe sbyte ReadValueImpl(Stream stream)
        {
            var @byte = (byte) stream.ReadByte();
            return *(sbyte*) &@byte;
        }
    }
}