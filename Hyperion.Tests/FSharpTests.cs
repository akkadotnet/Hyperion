#region copyright
// -----------------------------------------------------------------------
//  <copyright file="FSharpTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

#if AKKA
using Akka.Actor;
#endif
using System.Threading;
using FSharp.Quotations.Evaluator;
using Hyperion.FSharpTestTypes;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Quotations;
using Xunit;

namespace Hyperion.Tests
{
    public class FSharpTests : TestBase
    {

        [Fact]
        public void CanSerializeFSharpMap()
        {
            var expected = TestMap.createRecordWithMap;
            Serialize(expected);
            Reset();
            var actual = Deserialize<object>();
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void CanSerializeFSharpList()
        {
            var expected = ListModule.OfArray(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
            Serialize(expected);
            Reset();
            var actual = Deserialize<object>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeSimpleDU()
        {
            var expected = DU1.NewA(1);
            Serialize(expected);
            Reset();
            var actual = Deserialize<object>();
            Assert.Equal(expected,actual);
        }

        [Fact]
        public void CanSerializeStructBasedDU()
        {
            var expected = SDU1.NewB("hello", 1);
            Serialize(expected);
            Reset();
            var actual = Deserialize<object>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeNestedDU()
        {
            var expected = DU2.NewC(DU1.NewA(1));
            Serialize(expected);
            Reset();
            var actual = Deserialize<object>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeOptionalDU()
        {
            var expected = DU2.NewE(FSharpOption<DU1>.Some(DU1.NewA(1)));
            Serialize(expected);
            Reset();
            var actual = Deserialize<object>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeOption()
        {
            var expected = FSharpOption<string>.Some("hello");
            Serialize(expected);
            Reset();
            var actual = Deserialize<object>();
            Assert.Equal(expected, actual);
        }

#if AKKA
        public class FooActor : UntypedActor
        {
            protected override void OnReceive(object message)
            {                
            }
        }
#endif

        [Fact]
        public void CanSerializeRecord()
        {
                var expected = new TestRecord("foo", new FSharpOption<string>(null), "hello");
                Serialize(expected);
                Reset();
                var actual = Deserialize<TestRecord>();
               // Assert.Equal(expected, actual);
                Assert.Equal(expected.aref, actual.aref);
                Assert.Equal(expected.name, actual.name);
                Assert.Equal(expected.connections, actual.connections);

        }

        [Fact]
        public void CanSerializeStructRecord()
        {
            var expected = new TestStructRecord("foo", new FSharpOption<string>(null), "hello");
            Serialize(expected);
            Reset();
            var actual = Deserialize<TestStructRecord>();
            // Assert.Equal(expected, actual);
            Assert.Equal(expected.aref, actual.aref);
            Assert.Equal(expected.name, actual.name);
            Assert.Equal(expected.connections, actual.connections);

        }

        //FIXME: make F# quotations and Async serializable
        [Fact(Skip = "FIXME: problem with System.Core version=4.0")]
        public void CanSerializeQuotation()
        {
            var expected = TestQuotations.Quotation;
            Serialize(expected);
            Reset();
            var actual = Deserialize<FSharpExpr<FSharpFunc<int, FSharpAsync<int>>>>();

            var expectedFn = QuotationEvaluator.Compile(expected);
            var actualFn = QuotationEvaluator.Compile(actual);

            var a = FSharpAsync.RunSynchronously(expectedFn.Invoke(5), FSharpOption<int>.None, FSharpOption<CancellationToken>.None);
            var b = FSharpAsync.RunSynchronously(actualFn.Invoke(5), FSharpOption<int>.None, FSharpOption<CancellationToken>.None);
            
            Assert.Equal(a, b);
        }
    }
}
