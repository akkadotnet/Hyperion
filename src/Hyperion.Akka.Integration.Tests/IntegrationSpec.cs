using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Serialization;
using Xunit;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using Hyperion.Internal;
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

        [Fact]
        public void Bugfix263_Akka_HyperionSerializer_should_serialize_ActorPath_list()
        {
            var actor = Sys.ActorOf(Props.Create<MyActor>());
            var container = new ContainerClass(new List<ActorPath>{ actor.Path, actor.Path });
            var serialized = _serializer.ToBinary(container);
            var deserialized = _serializer.FromBinary<ContainerClass>(serialized);
            deserialized.Destinations.Count.Should().Be(2);
            deserialized.Destinations[0].Should().Be(deserialized.Destinations[1]);
        }

        [Fact]
        public async Task CanDeserializeANaughtyTypeWhenAllowed()
        {
            var config = ConfigurationFactory.ParseString(@"
akka {
    serialize-messages = on
    actor {
        serializers {
            hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
        }
        serialization-bindings {
            ""System.Object"" = hyperion
        }
        serialization-settings.hyperion.disallow-unsafe-type = false
    }
}");
            var system = ActorSystem.Create("unsafeSystem", config);
            
            try
            {
                var serializer = system.Serialization.FindSerializerForType(typeof(DirectoryInfo));
                var di = new DirectoryInfo(@"c:\");

                var serialized = serializer.ToBinary(di);
                var deserialized = serializer.FromBinary<DirectoryInfo>(serialized);
            }
            finally
            {
                await system.Terminate();
            }
        }
        
        [Fact]
        public async Task CantDeserializeANaughtyTypeByDefault()
        {
            var config = ConfigurationFactory.ParseString(@"
akka {
    serialize-messages = on
    actor {
        serializers {
            hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
        }
        serialization-bindings {
            ""System.Object"" = hyperion
        }
        serialization-settings.hyperion.disallow-unsafe-type = true # this is the default value
    }
}");
            var system = ActorSystem.Create("unsafeSystem", config);
            
            try
            {
                var deserializer = system.Serialization.FindSerializerForType(typeof(DirectoryInfo));
                var di = new DirectoryInfo(@"c:\");

                byte[] serialized;
                using (var stream = new MemoryStream())
                {
                    var serializer = new Serializer(SerializerOptions.Default.WithDisallowUnsafeType(false));
                    serializer.Serialize(di, stream);
                    stream.Position = 0;
                    serialized = stream.ToArray();
                }
                
                var ex = Assert.Throws<SerializationException>(() => deserializer.FromBinary<DirectoryInfo>(serialized));
                ex.InnerException.Should().BeOfType<EvilDeserializationException>();
            }
            finally
            {
                await system.Terminate();
            }
        }
        
        private class MyActor: ReceiveActor
        {
            
        }
        
        private class ContainerClass
        {
            public ContainerClass(List<ActorPath> destinations)
            {
                Destinations = destinations;
            }

            public List<ActorPath> Destinations { get; }
        }
        
        private class MyPocoClass
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }
}
