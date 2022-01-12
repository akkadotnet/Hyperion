namespace Hyperion
{
    /// <summary>
    /// Provide a callback to allow a user defined Type filter during certain operations 
    /// </summary>
    public interface ITypeFilter
    {
        /// <summary>
        /// Determines if a fully qualified class name is allowed to be processed or not
        /// </summary>
        /// <param name="typeName">The fully qualified class name of the type to be filtered</param>
        /// <returns><c>true</c> if a type is allowed</returns>
        bool IsAllowed(string typeName);
    }
}