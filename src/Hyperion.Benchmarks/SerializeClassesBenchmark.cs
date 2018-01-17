#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeClassesBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using BenchmarkDotNet.Attributes;

namespace Hyperion.Benchmarks
{
    public class SerializeClassesBenchmark : HyperionBenchmark
    {
        #region init
        private CyclicClassA cyclic;
        private VirtualTestClass virtualObject;
        private LargeSealedClass sealedObject;
        private GenericClass<int, string, bool, DateTime, Guid> genericObject;

        protected override void Init()
        {
            Serializer = new Serializer(new SerializerOptions(preserveObjectReferences:true));
            var a = new CyclicClassA();
            var b = new CyclicClassB();
            a.B = b;
            b.A = a;
            cyclic = a;

            virtualObject = new VirtualTestClass
            {
                DateProp = DateTime.Now,
                GuidProp = Guid.NewGuid(),
                IntProp = 812342354,
                StringProp = new string('x', 30)
            };

            sealedObject = LargeSealedClass.MakeRandom();

            genericObject = new GenericClass<int, string, bool, DateTime, Guid>(123, "hello-world", true, DateTime.Now, Guid.NewGuid());
        }

        #endregion

        [Benchmark] public void Cyclic_References() => SerializeAndDeserialize(cyclic);
        [Benchmark] public void Virtual_Classes() => SerializeAndDeserialize(virtualObject);
        [Benchmark] public void Large_Sealed_Classes() => SerializeAndDeserialize(sealedObject);
        [Benchmark] public void Generic_Classes() => SerializeAndDeserialize(genericObject);
    }

    #region test data types

    public class CyclicClassA
    {
        public CyclicClassB B { get; set; }
    }

    public class CyclicClassB
    {
        public CyclicClassA A { get; set; }
    }

    public class VirtualTestClass
    {
        public virtual string StringProp { get; set; }

        public virtual int IntProp { get; set; }

        public virtual Guid GuidProp { get; set; }

        public virtual DateTime DateProp { get; set; }
    }

    public class GenericClass<T1, T2, T3, T4, T5>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }
        public T4 Item4 { get; }
        public T5 Item5 { get; }

        public GenericClass(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }
    }

    public sealed class LargeSealedClass
    {
        public LargeSealedClass() { }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressZip { get; set; }
        public double CreditScore { get; set; }
        public DateTime DOB { get; set; }
        public string EMail { get; set; }
        public string FirstName { get; set; }
        public string HomePhone { get; set; }
        public string LastName { get; set; }
        public TestEnum TestEnum { get; set; }
        public string MiddleName { get; set; }
        public string MobilePhone { get; set; }
        public bool RegisteredToVote { get; set; }
        public decimal Salary { get; set; }
        public int YearsOfService { get; set; }
        public string SkypeID { get; set; }
        public string YahooID { get; set; }
        public string GoogleID { get; set; }
        public string Notes { get; set; }
        public bool? IsSmoker { get; set; }
        public bool? IsLoving { get; set; }
        public bool? IsLoved { get; set; }
        public bool? IsDangerous { get; set; }
        public bool? IsEducated { get; set; }
        public DateTime? LastSmokingDate { get; set; }
        public decimal? DesiredSalary { get; set; }
        public double? ProbabilityOfSpaceFlight { get; set; }
        public int? CurrentFriendCount { get; set; }
        public int? DesiredFriendCount { get; set; }


        private static int counter;
        public static LargeSealedClass MakeRandom()
        {
            var rnd = counter++;

            var data = new LargeSealedClass
            {
                FirstName = NaturalTextGenerator.GenerateFirstName(),
                MiddleName = NaturalTextGenerator.GenerateFirstName(),
                LastName = NaturalTextGenerator.GenerateLastName(),
                DOB = DateTime.Now.AddYears(5),
                Salary = 55435345,
                YearsOfService = 25,
                CreditScore = 0.7562,
                RegisteredToVote = (DateTime.UtcNow.Ticks & 1) == 0,
                TestEnum = TestEnum.HatesAll,
                Address1 = NaturalTextGenerator.GenerateAddressLine(),
                Address2 = NaturalTextGenerator.GenerateAddressLine(),
                AddressCity = NaturalTextGenerator.GenerateCityName(),
                AddressState = "CA",
                AddressZip = "91606",
                HomePhone = (DateTime.UtcNow.Ticks & 1) == 0 ? "(555) 123-4567" : null,
                EMail = NaturalTextGenerator.GenerateEMail()
            };

            if (0 != (rnd & (1 << 32))) data.Notes = NaturalTextGenerator.Generate(45);
            if (0 != (rnd & (1 << 31))) data.SkypeID = NaturalTextGenerator.GenerateEMail();
            if (0 != (rnd & (1 << 30))) data.YahooID = NaturalTextGenerator.GenerateEMail();

            if (0 != (rnd & (1 << 29))) data.IsSmoker = 0 != (rnd & (1 << 17));
            if (0 != (rnd & (1 << 28))) data.IsLoving = 0 != (rnd & (1 << 16));
            if (0 != (rnd & (1 << 27))) data.IsLoved = 0 != (rnd & (1 << 15));
            if (0 != (rnd & (1 << 26))) data.IsDangerous = 0 != (rnd & (1 << 14));
            if (0 != (rnd & (1 << 25))) data.IsEducated = 0 != (rnd & (1 << 13));

            if (0 != (rnd & (1 << 24))) data.LastSmokingDate = DateTime.Now.AddYears(-10);


            if (0 != (rnd & (1 << 23))) data.DesiredSalary = rnd / 1000m;
            if (0 != (rnd & (1 << 22))) data.ProbabilityOfSpaceFlight = rnd / (double)int.MaxValue;

            if (0 != (rnd & (1 << 21)))
            {
                data.CurrentFriendCount = rnd % 123;
                data.DesiredFriendCount = rnd % 121000;
            }

            return data;
        }
    }

    public class NaturalTextGenerator
    {
        public static string GenerateEMail() => "foo@fooo.com";
        public static string Generate(int i) => "fskldjflksjfl ksj dlfkjsdfl ksdjklf jsdlkj" + DateTime.Now.Ticks;
        public static string GenerateAddressLine() => "fkjdskfjskfjs" + DateTime.Now.Ticks;
        public static string GenerateFirstName() => "fksjdfkjsdkfjksdfs" + DateTime.Now.Ticks;
        public static string GenerateCityName() => "fksdfkjsdkfjsdkfs";
        public static string GenerateLastName() => "kfjdskdfjskj";
    }

    #endregion
}