using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperion.PerfTest.Types;

namespace Hyperion.PerfTest.Tests
{
    class TypicalPersonArrayTest : TestBase<TypicalPersonData[]>
    {
        protected override TypicalPersonData[] GetValue()
        {
            var l = new List<TypicalPersonData>();
            for (int i = 0; i < 100; i++)
            {
                l.Add(TypicalPersonData.MakeRandom());
            }
            return l.ToArray();
        }
    }
}
