#region copyright
// -----------------------------------------------------------------------
//  <copyright file="AttributeTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using Xunit;

namespace Hyperion.Tests
{
    public class AttributeTests : TestBase
    {
        #region test classes

        private abstract class Animal
        {
            public readonly string Name;

            [Ignore]
            public readonly string Secret;

            protected Animal(string name, string secret)
            {
                Name = name;
                Secret = secret;
            }
        }

        private class Dog : Animal
        {
            public readonly bool Barks;

            public Dog(string name, string secret, bool barks) : base(name, secret)
            {
                Barks = barks;
            }
        }

        #endregion

        [Fact]
        public void ShouldNotSerializeFieldsMarkedAsIgnore()
        {
            var expected = new Dog("Max", "p4ssw0rd", barks: true);
            Serialize(expected);
            Reset();
            var actual = Deserialize<Dog>();
           
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Barks, actual.Barks);
            Assert.Null(actual.Secret);
        }
    }
}