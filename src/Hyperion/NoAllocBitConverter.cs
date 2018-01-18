﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="NoAllocBitConverter.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Text;
using Hyperion.ValueSerializers;

namespace Hyperion
{
    /// <summary>
    /// Provides methods not allocating the byte buffer but using <see cref="SerializerSession.GetBuffer"/> to lease a buffer.
    /// </summary>
    public static class NoAllocBitConverter
    {
        public static void GetBytes(char value, byte[] bytes)
        {
            GetBytes((short) value, bytes);
        }

        public static unsafe void GetBytes(short value, byte[] bytes)
        {
            fixed (byte* b = bytes)
                *((short*) b) = value;
        }

        public static unsafe void GetBytes(int value, byte[] bytes)
        {
            fixed (byte* b = bytes)
                *((int*) b) = value;
        }

        public static unsafe void GetBytes(long value, byte[] bytes)
        {
            fixed (byte* b = bytes)
                *((long*) b) = value;
        }

        public static void GetBytes(ushort value, byte[] bytes)
        {
            GetBytes((short) value, bytes);
        }

        public static void GetBytes(uint value, byte[] bytes)
        {
            GetBytes((int) value, bytes);
        }

        public static void GetBytes(ulong value, byte[] bytes)
        {
            GetBytes((long) value, bytes);
        }

        public static unsafe void GetBytes(float value, byte[] bytes)
        {
            GetBytes(*(int*) &value, bytes);
        }

        public static unsafe void GetBytes(double value, byte[] bytes)
        {
            GetBytes(*(long*) &value, bytes);
        }

        internal static readonly UTF8Encoding Utf8 = (UTF8Encoding) Encoding.UTF8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe byte[] GetBytes(string str, SerializerSession session, out int byteCount)
        {
            //if first byte is 0 = null
            //if first byte is 254 or less, then length is value - 1
            //if first byte is 255 then the next 4 bytes are an int32 for length
            if (str == null)
            {
                byteCount = 1;
                return new[] {(byte) 0};
            }
            byteCount = Utf8.GetByteCount(str);
            if (byteCount < 254) //short string
            {
                byte[] bytes = session.GetBuffer(byteCount + 1);
                Utf8.GetBytes(str, 0, str.Length, bytes, 1);
                bytes[0] = (byte) (byteCount + 1);
                byteCount += 1;
                return bytes;
            }
            else //long string
            {
                byte[] bytes = session.GetBuffer(byteCount + 1 + 4);
                Utf8.GetBytes(str, 0, str.Length, bytes, 1 + 4);
                bytes[0] = 255;


                fixed (byte* b = bytes)
                    *((int*) (b+1) ) = byteCount;

                byteCount += 1 + 4;

                return bytes;
            }
        }

        public static unsafe void GetBytes(DateTime dateTime, byte[] bytes)
        {
            //datetime size is 9 ticks + kind
            fixed (byte* b = bytes)
                *((long*) b) = dateTime.Ticks;
            bytes[DateTimeSerializer.Size - 1] = (byte) dateTime.Kind;
        }

        public static unsafe void GetBytes(DateTimeOffset dateTime, byte[] bytes)
        {
            //datetimeoffset size is 16 ticks + offset
            fixed (byte* b = bytes)
            {
                *((long*)b) = dateTime.Ticks;
                *((long*)(b + sizeof(long))) = (long)dateTime.Offset.Ticks;
            }
        }
    }
}