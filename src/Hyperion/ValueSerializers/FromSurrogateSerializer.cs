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
    internal sealed class FromSurrogateSerializer : ValueSerializer
    {
        private readonly ValueSerializer _surrogateSerializer;
        private readonly Func<object, object> _translator;
        private readonly bool _preserveObjectReferences;

        public FromSurrogateSerializer(
            Func<object, object> translator,
            ValueSerializer surrogateSerializer,
            bool preserveObjectReferences)
        {
            _translator = translator;
            _surrogateSerializer = surrogateSerializer;
            _preserveObjectReferences = preserveObjectReferences;
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
            if(_preserveObjectReferences)
                session.ReplaceOrAddTrackedDeserializedObject(surrogateValue, value);
            return value;
        }

        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }
    }
}