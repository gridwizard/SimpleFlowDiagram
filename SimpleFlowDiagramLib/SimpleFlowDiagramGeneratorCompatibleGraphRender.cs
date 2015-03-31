using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LearningMahesh.DynamicIOStream;
using LearningMahesh.DynamicIOStream.Xml;

namespace SimpleFlowDiagramLib
{
    /*
     * This class renders "Nodes" to XML format understandable by "SimpleFlowDiagramGenerator.exe", which converts the XML to actual flowchart in HTML5.
     */
    public class SimpleFlowDiagramGeneratorCompatibleGraphRender
    {
        public IList<Node> ReadGraphXml(System.Xml.XmlReader XmlRdr)
        {
            IList<Node> RootNodes = new List<Node>();
            Node RootNode = null;

            dynamic xmlReader = DynamicXmlStream.Load(XmlRdr);

            foreach (dynamic RootNodeXml in (xmlReader.NODES.NODE as DynamicXmlStream).AsDynamicEnumerable()
                        .Where(RootNodeXml => RootNodeXml.Depth.Value == "0")
                    )
            {
                RootNode = new Node() { NodeHeader = RootNodeXml.HEADER.Value, NodeDetail = RootNodeXml.DETAIL.Value,  NodeHyperLink = RootNodeXml.HYPERLINK.Value, Depth = 0 };
                RootNodes.Add(RootNode);

                #region Recursively parse RootNodeXml.CHILDNODES
                try
                {
                    RecursiveParseChildNodes(RootNode, RootNodeXml.CHILDNODES);
                }
                catch
                {
                    // Ignored - for root nodes with no CHILDNODES
                }
                #endregion
            }

            return RootNodes;
        }

        protected void RecursiveParseChildNodes(
            Node Node,
            dynamic CHILDNODES
            )
        {
            Node ChildNode = null;

            foreach (dynamic ChildNodeXml in (CHILDNODES.NODE as DynamicXmlStream).AsDynamicEnumerable()
                        .Where(ChildNodeXml => ChildNodeXml.Depth.Value == (Node.Depth + 1).ToString())
                )
            {
                ChildNode = new Node() { NodeHeader = ChildNodeXml.HEADER.Value, NodeDetail = ChildNodeXml.DETAIL.Value, NodeHyperLink = ChildNodeXml.HYPERLINK.Value, Depth = Node.Depth+1 };
                Node.ChildNodes.Add(ChildNode);

                try
                {
                    RecursiveParseChildNodes(ChildNode, ChildNodeXml.CHILDNODES);
                }
                catch
                {
                    // Ignored - for leaf nodes with no CHILDNODES
                }
            }

            return;
        }

        public string RenderGraph(
            IList<Node> Nodes
            )
        {
            int CurrentDepth = 0;
            string RenderedXml = "<NODES>";
            var RootNodes = from RootNode in Nodes
                            where RootNode.ParentNodes.Count == 0
                            select RootNode;
            foreach (Node RootNode in RootNodes)
            {
                RecursiveRenderNodes(RootNode, null, ref CurrentDepth, ref RenderedXml);
            }

            RenderedXml += "</NODES>";

            return RenderedXml;
        }

        protected void RecursiveRenderNodes(
            Node Node,
            Node ParentNode,
            ref int CurrentDepth,
            ref string RenderedXml)
        {
            string Tabs = CurrentDepthToTabs(CurrentDepth);

            RenderedXml += Environment.NewLine;
            RenderedXml += (Tabs + "<NODE");
            RenderedXml += " Depth='" + Node.Depth.ToString() + "'";
            if (ParentNode != null)
            {

            }
            RenderedXml += ">";

            RenderedXml += Environment.NewLine;
            RenderedXml += (Tabs + "\t" + "<HEADER>" + Node.NodeHeader + "</HEADER>");
            RenderedXml += Environment.NewLine;
            RenderedXml += (Tabs + "\t" + "<DETAIL>" + Node.NodeDetail + "</DETAIL>");
            RenderedXml += Environment.NewLine;
            RenderedXml += (Tabs + "\t" + "<HYPERLINK>" + Node.NodeHyperLink + "</HYPERLINK>");

            if (Node.ChildNodes.Count > 0)
            {
                int CopyCurrentDepth = CurrentDepth + 1;

                RenderedXml += Environment.NewLine;
                RenderedXml += (Tabs + "\t" + "<CHILDNODES>");
                foreach (Node ChildNode in Node.ChildNodes)
                {
                    RecursiveRenderNodes(ChildNode, Node, ref CopyCurrentDepth, ref RenderedXml);
                }
                RenderedXml += Environment.NewLine;
                RenderedXml += "</CHILDNODES>";
            }

            RenderedXml += Environment.NewLine;
            RenderedXml += "</NODE>";

            return;
        }

        private string CurrentDepthToTabs(int CurrentDepth)
        {
            string Tabs = null;
            for (int i = 0; i < CurrentDepth; i++)
            {
                Tabs += "\t";
            }
            return Tabs;
        }
    }
}
