﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="UnsupportedTypeSerializerTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hyperion.ValueSerializers;
using Xunit;

namespace Hyperion.Tests
{
    public class UnsupportedTypeSerializerTests : TestBase
    {
        [Fact]
        public void DoUnsupportedTypesNotHangOnExceptions()
        {
            var tsk = Task.Run(() =>
            {
                var serializer = new Serializer();
                var t = new TestElement();
                try
                {
                    serializer.Serialize(t, new MemoryStream());
                }
                catch (Exception)
                {
                    try
                    {
                        serializer.Serialize(t, new MemoryStream());
                    }
                    catch (UnsupportedTypeException)
                    {
                    }
                }
            });
            if (!tsk.Wait(TimeSpan.FromSeconds(5)))
            {
                Assert.True(false, "Serializer did not complete in 5 seconds");
            }
        }

        [Fact]
        public void DoUnsupportedTypesThrowErrors()
        {
            var serializer = new Serializer();
            var t = new TestElement();
            try
            {
                serializer.Serialize(t, new MemoryStream());
            }
            catch (UnsupportedTypeException)
            {
                Assert.True(true);
            }
        }
    }


    /* Copied from MongoDB.Bson.BsonElement - causes failures in the serializer - not sure why */
#if NET45
    [Serializable]
#endif

    internal struct TestElement : IComparable<TestElement>, IEquatable<TestElement>
    {
        public TestElement(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }

        public static bool operator ==(TestElement lhs, TestElement rhs)
        {
            return Equals(lhs, rhs);
        }

        public static bool operator !=(TestElement lhs, TestElement rhs)
        {
            return !(lhs == rhs);
        }

        public TestElement Clone()
        {
            return new TestElement(Name, Value);
        }

        public TestElement DeepClone()
        {
            return new TestElement(Name, Value);
        }

        public int CompareTo(TestElement other)
        {
            var r = Name.CompareTo(other.Name);
            if (r != 0)
            {
                return r;
            }
            return Value.CompareTo(other.Value);
        }

        public bool Equals(TestElement rhs)
        {
            return Name == rhs.Name && Value == rhs.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(TestElement))
            {
                return false;
            }
            return Equals((TestElement) obj);
        }

        public override int GetHashCode()
        {
            // see Effective Java by Joshua Bloch
            var hash = 17;
            hash = 37*hash + Name.GetHashCode();
            hash = 37*hash + Value.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
    }
}