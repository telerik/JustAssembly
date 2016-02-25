using System;
using Mono.Cecil;
using JustAssembly.Core.Extensions;

namespace JustAssembly.Core.DiffItems.Attributes
{
    class CustomAttributeDiffItem : BaseDiffItem
    {
        private readonly CustomAttribute attribute;

        public CustomAttributeDiffItem(CustomAttribute oldAttribute, CustomAttribute newAttribute)
            : base(oldAttribute == null ? DiffType.New : DiffType.Deleted)
        {
            if (oldAttribute != null && newAttribute != null)
            {
                throw new InvalidOperationException();
            }

            this.attribute = oldAttribute ?? newAttribute;
        }

        protected override string GetXmlInfoString()
        {
            throw new NotSupportedException();
        }

        internal override void ToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("CustomAttribute");
            writer.WriteAttributeString("Name", attribute.Constructor.GetSignature());
            writer.WriteAttributeString("DiffType", this.DiffType.ToString());
            writer.WriteEndElement();
        }

        public override bool IsBreakingChange
        {
            get { return this.DiffType == Core.DiffType.Deleted; }
        }
    }
}
