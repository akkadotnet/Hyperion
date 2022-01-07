using System;
using System.Collections.Generic;

namespace Hyperion
{
    public class TypeFilterBuilder
    {
        public static TypeFilterBuilder Create() => new TypeFilterBuilder();

        private readonly List<Type> _types = new List<Type>();
        
        private TypeFilterBuilder()
        { }

        public TypeFilterBuilder Include<T>()
        {
            return Include(typeof(T));
        }

        public TypeFilterBuilder Include(Type type)
        {
            _types.Add(type);
            return this;
        }
        
        public TypeFilter Build()
            => new TypeFilter(_types);
    }
}