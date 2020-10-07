using System.IO;

namespace Hyperion.Tests
{
    public class PartialStreamTest : PrimitivesTest
    {
        public PartialStreamTest()
            : base(x => new OneBytePerReadStream(x))
        {
        }

        private class OneBytePerReadStream : Stream
        {
            private readonly Stream _baseStream;

            public OneBytePerReadStream(Stream baseStream)
            {
                _baseStream = baseStream;
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
                return _baseStream.Read(buffer, offset, count > 0 ? 1 : 0);
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