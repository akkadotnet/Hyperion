using System;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Hyperion.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var benchmark = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly);

            if (args.Length == 0)
            {
                benchmark.RunAll();
            }
            else
            {
                benchmark.Run(args);
            }
        }
    }
}
