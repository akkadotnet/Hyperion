using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ApprovalTests;
using ApprovalTests.Reporters;
using PublicApiGenerator;
using Xunit;

namespace Hyperion.API.Tests
{
#if(DEBUG)
    [UseReporter(typeof(DiffReporter), typeof(AllFailingTestsClipboardReporter))]
#else
    [UseReporter(typeof(DiffReporter))]
#endif
    public class CoreApiSpec
    {
        [Fact]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApi()
        {
            var publicApi = Filter(typeof(Serializer).Assembly.GeneratePublicApi());
            Approvals.Verify(publicApi);
        }

        static string Filter(string text)
        {
            return string.Join(Environment.NewLine, text.Split(new[]
                {
                    Environment.NewLine
                }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => 
                    !l.StartsWith("[assembly: ReleaseDateAttribute(")
                    && !l.StartsWith("[assembly: System.Security")
                    && !l.StartsWith("[assembly: System.Runtime.Versioning.TargetFramework("))
                .Where(l => !string.IsNullOrWhiteSpace(l))
            );
        }
    }
}
