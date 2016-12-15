#region copyright
// -----------------------------------------------------------------------
//  <copyright file="NullSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class NullSerializer : ValueSerializer
    {
        public const byte Manifest = 0;
        public static readonly NullSerializer Instance = new NullSerializer();

        public override void WriteManifest(Stream stream, SerializerSession session)
        {
            stream.WriteByte(Manifest);
        }

        public override void WriteValue(Stream stream, object value, SerializerSession session)
        {
        }

        public override object ReadValue(Stream stream, DeserializerSession session)
        {
            return null;
        }

        public override Type GetElementType()
        {
            throw new NotSupportedException();
        }
    }
}