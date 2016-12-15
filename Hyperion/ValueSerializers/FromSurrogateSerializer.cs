#region copyright
// -----------------------------------------------------------------------
//  <copyright file="FromSurrogateSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class FromSurrogateSerializer : ValueSerializer
    {
        private readonly ValueSerializer _surrogateSerializer;
        private readonly Func<object, object> _translator;

        public FromSurrogateSerializer(Func<object, object> translator, ValueSerializer surrogateSerializer)
        {
            _translator = translator;
            _surrogateSerializer = surrogateSerializer;
        }

        public override void WriteManifest(Stream stream, SerializerSession session)
        {
            throw new NotSupportedException();
        }

        public override void WriteValue(Stream stream, object value, SerializerSession session)
        {
            throw new NotSupportedException();
        }

        public override object ReadValue(Stream stream, DeserializerSession session)
        {
            var surrogateValue = _surrogateSerializer.ReadValue(stream, session);
            var value = _translator(surrogateValue);
            return value;
        }

        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }
    }
}