using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    public class ConsistentArraySerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type)
        {
            return type.IsOneDimensionalPrimitiveArray();
        }

        public override bool CanDeserialize(Serializer serializer, Type type)
        {
            return CanSerialize(serializer,type);
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type, ConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var res = ConsistentArraySerializer.Instance;
            typeMapping.TryAdd(type, res);
            return res;
        }
    }
}
