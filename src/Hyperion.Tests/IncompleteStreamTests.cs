using System.IO;
using Xunit;

namespace Hyperion.Tests
{
    public class IncompleteStreamTests : TestBase
    {
        private const int IncompleteBytes = 4;

        public IncompleteStreamTests()
            : base(x => new IncompleteReadStream(x, IncompleteBytes))
        {
        }

        [Fact]
        public void ThrowsOnEOF()
        {
            double data = 4; //double has 8 bytes
            Serialize(data);
            Reset();

            // manifest requires 1 byte
            // incomplete returned bytes are then (IncompleteBytes)4 - 1 = 3 => EOF
            Assert.Throws<EndOfStreamException>(() => Deserialize<int>());
        }

        private class IncompleteReadStream : Stream
        {
            private readonly Stream _baseStream;
            private readonly int _maxReadBytes;

            private int _totalReadBytes;

            public IncompleteReadStream(Stream baseStream, int maxReadBytes)
            {
                _baseStream = baseStream;
                _maxReadBytes = maxReadBytes;
            }

            public override void Flush()
            {
                _baseStream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _baseStream.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var allBytes = _totalReadBytes + count;
                var bytesToRead = allBytes > _maxReadBytes
                    ? _maxReadBytes - _totalReadBytes
                    : count;

                var readBytes =  _baseStream.Read(buffer, offset, bytesToRead);
                _totalReadBytes += readBytes;
                return readBytes;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _baseStream.Write(buffer, offset, count);
            }

            public override bool CanRead => _baseStream.CanRead;
            public override bool CanSeek => _baseStream.CanSeek;
            public override bool CanWrite => _baseStream.CanWrite;
            public override long Length => _baseStream.Length;

            public override long Position
            {
                get => _baseStream.Position;
                set => _baseStream.Position = value;
            }
        }
    }
}