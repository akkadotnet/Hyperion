#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SurrogateTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Hyperion.Tests
{
    #region test support classes
    
    public interface IOriginal
    {
        ISurrogate ToSurrogate();
    }

    public interface ISurrogate
    {
        IOriginal FromSurrogate();
    }

    public class Foo : IOriginal
    {
        public string Bar { get; set; }

        public ISurrogate ToSurrogate() => new FooSurrogate
        {
            Bar = Bar
        };
    }

    public class FooSurrogate : ISurrogate
    {
        public string Bar { get; set; }

        public IOriginal FromSurrogate()
        {
            return Restore();
        }

        public static FooSurrogate FromFoo(Foo foo)
        {
            return new FooSurrogate
            {
                Bar = foo.Bar
            };
        }

        public Foo Restore()
        {
            return new Foo
            {
                Bar = Bar
            };
        }
    }

    public class SurrogatedKey : IOriginal
    {
        public readonly string Key;

        public SurrogatedKey(string key)
        {
            Key = key;
        }

        public ISurrogate ToSurrogate() => new KeySurrogate(Key);
        public override bool Equals(object obj) => obj is SurrogatedKey && Key == ((SurrogatedKey) obj).Key;
        public override int GetHashCode() => Key?.GetHashCode() ?? 0;
    }

    public class KeySurrogate : ISurrogate
    {
        public readonly string Key;

        public KeySurrogate(string key)
        {
            Key = key;
        }

        public IOriginal FromSurrogate() => new SurrogatedKey(Key);
    }

    #endregion

    public class SurrogateTests
    {
        [Fact]
        public void CanSerializeWithSurrogate()
        {
            var surrogateHasBeenInvoked = false;
            var surrogates = new[]
            {
                Surrogate.Create<Foo, FooSurrogate>(FooSurrogate.FromFoo, surrogate =>
                {
                    surrogateHasBeenInvoked = true;
                    return surrogate.Restore();
                })
            };
            var stream = new MemoryStream();
            var serializer = new Serializer(SerializerOptions.Default.WithSurrogates(surrogates));
            var foo = new Foo
            {
                Bar = "I will be replaced!"
            };

            serializer.Serialize(foo, stream);
            stream.Position = 0;
            var actual = serializer.Deserialize<Foo>(stream);
            Assert.Equal(foo.Bar, actual.Bar);
            Assert.True(surrogateHasBeenInvoked);
        }

        [Fact]
        public void CanSerializeWithInterfaceSurrogate()
        {
            var surrogateHasBeenInvoked = false;
            var surrogates = new[]
            {
                Surrogate.Create<IOriginal, ISurrogate>(from => from.ToSurrogate(), surrogate =>
                {
                    surrogateHasBeenInvoked = true;
                    return surrogate.FromSurrogate();
                })
            };
            var stream = new MemoryStream();
            var serializer = new Serializer(SerializerOptions.Default.WithSurrogates(surrogates));
            var foo = new Foo
            {
                Bar = "I will be replaced!"
            };

            serializer.Serialize(foo, stream);
            stream.Position = 0;
            var actual = serializer.Deserialize<Foo>(stream);
            Assert.Equal(foo.Bar, actual.Bar);
            Assert.True(surrogateHasBeenInvoked);
        }

        [Fact]
        public void CanSerializeWithSurrogateInCollection()
        {
            var invoked = new List<ISurrogate>();
            var surrogates = new[]
            {
                Surrogate.Create<IOriginal, ISurrogate>(from => from.ToSurrogate(), surrogate =>
                {
                    invoked.Add(surrogate);
                    return surrogate.FromSurrogate();
                })
            };
            var stream = new MemoryStream();
            var serializer = new Serializer(SerializerOptions.Default.WithSurrogates(surrogates));
            var key = new SurrogatedKey("test key");
            var foo = new Foo
            {
                Bar = "I will be replaced!"
            };

            var dictionary = new Dictionary<SurrogatedKey, Foo>
            {
                [key] = foo
            };

            serializer.Serialize(dictionary, stream);
            stream.Position = 0;

            var actual = serializer.Deserialize<Dictionary<SurrogatedKey, Foo>> (stream);

            Assert.Equal(key, actual.Keys.First());
            Assert.Equal(foo.Bar, actual[key].Bar);
            Assert.Equal(2, invoked.Count);
        }
    }
}
