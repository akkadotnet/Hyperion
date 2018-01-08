#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ExpressionTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Xunit;

namespace Hyperion.Tests
{
    public class ExpressionTests
    {
        public struct Dummy
        {
            public string TestField;
            public int TestProperty { get; set; }
            public string Fact(string input) => input;

            public Dummy(string testField) : this()
            {
                TestField = testField;
            }
        }

        public class DummyException : Exception
        {
#if SERIALIZATION
            protected DummyException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
#endif
        }

        [Fact]
        public void Serializer_should_work_with_FieldInfo()
        {
            var fieldInfo = typeof(Dummy).GetField("TestField");
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(fieldInfo, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<FieldInfo>(stream);
                Assert.Equal(fieldInfo, deserialized);
            }
        }

        [Fact]
        public void Serializer_should_work_with_PropertyInfo()
        {
            var propertyInfo = typeof(Dummy).GetProperty("TestProperty");
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(propertyInfo, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<PropertyInfo>(stream);
                Assert.Equal(propertyInfo, deserialized);
            }
        }

        [Fact]
        public void Serializer_should_work_with_MethodInfo()
        {
            var methodInfo = typeof(Dummy).GetMethod("Fact");
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(methodInfo, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<MethodInfo>(stream);
                Assert.Equal(methodInfo, deserialized);
            }
        }

        [Fact]
        public void Serializer_should_work_with_SymbolDocumentInfo()
        {
            var info = Expression.SymbolDocument("testFile");
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(info, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<SymbolDocumentInfo>(stream);
                Assert.Equal(info.FileName, deserialized.FileName);
            }
        }

        [Fact]
        public void Serializer_should_work_with_ConstructorInfo()
        {
            var constructorInfo = typeof(Dummy).GetConstructor(new[] { typeof(string) });
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(constructorInfo, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<ConstructorInfo>(stream);
                Assert.Equal(constructorInfo, deserialized);
            }
        }

