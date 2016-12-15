using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperion.PerfTest.Types;

namespace Hyperion.PerfTest.Tests
{
    class TypicalPersonTest : TestBase<TypicalPersonData>
    {
        protected override TypicalPersonData GetValue()
        {
            return TypicalPersonData.MakeRandom();
        }
    }
}
