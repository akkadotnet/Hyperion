#region copyright
// -----------------------------------------------------------------------
//  <copyright file="HyperionConfig.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;

namespace Hyperion.Benchmarks
{
    public class HyperionConfig : ManualConfig
    {
        public HyperionConfig()
        {
            Add(StatisticColumn.Mean, StatisticColumn.Min, StatisticColumn.Max, StatisticColumn.OperationsPerSecond);
            Add(MarkdownExporter.GitHub);
            Add(MemoryDiagnoser.Default);
        }
    }
}