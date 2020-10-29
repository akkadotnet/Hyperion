#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TestBase.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using Xunit;

namespace Hyperion.Tests
{
    public abstract class TestBase
    {
        private Serializer _serializer;
        private readonly Stream _stream;

        protected TestBase()
            : this(x => x)
        {
        }

        protected TestBase(Func<Stream, Stream> streamFacade)
        {
            _serializer = new Serializer();
            _stream = streamFacade(new MemoryStream());
        }

        protected void CustomInit(Serializer serializer)
        {
            _serializer = serializer;
        }


        public void Reset()
        {
            _stream.Position = 0;
        }

        public void Serialize(object o)
        {
            _serializer.Serialize(o, _stream);
        }

        public T Deserialize<T>()
        {
            return _serializer.Deserialize<T>(_stream);
        }

        public void AssertMemoryStreamConsumed()
        {
            Assert.Equal(_stream.Length, _stream.Position);
        }
    }
}