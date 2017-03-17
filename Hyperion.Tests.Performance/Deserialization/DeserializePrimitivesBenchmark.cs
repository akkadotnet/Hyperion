﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DeserializePrimitivesBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using NBench;

namespace Hyperion.Tests.Performance.Deserialization
{
    public class ByteDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((byte)120);
        }

        [PerfBenchmark(
            Description = "Benchmark System.Byte deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 9000000)]
        public void Deserialize_Byte()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<byte>();
        }
    }

    public class Int16DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((short)12389);
        }

        [PerfBenchmark(
            Description = "Benchmark System.Int16 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 7500000)]
        public void Deserialize_Int16()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<short>();
        }
    }

    public class Int32DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(1238919);
        }

        [PerfBenchmark(
            Description = "Benchmark System.Int32 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 12000000)]
        public void Deserialize_Int32()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<int>();
        }
    }

    public class Int64DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(93111238919L);
        }

        [PerfBenchmark(
            Description = "Benchmark System.Int64 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 10000000)]
        public void Deserialize_Int64()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<long>();
        }
    }

    public class SByteDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((sbyte)-120);
        }

        [PerfBenchmark(
            Description = "Benchmark System.SByte deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 15000000)]
        public void Deserialize_SByte()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<sbyte>();
        }
    }

    public class UInt16DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((ushort)12389);
        }

        [PerfBenchmark(
            Description = "Benchmark System.UInt16 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 11000000)]
        public void Deserialize_UInt16()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<ushort>();
        }
    }

    public class UInt32DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((uint)11238919);
        }

        [PerfBenchmark(
            Description = "Benchmark System.UInt32 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 7000000)]
        public void Deserialize_UInt32()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<uint>();
        }
    }

    public class UInt64DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((ulong)793111238919L);
        }

        [PerfBenchmark(
            Description = "Benchmark System.UInt64 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 9000000)]
        public void Deserialize_UInt64()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<ulong>();
        }
    }

    public class SingleDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((float)1.21355);
        }

        [PerfBenchmark(
            Description = "Benchmark System.Single deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 6500000)]
        public void Deserialize_Single()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<float>();
        }
    }

    public class DoubleDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((double)1.21355);
        }

        [PerfBenchmark(
            Description = "Benchmark System.Double deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 6000000)]
        public void Deserialize_Double()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<double>();
        }
    }

    public class DecimalDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith((decimal)1.21355);
        }

        [PerfBenchmark(
            Description = "Benchmark System.Decimal deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 6200000)]
        public void Deserialize_Decimal()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<decimal>();
        }
    }

    public class BoolDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(true);
        }

        [PerfBenchmark(
            Description = "Benchmark System.Boolean deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 9000000)]
        public void Deserialize_Bool()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<bool>();
        }
    }

    public class StringDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            var s = new string('x', 100);
            InitStreamWith(s);
        }

        [PerfBenchmark(
            Description = "Benchmark System.String deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 3500000)]
        public void Deserialize_String()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<string>();
        }
    }

    public class GuidDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Guid.NewGuid());
        }

        [PerfBenchmark(
            Description = "Benchmark System.Guid deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 5500000)]
        public void Deserialize_Guid()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Guid>();
        }
    }

    public class TimeSpanDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(DateTime.Now.TimeOfDay);
        }

        [PerfBenchmark(
            Description = "Benchmark System.TimeSpan deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 1500000)]
        public void Deserialize_TimeSpan()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<TimeSpan>();
        }
    }

    public class DateTimeDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(DateTime.Now);
        }

        [PerfBenchmark(
            Description = "Benchmark System.DateTime deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 10000000)]
        public void Deserialize_DateTime()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<DateTime>();
        }
    }

    public class TypeDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(typeof(int));
        }

        [PerfBenchmark(
            Description = "Benchmark System.Type deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 90000)]
        public void Deserialize_Type()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Type>();
        }
    }

    public class Tuple1DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Tuple.Create(123));
        }

        [PerfBenchmark(
            Description = "Benchmark System.Tuple`1 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 1100000)]
        public void Deserialize_Tuple1()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Tuple<int>>();
        }
    }

    public class Tuple2DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Tuple.Create(123, 1234));
        }

        [PerfBenchmark(
            Description = "Benchmark System.Tuple`2 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 400000)]
        public void Deserialize_Tuple2()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Tuple<int, int>>();
        }
    }

    public class Tuple3DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Tuple.Create(123, 234, 345));
        }

        [PerfBenchmark(
            Description = "Benchmark System.Tuple`3 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 350000)]
        public void Deserialize_Tuple3()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Tuple<int, int, int>>();
        }
    }

    public class Tuple4DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Tuple.Create(123, 2345, 3456, 4501));
        }

        [PerfBenchmark(
            Description = "Benchmark System.Tuple`4 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 260000)]
        public void Deserialize_Tuple4()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Tuple<int, int, int, int>>();
        }
    }

    public class Tuple5DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Tuple.Create(123, 2345, 3245, 4561, 746756));
        }

        
        [PerfBenchmark(
            Description = "Benchmark System.Tuple`5 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 220000)]
        public void Deserialize_Tuple5()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Tuple<int, int, int, int, int>>();
        }
    }

    public class Tuple6DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Tuple.Create(123, 56568, 3445, 568567, 234236, 5821));
        }

        
        [PerfBenchmark(
            Description = "Benchmark System.Tuple`6 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 175000)]
        public void Deserialize_Tuple6()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Tuple<int, int, int, int, int, int>>();
        }
    }

    public class Tuple7DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Tuple.Create(123, 56568, 3445, 568567, 234236, 5821, 2456));
        }

        
        [PerfBenchmark(
            Description = "Benchmark System.Tuple`7 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 260000)]
        public void Deserialize_Tuple7()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Tuple<int, int, int, int, int, int, int>>();
        }
    }

    public class Tuple8DeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(Tuple.Create(123, 56568, 3445, 568567, 234236, 5821, 2456, 71231));
        }

        [PerfBenchmark(
            Description = "Benchmark System.Tuple`8 deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test,
            Skip = "FIXME: counter is always 0")]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 4000000)]
        public void Deserialize_Tuple8()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Tuple<int, int, int, int, int, int, int, int>>();
        }
    }
}