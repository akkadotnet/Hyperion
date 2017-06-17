#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializePrimitivesBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using BenchmarkDotNet.Attributes;

namespace Hyperion.Benchmarks
{
    [Config(typeof(HyperionConfig))]
    public class SerializePrimitivesBenchmark
    {
        #region init
        private Serializer serializer;
        private MemoryStream stream;

        private Guid guid;
        private string str;
        private DateTime dateTime;
        private TimeSpan time;

        [Setup]
        public void Setup()
        {
            serializer = new Serializer();
            stream = new MemoryStream();
            guid = Guid.NewGuid();
            str = new string('x', 100);
            dateTime = DateTime.UtcNow;
            time = DateTime.UtcNow.TimeOfDay;
        }

        [Cleanup]
        public void Cleanup()
        {
            stream.Dispose();
        }
        #endregion

        [Benchmark] public void Serialize_Boolean() => serializer.Serialize(true, stream);
        [Benchmark] public void Serialize_Byte() => serializer.Serialize((byte)123, stream);
        [Benchmark] public void Serialize_SByte() => serializer.Serialize((sbyte)123, stream);
        [Benchmark] public void Serialize_Int16() => serializer.Serialize((short)1234, stream);
        [Benchmark] public void Serialize_UInt16() => serializer.Serialize((ushort)1234, stream);
        [Benchmark] public void Serialize_Int32() => serializer.Serialize(12345, stream);
        [Benchmark] public void Serialize_UInt32() => serializer.Serialize(12345U, stream);
        [Benchmark] public void Serialize_Int64() => serializer.Serialize(123456L, stream);
        [Benchmark] public void Serialize_UInt64() => serializer.Serialize(123456UL, stream);
        [Benchmark] public void Serialize_Single() => serializer.Serialize(123456.34F, stream);
        [Benchmark] public void Serialize_Double() => serializer.Serialize(123456.34D, stream);
        [Benchmark] public void Serialize_Decimal() => serializer.Serialize(123456.34M, stream);
        [Benchmark] public void Serialize_Guid() => serializer.Serialize(guid, stream);
        [Benchmark] public void Serialize_DateTime() => serializer.Serialize(dateTime, stream);
        [Benchmark] public void Serialize_TimeSpan() => serializer.Serialize(time, stream);
        [Benchmark] public void Serialize_String() => serializer.Serialize(str, stream);
        [Benchmark] public void Serialize_Type() => serializer.Serialize(typeof(int), stream);

        [Benchmark] public void Serialize_Tuple1() => serializer.Serialize(Tuple.Create(1234), stream);
        [Benchmark] public void Serialize_Tuple2() => serializer.Serialize(Tuple.Create(1234, "hello world"), stream);
        [Benchmark] public void Serialize_Tuple3() => serializer.Serialize(Tuple.Create(1234, "hello world", true), stream);
        [Benchmark] public void Serialize_Tuple4() => serializer.Serialize(Tuple.Create(1234, "hello world", true, typeof(int)), stream);
        [Benchmark] public void Serialize_Tuple5() => serializer.Serialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL), stream);
        [Benchmark] public void Serialize_Tuple6() => serializer.Serialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime), stream);
        [Benchmark] public void Serialize_Tuple7() => serializer.Serialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time), stream);
        [Benchmark] public void Serialize_Tuple8() => serializer.Serialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time, guid), stream);

        [Benchmark] public void Serialize_ValueTuple1() => serializer.Serialize(ValueTuple.Create(1234), stream);
        [Benchmark] public void Serialize_ValueTuple2() => serializer.Serialize(ValueTuple.Create(1234, "hello world"), stream);
        [Benchmark] public void Serialize_ValueTuple3() => serializer.Serialize(ValueTuple.Create(1234, "hello world", true), stream);
        [Benchmark] public void Serialize_ValueTuple4() => serializer.Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int)), stream);
        [Benchmark] public void Serialize_ValueTuple5() => serializer.Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL), stream);
        [Benchmark] public void Serialize_ValueTuple6() => serializer.Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime), stream);
        [Benchmark] public void Serialize_ValueTuple7() => serializer.Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time), stream);
        [Benchmark] public void Serialize_ValueTuple8() => serializer.Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time, guid), stream);
    }
}