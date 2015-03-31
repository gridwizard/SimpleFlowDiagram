using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFlowDiagramLib
{
    // https://gridwizard.wordpress.com/2015/03/31/simpleflowdiagramlib-simple-c-library-to-serialize-graph-to-xml-and-vice-versa/
    public class Node
    {
        protected IList<Node> _ParentNodes = new List<Node>();
        public IList<Node> ParentNodes
        {
            get { return _ParentNodes; }
            set { _ParentNodes = value; }
        }

        protected IList<Node> _ChildNodes = new List<Node>();
        public IList<Node> ChildNodes
        {
            get { return _ChildNodes; }
            set { _ChildNodes = value; }
        }

        public int xSlot { get; set; }  // Horizontal slot #
        public int ySlot { get; set; }  // Vertical slot #

        public double x { get; set; }
        public double y { get; set; }

        public static double DEFAULT_NODE_WIDTH = 150;
        public static double DEFAULT_NODE_HEIGHT = 50;
        protected double _Width = DEFAULT_NODE_WIDTH;
        protected double _Height = DEFAULT_NODE_HEIGHT;

        public double Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
            }
        }

        public double Height
        {
            get
            {
                return _Height;
            }
            set
            {
                _Height = value;
            }
        }

        public string NodeHeader { get; set; }
        public string NodeDetail { get; set; }
        public string NodeHyperLink { get; set; }

        public int Depth { get; set; }
    }
}
