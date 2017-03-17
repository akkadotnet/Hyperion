﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Bugs.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using Xunit;

namespace Hyperion.Tests
{
    
    public class Bugs
    {

        public class ByteMessage
        {
            public DateTime UtcTime { get; }
            public long LongValue { get; }
            public byte ByteValue { get; }

            public ByteMessage(DateTime utcTime, byte byteValue, long longValue)
            {
                UtcTime = utcTime;
                ByteValue = byteValue;
                LongValue = longValue;
            }

            public override bool Equals(object obj)
            {
                var msg = obj as ByteMessage;
                return msg != null && Equals(msg);
            }

            public bool Equals(ByteMessage other)
            {
                return UtcTime.Equals(other.UtcTime) && LongValue.Equals(other.LongValue) && ByteValue.Equals(other.ByteValue);
            }
        }

        [Fact]
        public void CanSerializeMessageWithByte()
        {
            var stream = new MemoryStream();
            var msg = new ByteMessage(DateTime.UtcNow,1,2);
            var serializer = new Serializer(new SerializerOptions(versionTolerance: true, preserveObjectReferences: true));
            serializer.Serialize(msg, stream);
            stream.Position = 0;
            var res = serializer.Deserialize(stream);
        }

        [Fact]
        public void CanSerialieCustomType_bug()
        {
            var stream = new MemoryStream();
            var serializer = new Serializer(new SerializerOptions(versionTolerance: true, preserveObjectReferences: true));
            var root = new Recover(SnapshotSelectionCriteria.Latest);

            serializer.Serialize(root, stream);
            stream.Position = 0;
            var actual = serializer.Deserialize<Recover>(stream);
        }

        public class SnapshotSelectionCriteria
        {
            public static SnapshotSelectionCriteria Latest { get; set; } = new SnapshotSelectionCriteria()
            {
                Foo = "hello",
            };
            public string Foo { get; set; }
        }

#if SERIALIZATION
        [Serializable]
#endif
        public sealed class Recover
        {
            public static readonly Recover Default = new Recover(SnapshotSelectionCriteria.Latest);
            public Recover(SnapshotSelectionCriteria fromSnapshot, long toSequenceNr = long.MaxValue, long replayMax = long.MaxValue)
            {
                FromSnapshot = fromSnapshot;
                ToSequenceNr = toSequenceNr;
                ReplayMax = replayMax;
            }

            /// <summary>
            /// Criteria for selecting a saved snapshot from which recovery should start. Default is del youngest snapshot.
            /// </summary>
            public SnapshotSelectionCriteria FromSnapshot { get; private set; }

            /// <summary>
            /// Upper, inclusive sequence number bound. Default is no upper bound.
            /// </summary>
            public long ToSequenceNr { get; private set; }

            /// <summary>
            /// Maximum number of messages to replay. Default is no limit.
            /// </summary>
            public long ReplayMax { get; private set; }
        }
    }
}
