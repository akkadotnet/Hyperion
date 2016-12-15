#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializerSession.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;

namespace Hyperion
{
    public class SerializerSession
    {
        public const int MinBufferSize = 9;
        private readonly Dictionary<object, int> _objects;
        public readonly Serializer Serializer;
        private LinkedList<Type> _trackedTypes;
        private byte[] _buffer = new byte[MinBufferSize];

        private int _nextObjectId;
        private readonly ushort _nextTypeId;

        public SerializerSession(Serializer serializer)
        {
            Serializer = serializer;
            if (serializer.Options.PreserveObjectReferences)
            {
                _objects = new Dictionary<object, int>();
            }
            _nextTypeId = (ushort)(serializer.Options.KnownTypes.Length );
        }

        public void TrackSerializedObject(object obj)
        {
            try
            {
                _objects.Add(obj, _nextObjectId++);
            }
            catch (Exception x)
            {
                throw new Exception("Error tracking object ", x);
            }
        }

        public bool TryGetObjectId(object obj, out int objectId)
        {
            return _objects.TryGetValue(obj, out objectId);
        }

        public bool ShouldWriteTypeManifest(Type type, out ushort index)
        {
            return !TryGetValue(type, out index);
        }

        public byte[] GetBuffer(int length)
        {
            if (length <= _buffer.Length)
                return _buffer;

            length = Math.Max(length, _buffer.Length*2);

            _buffer = new byte[length];

            return _buffer;
        }

        public bool TryGetValue(Type key, out ushort value)
        {
            if (_trackedTypes == null || _trackedTypes.Count == 0)
            {
                value = 0;
                return false;
            }

            ushort j = _nextTypeId;
            foreach (var t in _trackedTypes)
            {
                if (key == t)
                {
                    value = j;
                    return true;
                }
                j++;
            }

            value = 0;
            return false;
        }

        public void TrackSerializedType(Type type)
        {
            if (_trackedTypes == null)
            {
                _trackedTypes = new LinkedList<Type>();
            }
            _trackedTypes.AddLast(type);
        }
    }
}