#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DateTimeSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class DateTimeSerializer : SessionAwareByteArrayRequiringValueSerializer<DateTime>
    {
        public const byte Manifest = 5;
        public const int Size = sizeof(long) + sizeof(byte);
        public static readonly DateTimeSerializer Instance = new DateTimeSerializer();

        public DateTimeSerializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        private static void WriteValueImpl(Stream stream, DateTime dateTime, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(dateTime, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static DateTime ReadValueImpl(Stream stream, byte[] bytes)
        {
            var dateTime = ReadDateTime(stream, bytes);
            return dateTime;
        }

        private static DateTime ReadDateTime(Stream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, Size);
            var ticks = BitConverter.ToInt64(bytes, 0);
            var kind = (DateTimeKind) bytes[Size - 1]; //avoid reading a single byte from the stream
            var dateTime = new DateTime(ticks, kind);
            return dateTime;
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}