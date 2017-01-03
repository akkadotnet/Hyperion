#region copyright
// -----------------------------------------------------------------------
//  <copyright file="UnsupportedTypeSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.Reflection;
using Hyperion.Compilation;
using Hyperion.Internal;

namespace Hyperion.ValueSerializers
{
    //https://github.com/AsynkronIT/Hyperion/issues/115

    public class UnsupportedTypeException:Exception
    {
        public Type Type;
        public UnsupportedTypeException(Type t, string msg):base(msg)
        { }
    }
    public class UnsupportedTypeSerializer:ValueSerializer
    {
        private readonly string _errorMessage;
        private readonly Type _invalidType;
        public UnsupportedTypeSerializer(Type t, string msg)
        {
            _errorMessage = msg;
            _invalidType = t;
        }
        public override int EmitReadValue([NotNull] ICompiler<ObjectReader> c, int stream, int session, [NotNull] FieldInfo field)
        {
            throw new UnsupportedTypeException(_invalidType, _errorMessage);
        }
        public override void EmitWriteValue(ICompiler<ObjectWriter> c, int stream, int fieldValue, int session)
        {
            throw new UnsupportedTypeException(_invalidType, _errorMessage);
        }
        public override object ReadValue([NotNull] Stream stream, [NotNull] DeserializerSession session)
        {
            throw new UnsupportedTypeException(_invalidType, _errorMessage);
        }
        public override void WriteManifest([NotNull] Stream stream, [NotNull] SerializerSession session)
        {
            throw new UnsupportedTypeException(_invalidType, _errorMessage);
        }
        public override void WriteValue([NotNull] Stream stream, object value, [NotNull] SerializerSession session)
        {
            throw new UnsupportedTypeException(_invalidType, _errorMessage);
        }
        public override Type GetElementType()
        {
            throw new UnsupportedTypeException(_invalidType, _errorMessage);
        }
    }
}
