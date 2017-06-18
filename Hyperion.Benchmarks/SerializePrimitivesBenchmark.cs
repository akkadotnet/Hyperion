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
    public class SerializePrimitivesBenchmark : HyperionBenchmark
    {
        #region init
        private Guid guid;
        private string str;
        private DateTime dateTime;
        private TimeSpan time;
    
        protected override void Init()
        {
            base.Init();

            guid = Guid.NewGuid();
            str = new string('x', 100);
            dateTime = DateTime.UtcNow;
            time = DateTime.UtcNow.TimeOfDay;
        }

        #endregion

        [Benchmark] public void Serialize_Boolean() => Serialize(true);
        [Benchmark] public void Serialize_Byte() => Serialize((byte)123);
        [Benchmark] public void Serialize_SByte() => Serialize((sbyte)123);
        [Benchmark] public void Serialize_Int16() => Serialize((short)1234);
        [Benchmark] public void Serialize_UInt16() => Serialize((ushort)1234);
        [Benchmark] public void Serialize_Int32() => Serialize(12345);
        [Benchmark] public void Serialize_UInt32() => Serialize(12345U);
        [Benchmark] public void Serialize_Int64() => Serialize(123456L);
        [Benchmark] public void Serialize_UInt64() => Serialize(123456UL);
        [Benchmark] public void Serialize_Single() => Serialize(123456.34F);
        [Benchmark] public void Serialize_Double() => Serialize(123456.34D);
        [Benchmark] public void Serialize_Decimal() => Serialize(123456.34M);
        [Benchmark] public void Serialize_Guid() => Serialize(guid);
        [Benchmark] public void Serialize_DateTime() => Serialize(dateTime);
        [Benchmark] public void Serialize_TimeSpan() => Serialize(time);
        [Benchmark] public void Serialize_String() => Serialize(str);
        [Benchmark] public void Serialize_Type() => Serialize(typeof(int));

        [Benchmark] public void Serialize_Tuple1() => Serialize(Tuple.Create(1234));
        [Benchmark] public void Serialize_Tuple2() => Serialize(Tuple.Create(1234, "hello world"));
        [Benchmark] public void Serialize_Tuple3() => Serialize(Tuple.Create(1234, "hello world", true));
        [Benchmark] public void Serialize_Tuple4() => Serialize(Tuple.Create(1234, "hello world", true, typeof(int)));
        [Benchmark] public void Serialize_Tuple5() => Serialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL));
        [Benchmark] public void Serialize_Tuple6() => Serialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime));
        [Benchmark] public void Serialize_Tuple7() => Serialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time));
        [Benchmark] public void Serialize_Tuple8() => Serialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time, guid));

        [Benchmark] public void Serialize_ValueTuple1() => Serialize(ValueTuple.Create(1234));
        [Benchmark] public void Serialize_ValueTuple2() => Serialize(ValueTuple.Create(1234, "hello world"));
        [Benchmark] public void Serialize_ValueTuple3() => Serialize(ValueTuple.Create(1234, "hello world", true));
        [Benchmark] public void Serialize_ValueTuple4() => Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int)));
        [Benchmark] public void Serialize_ValueTuple5() => Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL));
        [Benchmark] public void Serialize_ValueTuple6() => Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime));
        [Benchmark] public void Serialize_ValueTuple7() => Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time));
        [Benchmark] public void Serialize_ValueTuple8() => Serialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time, guid));
    }
}