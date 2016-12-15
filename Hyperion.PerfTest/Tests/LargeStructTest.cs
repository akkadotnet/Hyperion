using Hyperion.PerfTest.Types;

namespace Hyperion.PerfTest.Tests
{
    internal class LargeStructTest : TestBase<LargeStruct>
    {
        protected override LargeStruct GetValue()
        {
            return LargeStruct.Create();
        }
    }
}