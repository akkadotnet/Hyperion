#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ToSurrogateSerializer.cs" company="Akka.NET Team">
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
    public class ToSurrogateSerializer : ValueSerializer
    {
        private readonly Func<object, object> _translator;

        public ToSurrogateSerializer(Func<object, object> translator)
        {
            _translator = translator;
        }

        public override void WriteManifest(Stream stream, SerializerSession session)
        {
            //intentionally left blank
        }

        public override void WriteValue(Stream stream, object value, SerializerSession session)
        {
            var surrogateValue = _translator(value);
            stream.WriteObjectWithManifest(surrogateValue, session);
        }

        public override object ReadValue(Stream stream, DeserializerSession session)
        {
            throw new NotSupportedException();
        }

        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }
    }
}