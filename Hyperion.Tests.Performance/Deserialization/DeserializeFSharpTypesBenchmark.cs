#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DeserializeFSharpTypesBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using Hyperion.FSharpTestTypes;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using NBench;

namespace Hyperion.Tests.Performance.Deserialization
{
    public class DiscriminatedUnionDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(DU2.NewC(DU1.NewB("test", 123)));
        }

        [PerfBenchmark(
            Description = "Benchmark dyscriminated union deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 500000)]
        public void Deserialize_DiscriminatedUnion()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<DU2>();
        }
    }

    public class RecordDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            var record = new User(
                name: "John Doe",
                aref: FSharpOption<string>.Some("ok"),
                connections: "test");
            InitStreamWith(record);
        }

        [PerfBenchmark(
            Description = "Benchmark record deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 350000)]
        public void Deserialize_Record()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<User>();
        }
    }

    public class FSharpMapDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(TestMap.createRecordWithMap);
        }

        [PerfBenchmark(
            Description = "Benchmark F# map deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 120000)]
        public void Deserialize_FSharpMap()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<TestMap.RecordWithMap>();
        }
    }

    public class FSharpListDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            var list = ListModule.OfArray(new[] { 123, 2342355, 456456467578, 234234, -234281 });
            InitStreamWith(list);
        }

        [PerfBenchmark(
            Description = "Benchmark F# list deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test, 
            Skip = "FIXME: counter is always 0")]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 4000000)]
        public void Deserialize_FSharpList()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<FSharpList<int>>();
        }
    }

    public class FSharpSetDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            var set = SetModule.OfArray(new[] { 123, 2342355, 456456467578, 234234, -234281 });
            InitStreamWith(set);
        }

        [PerfBenchmark(
            Description = "Benchmark F# list deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test,
            Skip="FIXME: counter is always 0")]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 4000000)]
        public void Deserialize_FSharpSet()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<FSharpSet<int>>();
        }
    }
}
