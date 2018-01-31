#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Compiler.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hyperion.Compilation
{
    internal sealed class Compiler<TDel> : ICompiler<TDel>
    {
        private readonly List<Expression> _content = new List<Expression>();
        private readonly List<Expression> _expressions = new List<Expression>();
        private readonly List<ParameterExpression> _parameters = new List<ParameterExpression>();
        private readonly List<ParameterExpression> _variables = new List<ParameterExpression>();

        public int NewObject(Type type)
        {
            var exp = ExpressionEx.GetNewExpression(type);
            _expressions.Add(exp);
            return _expressions.Count - 1;
        }

        public int Parameter<T>(string name)
        {
            var exp = Expression.Parameter(typeof(T), name);
            _parameters.Add(exp);
            _expressions.Add(exp);
            return _expressions.Count - 1;
        }

        public int Variable<T>(string name)
        {
            var exp = Expression.Variable(typeof(T), name);
            _variables.Add(exp);
            _expressions.Add(exp);
            return _expressions.Count - 1;
        }

        public int Variable(string name,Type type)
        {
            var exp = Expression.Variable(type, name);
            _variables.Add(exp);
            _expressions.Add(exp);
            return _expressions.Count - 1;
        }

        public int GetVariable<T>(string name)
        {
            var t = typeof(T);
            for (int i = 0; i < _expressions.Count; i++)
            {
                if (_expressions[i] is ParameterExpression parameter && parameter.Name == name && parameter.Type == t)
                {
                    return i;
                }
            }

            throw new Exception("Variable not found.");
        }

        public int Constant(object value)
        {
            var constant = value.ToConstant();
            _expressions.Add(constant);
            return _expressions.Count - 1;
        }

        public int CastOrUnbox(int value, Type type)
        {
            var tempQualifier = _expressions[value];
            var cast = type.GetTypeInfo().IsValueType
                // ReSharper disable once AssignNullToNotNullAttribute
                ? Expression.Unbox(tempQualifier, type)
                // ReSharper disable once AssignNullToNotNullAttribute
                : Expression.Convert(tempQualifier, type);
            var exp = (Expression) cast;
            _expressions.Add(exp);
            return _expressions.Count - 1;
        }

        public void EmitCall(MethodInfo method, int target, params int[] arguments)
        {
            var targetExp = _expressions[target];
            var argumentsExp = ArgsToExpr(arguments);
            var call = Expression.Call(targetExp, method, argumentsExp);
            _content.Add(call);
        }

        public void EmitStaticCall(MethodInfo method, params int[] arguments)
        {
            var argumentsExp = ArgsToExpr(arguments);
            var call = Expression.Call(null, method, argumentsExp);
            _content.Add(call);
        }

        public int Call(MethodInfo method, int target, params int[] arguments)
        {
            var targetExp = _expressions[target];
            var argumentsExp = ArgsToExpr(arguments);
            var call = Expression.Call(targetExp, method, argumentsExp);
            _expressions.Add(call);
            return _expressions.Count - 1;
        }

        public int StaticCall(MethodInfo method, params int[] arguments)
        {
            var argumentsExp = ArgsToExpr(arguments);
            var call = Expression.Call(null, method, argumentsExp);
            _expressions.Add(call);
            return _expressions.Count - 1;
        }

        public int ReadField(FieldInfo field, int target)
        {
            var targetExp = _expressions[target];
            var accessExp = Expression.Field(targetExp, field);
            _expressions.Add(accessExp);
            return _expressions.Count - 1;
        }

        public int WriteField(FieldInfo field, int typedTarget, int target, int value)
        {
            if (field.IsInitOnly)
            {
                //TODO: field is readonly, can we set it via IL or only via reflection
                var method = typeof(FieldInfo).GetTypeInfo()
                    .GetMethod(nameof(FieldInfo.SetValue), new[] {typeof(object), typeof(object)});
                var fld = Constant(field);
                var valueToObject = Convert<object>(value);
                return Call(method, fld, target, valueToObject); 
            }
            var targetExp = _expressions[typedTarget];
            var valueExp = _expressions[value];
            var accessExp = Expression.Field(targetExp, field);
            var writeExp = Expression.Assign(accessExp, valueExp);
            _expressions.Add(writeExp);
            return _expressions.Count - 1;
        }

        public Expression ToBlock()
        {
            if (_content.Count == 0)
            {
                _content.Add(Expression.Empty());
            }

            return Expression.Block(_variables.ToArray(), _content);
        }

        public TDel Compile()
        {
            var body = ToBlock();
            var parameters = _parameters.ToArray();
            var res = Expression.Lambda<TDel>(body, parameters).Compile();
            return res;
        }

        public int Convert<T>(int value)
        {
            var valueExp = _expressions[value];
            var con = (Expression) Expression.Convert(valueExp, typeof(T));
            _expressions.Add(con);
            return _expressions.Count - 1;
        }

        public int WriteVar(int variable, int value)
        {
            var varExp = _expressions[variable];
            var valueExp = _expressions[value];
            var assign = Expression.Assign(varExp, valueExp);
            _expressions.Add(assign);
            return _expressions.Count - 1;
        }

        public void Emit(int value)
        {
            var exp = _expressions[value];
            _content.Add(exp);
        }

        public int Convert(int value, Type type)
        {
            var valueExp = _expressions[value];
            var conv = (Expression) Expression.Convert(valueExp, type);
            _expressions.Add(conv);
            return _expressions.Count - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Expression[] ArgsToExpr(int[] arguments)
        {
            var result = new Expression[arguments.Length];
            var i = 0;
            foreach (var n in arguments)
            {
                result[i] = _expressions[n];
                i++;
            }
            return result;
        }
    }
}