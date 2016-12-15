using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace Wire.Tests.Performance.Deserialization
{
    public class ImmutableArrayDeserializationBenchmark : PerfTestBase
    {
#if !NBENCH
        public ImmutableArrayDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(ImmutableArray.Create(123, 56568, 3445, 568567, 234236, 5821, 2456, 71231));
        }

        [NBenchFact]
        [PerfBenchmark(
            Description = "Benchmark immutable array deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 240000)]
        public void Deserialize_ImmutableArray()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<ImmutableArray<int>>();
        }
    }

    public class ImmutableListDeserializationBenchmark : PerfTestBase
    {
#if !NBENCH
        public ImmutableListDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(ImmutableList.Create(123, 56568, 3445, 568567, 234236, 5821, 2456, 71231));
        }

        [NBenchFact]
        [PerfBenchmark(
            Description = "Benchmark immutable list deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 230000)]
        public void Deserialize_ImmutableList()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<ImmutableList<int>>();
        }
    }

    public class ImmutableHashSetDeserializationBenchmark : PerfTestBase
    {
#if !NBENCH
        public ImmutableHashSetDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(ImmutableHashSet.Create(123, 56568, 3445, 568567, 234236, 5821, 2456, 71231));
        }

        [NBenchFact]
        [PerfBenchmark(
            Description = "Benchmark immutable hash set deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 120000)]
        public void Deserialize_ImmutableHashSet()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<ImmutableHashSet<int>>();
        }
    }

    public class ImmutableSortedSetDeserializationBenchmark : PerfTestBase
    {
#if !NBENCH
        public ImmutableSortedSetDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(ImmutableSortedSet.Create(123, 56568, 3445, 568567, 234236, 5821, 2456, 71231));
        }

        [NBenchFact]
        [PerfBenchmark(
            Description = "Benchmark immutable sorted set deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 180000)]
        public void Deserialize_ImmutableSortedSet()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<ImmutableSortedSet<int>>();
        }
    }

    public class ImmutableDictionaryDeserializationBenchmark : PerfTestBase
    {
#if !NBENCH
        public ImmutableDictionaryDeserializationBenchmark(ITestOutputHelper output) : base(output) { }
#endif
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(ImmutableDictionary.CreateRange(new Dictionary<string, int>
            {
                ["key1"] = 123,
                ["key2"] = 1234,
                ["key3"] = 12345,
                ["key4"] = 123456,
                ["key5"] = 1234567,
                ["key6"] = 12345678,
                ["key7"] = 123456789,
            }));
        }

        [NBenchFact]
        [PerfBenchmark(
            Description = "Benchmark immutable dictionary deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 75000)]
        public void Deserialize_ImmutableDictionary()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<ImmutableDictionary<string, int>>();
        }
    }
}