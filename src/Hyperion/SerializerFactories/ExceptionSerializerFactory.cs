#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ExceptionSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Reflection;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    internal sealed class ExceptionSerializerFactory : ValueSerializerFactory
    {
        private static readonly TypeInfo ExceptionTypeInfo = typeof(Exception).GetTypeInfo();
        private readonly FieldInfo _className;
        private readonly FieldInfo _innerException;
        private readonly FieldInfo _stackTraceString;
        private readonly FieldInfo _remoteStackTraceString;
        private readonly FieldInfo _message;

        public ExceptionSerializerFactory()
        {
            _className = ExceptionTypeInfo.GetField("_className", BindingFlagsEx.All);
            _innerException = ExceptionTypeInfo.GetField("_innerException", BindingFlagsEx.All);
            _message = ExceptionTypeInfo.GetField("_message", BindingFlagsEx.All);
            _remoteStackTraceString = ExceptionTypeInfo.GetField("_remoteStackTraceString", BindingFlagsEx.All);
            _stackTraceString = ExceptionTypeInfo.GetField("_stackTraceString", BindingFlagsEx.All);
        }

        public override bool CanSerialize(Serializer serializer, Type type) => ExceptionTypeInfo.IsAssignableFrom(type.GetTypeInfo());

        public override bool CanDeserialize(Serializer serializer, Type type) => CanSerialize(serializer, type);

#if NETSTANDARD16
        // Workaround for CoreCLR where FormatterServices.GetUninitializedObject is not public
        private static readonly Func<Type, object> GetUninitializedObject =
            (Func<Type, object>)
                typeof(string).GetTypeInfo().Assembly.GetType("System.Runtime.Serialization.FormatterServices")
                    .GetMethod("GetUninitializedObject", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .CreateDelegate(typeof(Func<Type, object>));
#else
        private object GetUninitializedObject(Type type) => System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#endif

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var exceptionSerializer = new ObjectSerializer(type);
            var hasDefaultConstructor = type.GetTypeInfo().GetConstructor(new Type[0]) != null;
            var exceptionObject = hasDefaultConstructor ? Activator.CreateInstance(type) : GetUninitializedObject(type);

            exceptionSerializer.Initialize((stream, session) =>
            {
                var exception = exceptionObject;
                var className = stream.ReadString(session);
                var message = stream.ReadString(session);
                var remoteStackTraceString = stream.ReadString(session);
                var stackTraceString = stream.ReadString(session);
                var innerException = stream.ReadObject(session);

#if NETSTANDARD20
                if (_className != null)
                {
                    _className.SetValue(exception, className);
                }
#else
                _className.SetValue(exception, className);
#endif
                _message.SetValue(exception, message);
                _remoteStackTraceString.SetValue(exception, remoteStackTraceString);
                _stackTraceString.SetValue(exception, stackTraceString);
                _innerException.SetValue(exception, innerException);
                return exception;
            }, (stream, exception, session) =>
            {
#if NETSTANDARD20
                string className = _className == null ? null : (string)_className.GetValue(exception);
#else
                var className = (string)_className.GetValue(exception);
#endif
                var message = (string)_message.GetValue(exception);
                var remoteStackTraceString = (string)_remoteStackTraceString.GetValue(exception);
                var stackTraceString = (string)_stackTraceString.GetValue(exception);
                var innerException = _innerException.GetValue(exception);
                StringSerializer.WriteValueImpl(stream, className, session);
                StringSerializer.WriteValueImpl(stream, message, session);
                StringSerializer.WriteValueImpl(stream, remoteStackTraceString, session);
                StringSerializer.WriteValueImpl(stream, stackTraceString, session);
                stream.WriteObjectWithManifest(innerException, session);
            });
            typeMapping.TryAdd(type, exceptionSerializer);
            return exceptionSerializer;
        }
    }
}