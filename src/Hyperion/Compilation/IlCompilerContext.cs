#region copyright
// -----------------------------------------------------------------------
//  <copyright file="IlCompilerContext.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Hyperion.Compilation
{
#if NET45
    internal sealed class IlCompilerContext
    {
        private int _stackDepth;

        public IlCompilerContext(ILGenerator il, Type selfType)
        {
            Il = new IlEmitter(il);
            SelfType = selfType;
        }

        public IlEmitter Il { get; }


        public int StackDepth
        {
            get { return _stackDepth; }
            set
            {
                _stackDepth = value;
                if (value < 0)
                {
                    throw new NotSupportedException("Stack depth can not be less than 0");
                }
            }
        }

        public Type SelfType { get; }
    }

    internal sealed class IlEmitter
    {
        private readonly ILGenerator _il;
        private readonly StringBuilder _sb = new StringBuilder();
        public IlEmitter(ILGenerator il)
        {
            _il = il;
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        public void Emit(OpCode opcode)
        {
            _sb.AppendLine($"{opcode}");
            _il.Emit(opcode);
        }

        public void Emit(OpCode opcode,FieldInfo field)
        {
            _sb.AppendLine($"{opcode} field {field}");
            _il.Emit(opcode,field);
        }

        public void Emit(OpCode opcode, ConstructorInfo ctor)
        {
            _sb.AppendLine($"{opcode} ctor {ctor}");
            _il.Emit(opcode, ctor);
        }

        public void Emit(OpCode opcode, int value)
        {
            _sb.AppendLine($"{opcode}_{value}");
            _il.Emit(opcode, value);
        }

        public void Emit(OpCode opcode, Type type)
        {
            _sb.AppendLine($"{opcode} type {type.Name}");
            _il.Emit(opcode, type);
        }

        public void EmitCall(OpCode opcode, MethodInfo method, Type[] optionalTypes)
        {
            _sb.AppendLine($"{opcode} {method}");
            _il.EmitCall(opcode, method, optionalTypes);
        }
    }
#endif
}