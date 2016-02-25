using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.Extensions
{
    static class CodeViewerExtensions
    {
        public static int GetLineFromMemberToken(this ICodeViewerResults self, uint typeToken, uint memberToken)
        {
            var lineToMemberMap = self.LineToMemberTokenMap.FirstOrDefault(
                t => (t.Item2 is IMemberTokenProvider) && (t.Item2 as IMemberTokenProvider).DeclaringTypeToken == typeToken && (t.Item2 as IMemberTokenProvider).MemberToken == memberToken);

            if (lineToMemberMap != null)
            {
                return lineToMemberMap.Item1;
            }

            return -1;
        }

        public static int GetLineFromTypeToken(this ICodeViewerResults self, uint typeToken)
        {
            var lineToMemberMap = self.LineToMemberTokenMap.FirstOrDefault(t => (t.Item2 is ITypeTokenProvider) && (t.Item2 as ITypeTokenProvider).TypeToken == typeToken);

            if (lineToMemberMap != null)
            {
                return lineToMemberMap.Item1;
            }

            return -1;
        }
    }
}
