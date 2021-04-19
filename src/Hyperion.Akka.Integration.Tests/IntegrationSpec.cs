using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Serialization;
using Xunit;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using Xunit.Abstractions;
using AkkaSerializer = Akka.Serialization.Serializer;

namespace Hyperion.Akka.Integration.Tests
{
    public class IntegrationSpec : TestKit
    {
        private static readonly Config Config = ConfigurationFactory.ParseString(@"
          akka {
            loglevel = WARNING
            stdout-loglevel = WARNING
            serialize-messages = on
            actor {
                serializers {
                    hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                }
                serialization-bindings {
                    ""System.Object"" = hyperion
                }
            }
          }
          # use random ports to avoid race conditions with binding contention
          akka.remote.dot-netty.tcp.port = 0");

        private readonly AkkaSerializer _serializer;

        public IntegrationSpec(ITestOutputHelper output)
            : base(Config, nameof(IntegrationSpec), output)
        {
            _serializer = Sys.Serialization.FindSerializerForType(typeof(object));
        }

        [Fact]
        public void Akka_should_load_Hyperion_correctly()
        {
            _serializer.Should().BeOfType<HyperionSerializer>();
        }

        [Fact]
        public void Akka_HyperionSerializer_should_serialize_properly()
        {
            var myObject = new MyPocoClass
            {
                Name = "John Doe",
                Count = 24
            };

            var serialized = _serializer.ToBinary(myObject);
            var deserialized = _serializer.FromBinary<MyPocoClass>(serialized);

            deserialized.Name.Should().Be("John Doe");
            deserialized.Count.Should().Be(24);
        }

        private class MyPocoClass
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }
}
