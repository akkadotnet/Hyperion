#region copyright
// -----------------------------------------------------------------------
//  <copyright file="InterfaceTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.IO;
using Xunit;

namespace Hyperion.Tests
{
   
    public class InterfaceTests
    {
        public class Bar
        {
            public IFoo Foo { get; set; }
        }

        public interface IFoo
        {
            int A { get; set; }
            string B { get; set; }
        }
        public class Foo : IFoo
        {
            public int A { get; set; }
            public string B { get; set; }
        }

        [Fact]
        public void Serializer_should_work_with_types_having_interface_fields()
        {
            var b = new Bar
            {
                Foo = new Foo()
                {
                    A = 123,
                    B = "hello"
                }
            };
            var stream = new MemoryStream();
            var serializer = new Serializer(new SerializerOptions());
            serializer.Serialize(b, stream);
            stream.Position = 0;
            var res = serializer.Deserialize(stream);
        }
    }
}
