#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ObjectReferenceSerializer.cs" company="Akka.NET Team">
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
    public class ObjectReferenceSerializer : ValueSerializer
    {
        public const byte Manifest = 253;
        public static readonly ObjectReferenceSerializer Instance = new ObjectReferenceSerializer();

        public override void WriteManifest(Stream stream, SerializerSession session)
        {
            stream.WriteByte(Manifest);
        }

        public override void WriteValue(Stream stream, object value, SerializerSession session)
        {
            Int32Serializer.WriteValueImpl(stream,(int)value,session);
        }

        public override object ReadValue(Stream stream, DeserializerSession session)
        {
            var id = stream.ReadInt32(session);
            var obj = session.GetDeserializedObject(id);
            return obj;
        }

        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }
    }
}