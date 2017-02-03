#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DeserializeComplexObjectsBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using Hyperion.Tests.Performance.Types;
using NBench;

namespace Hyperion.Tests.Performance.Deserialization
{
    public class StructDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(LargeStruct.Create());
        }

        [PerfBenchmark(
            Description = "Benchmark struct deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 1000000)]
        public void Deserialize_Struct()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<LargeStruct>();
        }
    }

    public class ClassDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(TypicalPersonData.MakeRandom());
        }

        [PerfBenchmark(
            Description = "Benchmark class deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 180000)]
        public void Deserialize_Class()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<TypicalPersonData>();
        }
    }

    public class CyclicReferenceDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);

            var a = new CyclicA();
            var b = new CyclicB();
            a.B = b;
            b.A = a;

            InitStreamWith(a);
        }

        [PerfBenchmark(
            Description = "Benchmark cyclic reference deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test,
            Skip = "FIXME: Stack overflow exception")]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 4000000)]
        public void Deserialize_Cyclic()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<CyclicA>();
        }
    }
}