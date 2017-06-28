#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializePrimitivesBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
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

        [Benchmark] public void Boolean() => SerializeAndDeserialize(true);
        [Benchmark] public void Byte() => SerializeAndDeserialize((byte)123);
        [Benchmark] public void SByte() => SerializeAndDeserialize((sbyte)123);
        [Benchmark] public void Int16() => SerializeAndDeserialize((short)1234);
        [Benchmark] public void UInt16() => SerializeAndDeserialize((ushort)1234);
        [Benchmark] public void Int32() => SerializeAndDeserialize(12345);
        [Benchmark] public void UInt32() => SerializeAndDeserialize(12345U);
        [Benchmark] public void Int64() => SerializeAndDeserialize(123456L);
        [Benchmark] public void UInt64() => SerializeAndDeserialize(123456UL);
        [Benchmark] public void Single() => SerializeAndDeserialize(123456.34F);
        [Benchmark] public void Double() => SerializeAndDeserialize(123456.34D);
        [Benchmark] public void Decimal() => SerializeAndDeserialize(123456.34M);
        [Benchmark] public void Guids() => SerializeAndDeserialize(guid);
        [Benchmark] public void DateTimes() => SerializeAndDeserialize(dateTime);
        [Benchmark] public void TimeSpan() => SerializeAndDeserialize(time);
        [Benchmark] public void String() => SerializeAndDeserialize(str);
        [Benchmark] public void Type() => SerializeAndDeserialize(typeof(int));

        [Benchmark] public void Tuple1() => SerializeAndDeserialize(Tuple.Create(1234));
        [Benchmark] public void Tuple2() => SerializeAndDeserialize(Tuple.Create(1234, "hello world"));
        [Benchmark] public void Tuple3() => SerializeAndDeserialize(Tuple.Create(1234, "hello world", true));
        [Benchmark] public void Tuple4() => SerializeAndDeserialize(Tuple.Create(1234, "hello world", true, typeof(int)));
        [Benchmark] public void Tuple5() => SerializeAndDeserialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL));
        [Benchmark] public void Tuple6() => SerializeAndDeserialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime));
        [Benchmark] public void Tuple7() => SerializeAndDeserialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time));
        [Benchmark] public void Tuple8() => SerializeAndDeserialize(Tuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time, guid));

        [Benchmark] public void ValueTuple1() => SerializeAndDeserialize(ValueTuple.Create(1234));
        [Benchmark] public void ValueTuple2() => SerializeAndDeserialize(ValueTuple.Create(1234, "hello world"));
        [Benchmark] public void ValueTuple3() => SerializeAndDeserialize(ValueTuple.Create(1234, "hello world", true));
        [Benchmark] public void ValueTuple4() => SerializeAndDeserialize(ValueTuple.Create(1234, "hello world", true, typeof(int)));
        [Benchmark] public void ValueTuple5() => SerializeAndDeserialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL));
        [Benchmark] public void ValueTuple6() => SerializeAndDeserialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime));
        [Benchmark] public void ValueTuple7() => SerializeAndDeserialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time));
        [Benchmark] public void ValueTuple8() => SerializeAndDeserialize(ValueTuple.Create(1234, "hello world", true, typeof(int), 345666UL, dateTime, time, guid));
    }
}