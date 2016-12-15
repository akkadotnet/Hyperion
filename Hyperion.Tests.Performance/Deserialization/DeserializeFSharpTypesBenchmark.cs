using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Hyperion.FSharpTestTypes;
using Xunit.Abstractions;

namespace Hyperion.Tests.Performance.Deserialization
{
    public class DiscriminatedUnionDeserializationBenchmark : PerfTestBase
    {
#if !NBENCH
        public DiscriminatedUnionDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(DU2.NewC(DU1.NewB("test", 123)));
        }

        [NBenchFact]
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
#if !NBENCH
        public RecordDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            var record = new User(
                name: "John Doe",
                aref: FSharpOption<string>.Some("ok"),
                connections: "test");
            InitStreamWith(record);
        }

        [NBenchFact]
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
#if !NBENCH
        public FSharpMapDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(TestMap.createRecordWithMap);
        }

        [NBenchFact]
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
#if !NBENCH
        public FSharpListDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            var list = ListModule.OfArray(new[] { 123, 2342355, 456456467578, 234234, -234281 });
            InitStreamWith(list);
        }

        [NBenchFact(Skip = "FIXME: counter is always 0")]
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
#if !NBENCH
        public FSharpSetDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            var set = SetModule.OfArray(new[] { 123, 2342355, 456456467578, 234234, -234281 });
            InitStreamWith(set);
        }

        [NBenchFact(Skip = "FIXME: counter is always 0")]
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