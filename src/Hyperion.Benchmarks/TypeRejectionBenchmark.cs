using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Hyperion.Internal;

namespace Hyperion.Benchmarks
{
    [Config(typeof(HyperionConfig))]
    public class TypeRejectionBenchmark
    {
        private Serializer _serializer;
        private Stream _dangerousStream;

        [GlobalSetup]
        public void Setup()
        {
            var di = new DirectoryInfo("C:\\Windows\\Windows32");
            var serializer = new Serializer(SerializerOptions.Default.WithDisallowUnsafeType(false));
            _dangerousStream = new MemoryStream();
            serializer.Serialize(di, _dangerousStream);

            _serializer = new Serializer();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _dangerousStream.Dispose();
        }
        
        [Benchmark]
        public void DeserializeDanger()
        {
            _dangerousStream.Position = 0;
            try
            {
                _serializer.Deserialize<DirectoryInfo>(_dangerousStream);
            }
            catch(EvilDeserializationException)
            {
                // no-op
            }
        }
        
    }
}