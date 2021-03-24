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
using Hyperion.Extensions;

namespace Hyperion.ValueSerializers
{
    public class DateTimeOffsetSerializer : SessionAwareByteArrayRequiringValueSerializer<DateTimeOffset>
    {
        public const byte Manifest = 10;
        public const int Size = sizeof(long) + sizeof(long);
        public static readonly DateTimeOffsetSerializer Instance = new DateTimeOffsetSerializer();

        public DateTimeOffsetSerializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
        {
        }

        private static void WriteValueImpl(Stream stream, DateTimeOffset dateTimeOffset, byte[] bytes)
        {
            NoAllocBitConverter.GetBytes(dateTimeOffset, bytes);
            stream.Write(bytes, 0, Size);
        }

        public static DateTimeOffset ReadValueImpl(Stream stream, byte[] bytes)
        {
            var dateTime = ReadDateTimeOffset(stream, bytes);
            return dateTime;
        }

        private static DateTimeOffset ReadDateTimeOffset(Stream stream, byte[] bytes)
        {
            stream.ReadFull(bytes, 0, Size);
            var dateTimeTicks = BitConverter.ToInt64(bytes, 0);
            var offsetTicks = BitConverter.ToInt64(bytes, sizeof(long));
            var dateTimeOffset = new DateTimeOffset(dateTimeTicks, TimeSpan.FromTicks(offsetTicks));
            return dateTimeOffset;
        }

        public override int PreallocatedByteBufferSize => Size;
    }
}