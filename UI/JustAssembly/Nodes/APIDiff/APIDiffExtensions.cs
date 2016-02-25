using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustAssembly.Core;
using JustAssembly.Interfaces;

namespace JustAssembly.Nodes.APIDiff
{
    static class APIDiffExtensions
    {
        public static void Validate(this LoadAPIItemsContext context)
        {
            if (context != null && !context.IsEmpty)
            {
                throw new Exception("Invalid load context state.");
            }
        }

        public static APIDiffInfo GenerateAPIDiffInfo(this LoadAPIItemsContext context, IOldToNewTupleMap<MemberMetadataBase> metadataTuple)
        {
            return context == null ? null : new APIDiffInfo(context.GetDiffItem(metadataTuple));
        }

        public static LoadAPIItemsContext GenerateLoadAPIItemsContext(this APIDiffInfo apiDiffInfo)
        {
            return apiDiffInfo == null ? null : new LoadAPIItemsContext(apiDiffInfo);
        }

        public static DifferenceDecoration GetDifferenceDecoration(this IMetadataDiffItem diffItem, bool breakingChangesOnly)
        {
            if (diffItem == null || breakingChangesOnly && !diffItem.IsBreakingChange)
            {
                return DifferenceDecoration.NoDifferences;
            }

            switch (diffItem.DiffType)
            {
                case DiffType.Deleted:
                    return DifferenceDecoration.Deleted;
                case DiffType.Modified:
                    return DifferenceDecoration.Modified;
                case DiffType.New:
                    return DifferenceDecoration.Added;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
