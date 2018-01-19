#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeStructsBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace Hyperion.Benchmarks
{
    public class SerializeStructsBenchmark : HyperionBenchmark
    {
        #region init
        private StandardStruct standardValue;
        private BlittableStruct blittableValue;
        private TestEnum testEnum;
        
        protected override void Init()
        {
            standardValue = new StandardStruct(1, "John", "Doe", isLoggedIn: false);
            blittableValue = new BlittableStruct(59, 92);
            testEnum = TestEnum.HatesAll;
        }

        #endregion

        [Benchmark] public void Enums() => SerializeAndDeserialize(testEnum);
        [Benchmark] public void Standard_Value_Types() => SerializeAndDeserialize(standardValue);
        [Benchmark] public void Blittable_Value_Types() => SerializeAndDeserialize(blittableValue);
    }

    #region test data types

    public struct StandardStruct
    {
        public readonly int Id;
        public readonly string FirstName;
        public readonly string LastName;
        public readonly bool IsLoggedIn;

        public StandardStruct(int id, string firstName, string lastName, bool isLoggedIn)
            : this()
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            IsLoggedIn = isLoggedIn;
        }
    }

    /// <summary>
    /// Blittable types have field layout matching their memory layout.
    /// See: https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BlittableStruct
    {
        public readonly int X;
        public readonly int Y;

        public BlittableStruct(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
    }

    public enum TestEnum
    {
        Married,
        Divorced,
        HatesAll
    }

    #endregion
}