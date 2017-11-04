using System;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Hyperion.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var benchmark = BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly());
            benchmark.RunAll();
        }
    }
}
