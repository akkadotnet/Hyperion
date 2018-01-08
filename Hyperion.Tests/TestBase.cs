#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TestBase.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.IO;
using Xunit;

namespace Hyperion.Tests
{
    public abstract class TestBase
    {
        protected Serializer Serializer;
        private readonly MemoryStream _stream;

        protected TestBase(SerializerOptions options)
        {
            Serializer = new Serializer(options);
            _stream = new MemoryStream();
        }

        protected TestBase()
        {
            Serializer = new Serializer();
            _stream = new MemoryStream();
        }

        protected void CustomInit(Serializer serializer)
        {
            Serializer = serializer;
        }


        public void Reset()
        {
            _stream.Position = 0;
        }

        public void Serialize(object o)
        {
            Serializer.Serialize(o, _stream);
        }

        public void Serialize(object o, SerializerSession session)
        {
            Serializer.Serialize(o, _stream, session);
        }

        public T Deserialize<T>()
        {
            return Serializer.Deserialize<T>(_stream);
        }

        public T Deserialize<T>(DeserializerSession session)
        {
            return Serializer.Deserialize<T>(_stream, session);
        }

        public void AssertMemoryStreamConsumed()
        {
            Assert.Equal(_stream.Length, _stream.Position);
        }
    }
}