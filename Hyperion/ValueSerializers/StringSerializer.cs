#region copyright
// -----------------------------------------------------------------------
//  <copyright file="StringSerializer.cs" company="Akka.NET Team">
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
    public class StringSerializer : ValueSerializer
    {
        public const byte Manifest = 7;
        public static readonly StringSerializer Instance = new StringSerializer();

        public static void WriteValueImpl(Stream stream, string s, SerializerSession session)
        {
            int byteCount;
            var bytes = NoAllocBitConverter.GetBytes(s, session, out byteCount);
            stream.Write(bytes, 0, byteCount);
        }

        public static string ReadValueImpl(Stream stream, DeserializerSession session)
        {
            return stream.ReadString(session);
        }

        public override void WriteManifest(Stream stream, SerializerSession session)
        {
            stream.WriteByte(Manifest);
        }

        public override void WriteValue(Stream stream, object value, SerializerSession session)
        {
            WriteValueImpl(stream, (string) value, session);
        }

        public override object ReadValue(Stream stream, DeserializerSession session)
        {
            return ReadValueImpl(stream, session);
        }

        public override Type GetElementType()
        {
            return typeof(string);
        }
    }
}