        [Fact]
        public void Serializer_should_work_with_ConstantExpression()
        {
            var expr = Expression.Constant(12);
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<ConstantExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Value, deserialized.Value);
                Assert.Equal(expr.Type, deserialized.Type);
            }
        }

        [Fact]
        public void Serializer_should_work_with_UnaryExpression()
        {
            var expr = Expression.Decrement(Expression.Constant(1));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<UnaryExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Method, deserialized.Method);
                Assert.Equal(expr.Operand.ConstantValue(), deserialized.Operand.ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_BinaryExpression()
        {
            var expr = Expression.Add(Expression.Constant(1), Expression.Constant(2));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<BinaryExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Method, deserialized.Method);
            }
        }

        [Fact]
        public void Serializer_should_work_with_IndexExpression()
        {
            var value = new[] {1, 2, 3};
            var arrayExpr = Expression.Constant(value);
            var expr = Expression.ArrayAccess(arrayExpr, Expression.Constant(1));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<IndexExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Indexer, deserialized.Indexer);
                Assert.Equal(1, deserialized.Arguments.Count);
                Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
                var actual = (int[])deserialized.Object.ConstantValue();
                Assert.Equal(value[0], actual[0]);
                Assert.Equal(value[1], actual[1]);
                Assert.Equal(value[2], actual[2]);
            }
        }

        [Fact]
        public void Serializer_should_work_with_MemberAssignment()
        {
            var property = typeof(Dummy).GetProperty("TestProperty");
            var expr = Expression.Bind(property.SetMethod, Expression.Constant(9));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<MemberAssignment>(stream);
                Assert.Equal(expr.BindingType, deserialized.BindingType);
                Assert.Equal(expr.Member, deserialized.Member);
                Assert.Equal(expr.Expression.ConstantValue(), deserialized.Expression.ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_ConditionalExpression()
        {
            var expr = Expression.Condition(Expression.Constant(true), Expression.Constant(1), Expression.Constant(2));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<ConditionalExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Test.ConstantValue(), deserialized.Test.ConstantValue());
                Assert.Equal(expr.IfTrue.ConstantValue(), deserialized.IfTrue.ConstantValue());
                Assert.Equal(expr.IfFalse.ConstantValue(), deserialized.IfFalse.ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_BlockExpression()
        {
            var expr = Expression.Block(new[] { Expression.Constant(1), Expression.Constant(2), Expression.Constant(3) });
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<BlockExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Expressions.Count, deserialized.Expressions.Count);
                Assert.Equal(expr.Expressions[0].ConstantValue(), deserialized.Expressions[0].ConstantValue());
                Assert.Equal(expr.Expressions[1].ConstantValue(), deserialized.Expressions[1].ConstantValue());
                Assert.Equal(expr.Result.ConstantValue(), deserialized.Result.ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_LabelTarget()
        {
            var label = Expression.Label(typeof(int), "testLabel");
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(label, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<LabelTarget>(stream);
                Assert.Equal(label.Name, deserialized.Name);
                Assert.Equal(label.Type, deserialized.Type);
            }
        }

        [Fact]
        public void Serializer_should_work_with_LabelExpression()
        {
            var label = Expression.Label(typeof(int), "testLabel");
            var expr = Expression.Label(label, Expression.Constant(2));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<LabelExpression>(stream);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Target.Name, deserialized.Target.Name);
                Assert.Equal(expr.DefaultValue.ConstantValue(), deserialized.DefaultValue.ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_MethodCallExpression()
        {
            var methodInfo = typeof(Dummy).GetMethod("Fact");
            var expr = Expression.Call(Expression.Constant(new Dummy()), methodInfo, Expression.Constant("test string"));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<MethodCallExpression>(stream);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Method, deserialized.Method);
                Assert.Equal(expr.Object.ConstantValue(), deserialized.Object.ConstantValue());
                Assert.Equal(1, deserialized.Arguments.Count);
                Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_DefaultExpression()
        {
            var expr = Expression.Default(typeof(int));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<DefaultExpression>(stream);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
            }
        }

        [Fact]
        public void Serializer_should_work_with_ParameterExpression()
        {
            var expr = Expression.Parameter(typeof(int), "p1");
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<ParameterExpression>(stream);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Name, deserialized.Name);
            }
        }

        [Fact]
        public void Serializer_should_work_with_TargetInvocationException()
        {
            var exc = new TargetInvocationException(null);
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(exc, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<TargetInvocationException>(stream);
                Assert.Equal(exc.Message, deserialized.Message);
                Assert.Equal(exc.StackTrace, deserialized.StackTrace);
            }
        }

        [Fact]
        public void Serializer_should_work_with_ObjectDisposedException()
        {
            var exc = new ObjectDisposedException("Object is already disposed", new ArgumentException("One level deeper"));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(exc, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<ObjectDisposedException>(stream);
                Assert.Equal(exc.Message, deserialized.Message);
                Assert.Equal(exc.StackTrace, deserialized.StackTrace);
                Assert.Equal(exc.InnerException.GetType(), deserialized.InnerException.GetType());
                Assert.Equal(exc.InnerException.Message, deserialized.InnerException.Message);
                Assert.Equal(exc.InnerException.StackTrace, deserialized.InnerException.StackTrace);
            }
        }

        [Fact]
        public void Serializer_should_work_with_CatchBlock()
        {
            var expr = Expression.Catch(typeof(DummyException), Expression.Constant(2));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<CatchBlock>(stream);
                Assert.Equal(expr.Test, deserialized.Test);
                Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_GotoExpression()
        {
            var label = Expression.Label(typeof(void), "testLabel");
            var expr = Expression.Continue(label);
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<GotoExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Kind, deserialized.Kind);
                Assert.Equal(expr.Target.Name, deserialized.Target.Name);
            }
        }

        [Fact]
        public void Serializer_should_work_with_NewExpression()
        {
            var ctor = typeof(Dummy).GetConstructor(new[] { typeof(string) });
            var expr = Expression.New(ctor, Expression.Constant("test param"));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<NewExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Constructor, deserialized.Constructor);
                Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
                Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_DebugInfoExpression()
        {
            var info = Expression.SymbolDocument("testFile");
            var expr = Expression.DebugInfo(info, 1, 2, 3, 4);
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<DebugInfoExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Document.FileName, deserialized.Document.FileName);
                Assert.Equal(expr.EndColumn, deserialized.EndColumn);
                Assert.Equal(expr.StartColumn, deserialized.StartColumn);
                Assert.Equal(expr.EndLine, deserialized.EndLine);
                Assert.Equal(expr.StartLine, deserialized.StartLine);
            }
        }

        [Fact]
        public void Serializer_should_work_with_LambdaExpression()
        {
            var methodInfo = typeof(Dummy).GetMethod("Fact");
            var param = Expression.Parameter(typeof (Dummy), "dummy");
            var expr = Expression.Lambda(Expression.Call(param, methodInfo, Expression.Constant("s")), param);
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<LambdaExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Name, deserialized.Name);
                Assert.Equal(expr.TailCall, deserialized.TailCall);
                Assert.Equal(expr.ReturnType, deserialized.ReturnType);
                Assert.Equal(expr.Parameters.Count, deserialized.Parameters.Count);
                Assert.Equal(expr.Parameters[0].Name, deserialized.Parameters[0].Name);
            }
        }

        [Fact]
        public void Serializer_should_work_with_lambda_expression_having_generic_methods() {
            Expression<Func<Dummy, bool>> expr = dummy => dummy.TestField.Contains('s');
            var serializer = new Serializer(new SerializerOptions(preserveObjectReferences: true));
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(expr, ms);
                ms.Seek(0, SeekOrigin.Begin);
                var deserialized = serializer.Deserialize<Expression<Func<Dummy, bool>>>(ms);
                Assert.NotNull(((MethodCallExpression)deserialized.Body).Method);
                Assert.True(deserialized.Compile()(new Dummy("sausages")));
                Assert.False(deserialized.Compile()(new Dummy("field")));
            }
        }

        [Fact]
        public void Serializer_should_work_with_InvocationExpression()
        {
            var methodInfo = typeof(Dummy).GetMethod("Fact");
            var param = Expression.Parameter(typeof(Dummy), "dummy");
            var lambda = Expression.Lambda(Expression.Call(param, methodInfo, Expression.Constant("s")), param);
            var expr = Expression.Invoke(lambda, Expression.Constant(new Dummy()));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<InvocationExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
                Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_ElementInit()
        {
            var listAddMethod = typeof (List<int>).GetMethod("Add");
            var expr = Expression.ElementInit(listAddMethod, Expression.Constant(1));
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<ElementInit>(stream);
                Assert.Equal(expr.AddMethod, deserialized.AddMethod);
                Assert.Equal(1, deserialized.Arguments.Count);
                Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
            }
        }

        [Fact]
        public void Serializer_should_work_with_LoopExpression()
        {
            var breakLabel = Expression.Label(typeof (void), "break");
            var continueLabel = Expression.Label(typeof(void), "cont");
            var expr = Expression.Loop(Expression.Constant(2), breakLabel, continueLabel);
            var serializer = new Hyperion.Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(expr, stream);
                stream.Position = 0;
                var deserialized = serializer.Deserialize<LoopExpression>(stream);
                Assert.Equal(expr.NodeType, deserialized.NodeType);
                Assert.Equal(expr.Type, deserialized.Type);
                Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());
                Assert.Equal(expr.BreakLabel.Name, deserialized.BreakLabel.Name);
                Assert.Equal(expr.ContinueLabel.Name, deserialized.ContinueLabel.Name);
            }
        }
    }

    internal static class ExpressionExtensions
    {
        public static object ConstantValue(this Expression expr) => ((ConstantExpression)expr).Value;
    }
}