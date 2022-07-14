﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ExceptionSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    internal sealed class AggregateExceptionSerializerFactory : ValueSerializerFactory
    {
        private static readonly TypeInfo ExceptionTypeInfo = typeof(Exception).GetTypeInfo();
        private static readonly TypeInfo AggregateExceptionTypeInfo = typeof(AggregateException).GetTypeInfo();
        private readonly FieldInfo _className;
        private readonly FieldInfo _innerException;
        private readonly FieldInfo _stackTraceString;
        private readonly FieldInfo _remoteStackTraceString;
        private readonly FieldInfo _message;
        private readonly FieldInfo _innerExceptions;

        public AggregateExceptionSerializerFactory()
        {
            _className = ExceptionTypeInfo.GetField("_className", BindingFlagsEx.All);
            _innerException = ExceptionTypeInfo.GetField("_innerException", BindingFlagsEx.All);
            _message = AggregateExceptionTypeInfo.GetField("_message", BindingFlagsEx.All);
            _remoteStackTraceString = ExceptionTypeInfo.GetField("_remoteStackTraceString", BindingFlagsEx.All);
            _stackTraceString = ExceptionTypeInfo.GetField("_stackTraceString", BindingFlagsEx.All);
            _innerExceptions = AggregateExceptionTypeInfo.GetField("m_innerExceptions", BindingFlagsEx.All);
        }

        public override bool CanSerialize(Serializer serializer, Type type) => 
            AggregateExceptionTypeInfo.IsAssignableFrom(type.GetTypeInfo());

        public override bool CanDeserialize(Serializer serializer, Type type) => CanSerialize(serializer, type);

        private static readonly Func<Type,object> GetUninitializedObject = System.Runtime.Serialization.FormatterServices.GetUninitializedObject;

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var exceptionSerializer = new ObjectSerializer(type);
            exceptionSerializer.Initialize((stream, session) =>
            {
                var info = new SerializationInfo(type, new FormatterConverter());

                info.AddValue("ClassName", stream.ReadString(session), typeof (string));
                info.AddValue("Message", stream.ReadString(session), typeof (string));
                info.AddValue("Data", stream.ReadObject(session), typeof (IDictionary));
                info.AddValue("InnerException", stream.ReadObject(session), typeof (Exception));
                info.AddValue("HelpURL", stream.ReadString(session), typeof (string));
                info.AddValue("StackTraceString", stream.ReadString(session), typeof (string));
                info.AddValue("RemoteStackTraceString", stream.ReadString(session), typeof (string));
                info.AddValue("RemoteStackIndex", stream.ReadInt32(session), typeof (int));
                info.AddValue("ExceptionMethod", stream.ReadString(session), typeof (string));
                info.AddValue("HResult", stream.ReadInt32(session));
                info.AddValue("Source", stream.ReadString(session), typeof (string));
                info.AddValue("InnerExceptions", stream.ReadObject(session), typeof (Exception[]));
                
                return Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, new object[]{info, new StreamingContext()}, null);
            }, (stream, exception, session) =>
            {
                var info = new SerializationInfo(type, new FormatterConverter());
                var context = new StreamingContext();
                ((AggregateException)exception).GetObjectData(info, context);

                var className = info.GetString("ClassName");
                var message = info.GetString("Message");
                var data = info.GetValue("Data", typeof(IDictionary));
                var innerException = info.GetValue("InnerException", typeof(Exception));
                var helpUrl = info.GetString("HelpURL");
                var stackTraceString = info.GetString("StackTraceString");
                var remoteStackTraceString = info.GetString("RemoteStackTraceString");
                var remoteStackIndex = info.GetInt32("RemoteStackIndex");
                var exceptionMethod = info.GetString("ExceptionMethod");
                var hResult = info.GetInt32("HResult");
                var source = info.GetString("Source");
                var innerExceptions = (Exception[]) info.GetValue("InnerExceptions", typeof(Exception[]));
                
                StringSerializer.WriteValueImpl(stream, className, session);
                StringSerializer.WriteValueImpl(stream, message, session);
                stream.WriteObjectWithManifest(data, session);
                stream.WriteObjectWithManifest(innerException, session);
                StringSerializer.WriteValueImpl(stream, helpUrl, session);
                StringSerializer.WriteValueImpl(stream, stackTraceString, session);
                StringSerializer.WriteValueImpl(stream, remoteStackTraceString, session);
                Int32Serializer.WriteValueImpl(stream, remoteStackIndex, session);
                StringSerializer.WriteValueImpl(stream, exceptionMethod, session);
                Int32Serializer.WriteValueImpl(stream, hResult, session);
                StringSerializer.WriteValueImpl(stream, source, session);
                stream.WriteObjectWithManifest(innerExceptions, session);
            });
            if (serializer.Options.KnownTypesDict.TryGetValue(type, out var index))
            {
                var wrapper = new KnownTypeObjectSerializer(exceptionSerializer, index);
                typeMapping.TryAdd(type, wrapper);
            }
            else
                typeMapping.TryAdd(type, exceptionSerializer);
            return exceptionSerializer;
        }
    }
}