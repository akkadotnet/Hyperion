using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Wire.Tests.Performance.Types;
using Xunit.Abstractions;

namespace Wire.Tests.Performance.Deserialization
{
    public class StructDeserializationBenchmark : PerfTestBase
    {
#if !NBENCH
        public StructDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(LargeStruct.Create());
        }

        [NBenchFact]
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
#if !NBENCH
        public ClassDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(TypicalPersonData.MakeRandom());
        }

        [NBenchFact]
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
#if !NBENCH
        public CyclicReferenceDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);

            var a = new CyclicA();
            var b = new CyclicB();
            a.B = b;
            b.A = a;

            InitStreamWith(a);
        }

        [NBenchFact(Skip="FIXME: Stack overflow exception")]
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