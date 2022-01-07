namespace Hyperion
{
    public interface ITypeFilter
    {
        bool IsAllowed(string typeName);
    }
}