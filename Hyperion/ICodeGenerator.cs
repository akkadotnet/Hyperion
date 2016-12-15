using Hyperion.ValueSerializers;

namespace Hyperion
{
    public interface ICodeGenerator
    {
        void BuildSerializer(Serializer serializer, ObjectSerializer objectSerializer);
    }
}