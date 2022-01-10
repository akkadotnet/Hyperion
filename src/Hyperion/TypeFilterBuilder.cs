using System;
using System.Collections.Generic;

namespace Hyperion
{
    /// <summary>
    /// Helper class to programatically create a <see cref="TypeFilter"/> using fluent builder pattern.
    /// </summary>
    public class TypeFilterBuilder
    {
        /// <summary>
        /// Create a new instance of <see cref="TypeFilterBuilder"/>
        /// </summary>
        /// <returns>a new instance of <see cref="TypeFilterBuilder"/> </returns>
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