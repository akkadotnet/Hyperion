#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TypeSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.IO;
using Hyperion.Extensions;
using Hyperion.Internal;

namespace Hyperion.ValueSerializers
{
    internal sealed class TypeSerializer : ValueSerializer
    {
        public const byte Manifest = 16;
        public static readonly TypeSerializer Instance = new TypeSerializer();

        public override void WriteManifest(Stream stream, SerializerSession session)
        {
            ushort typeIdentifier;
            if (session.ShouldWriteTypeManifest(TypeEx.RuntimeType,out typeIdentifier))
            {
                stream.WriteByte(Manifest);
            }
            else
            {
                stream.Write(new[] { ObjectSerializer.ManifestIndex });
                UInt16Serializer.WriteValueImpl(stream,typeIdentifier,session);
            }
        }

        public override void WriteValue(Stream stream, object value, SerializerSession session)
        {
            if (value == null)
            {
                StringSerializer.WriteValueImpl(stream,null,session);
            }
            else
            {
                var type = (Type) value;
                int existingId;
                if (session.Serializer.Options.PreserveObjectReferences && session.TryGetObjectId(type, out existingId))
                {
                    ObjectReferenceSerializer.Instance.WriteManifest(stream, session);
                    ObjectReferenceSerializer.Instance.WriteValue(stream, existingId, session);
                }
                else
                { 
                    if (session.Serializer.Options.PreserveObjectReferences)
                    {
                        session.TrackSerializedObject(type);
                    }
                    //type was not written before, add it to the tacked object list
                    var name = type.GetShortAssemblyQualifiedName();
                    StringSerializer.WriteValueImpl(stream, name, session);
                }
            }
        }

        private static readonly ConcurrentDictionary<string, Type> TypeNameLookup =
            new ConcurrentDictionary<string, Type>();
        public override object ReadValue(Stream stream, DeserializerSession session)
        {
            var bytes = stream.ReadByteArrayKey(session);
            if (bytes == null)
                return null;
            var byteArr = bytes.Value;

            // Read possible rejected keys from the cache
            if(session.Serializer.RejectedKeys.Contains(byteArr))
                throw new EvilDeserializationException(
                    "Unsafe Type Deserialization Detected!",
                    StringEx.FromUtf8Bytes(byteArr.Bytes, 0, byteArr.Bytes.Length));
            if(session.Serializer.UserRejectedKeys.Contains(byteArr))
                throw new UserEvilDeserializationException(
                    "Unsafe Type Deserialization Detected!", 
                    StringEx.FromUtf8Bytes(byteArr.Bytes, 0, byteArr.Bytes.Length));
            
            var shortname = StringEx.FromUtf8Bytes(byteArr.Bytes, 0, byteArr.Bytes.Length);
            var options = session.Serializer.Options;
            
            try
            {
                var type = TypeNameLookup.GetOrAdd(shortname,
                    name => TypeEx.LoadTypeByName(shortname, options.DisallowUnsafeTypes, options.TypeFilter));

                //add the deserialized type to lookup
                if (session.Serializer.Options.PreserveObjectReferences)
                {
                    session.TrackDeserializedObject(type);
                }
                return type;
            }
            catch (UserEvilDeserializationException)
            {
                // Store rejected types in the cache (optimization)
                session.Serializer.UserRejectedKeys.Add(byteArr);
                throw;
            }
            catch (EvilDeserializationException)
            {
                // Store rejected types in the cache (optimization)
                session.Serializer.RejectedKeys.Add(byteArr);
                throw;
            }
        }

        public override Type GetElementType()
        {
            return typeof (Type);
        }
    }
}