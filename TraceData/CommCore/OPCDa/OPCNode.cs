using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Da;

namespace TraceData.CommCore.OPCDa
{
     public  class OPCNode
    {

        public OPCNode(string rootName)
        {
            Tag = new BrowseElement();
            ChildNode = new List<OPCNode>();
            Tag.ItemName = rootName;
        }

        public string RootName { get; set; }
        public BrowseElement Tag { get; set; }

        public List<OPCNode> ChildNode { get; set; }

    }

   
}
