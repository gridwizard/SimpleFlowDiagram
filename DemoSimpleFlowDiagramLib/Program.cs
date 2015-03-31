using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleFlowDiagramLib;

namespace DemoSimpleFlowDiagramLib
{
    class Program
    {
        // https://gridwizard.wordpress.com/2015/03/31/simpleflowdiagramlib-simple-c-library-to-serialize-graph-to-xml-and-vice-versa/
        static void Main(string[] args)
        {
            /*
             * DEMO 1: Basic Node.x/y calculation and rendering
             */
            Console.WriteLine("DEMO 1: Basic Node.x/y calculation and rendering");
            IList<Node> Nodes = new List<Node>();

            GenerateNodes(Nodes, 3, 3);
            Console.WriteLine("Finished generating dummy nodes, # Nodes: " + Nodes.Count);

            CanvasDefinition Canvas = DiagramCanvasEngine.GenerateLayout(
                        Nodes,
                        Node.DEFAULT_NODE_HEIGHT / 2,
                        CanvasDefinition.LayoutDirection.LeftToRight
                        );
            Console.WriteLine("Finished calculating layout");

            GraphDisplayFormatSettings DisplaySettings = new GraphDisplayFormatSettings();
            DisplaySettings.NodeHeaderSettings.ForeColorName = "Black";
            DisplaySettings.NodeDetailSettings.ForeColorName = "Black";

            IGraphRender Html5Render = new Html5GraphRender();
            Html5Render.RenderGraph(Canvas, Nodes, DisplaySettings, "Flowchart.html");

            Console.WriteLine("Finished render to HTML5 to Flowchart.html");


            /*
             * DEMO 2. SimpleFlowDiagramGeneratorCompatibleGraphRender
             */
            Console.WriteLine("DEMO 2. SimpleFlowDiagramGeneratorCompatibleGraphRender");
            SimpleFlowDiagramGeneratorCompatibleGraphRender XmlConverter = new SimpleFlowDiagramGeneratorCompatibleGraphRender();
            string GraphXml = XmlConverter.RenderGraph(Nodes);
            string GraphXmlFilePath = "GraphXml.xml";
            System.IO.File.WriteAllText(GraphXmlFilePath, GraphXml);

            Console.WriteLine("Finished writing graph to XML format compatible with SimpleFlowDiagramGenerator.exe");

            // Read back from GraphXml
            MemoryStream Stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(Stream);
            writer.Write(GraphXml);
            writer.Flush();
            Stream.Position = 0;
            System.Xml.XmlReader XmlRdr = System.Xml.XmlReader.Create(Stream);
            IList<Node> ResurrectedNodes = XmlConverter.ReadGraphXml(XmlRdr);

            DiagramCanvasEngine.GenerateLayout(
                        ResurrectedNodes,
                        Node.DEFAULT_NODE_HEIGHT / 2,
                        CanvasDefinition.LayoutDirection.LeftToRight
                        );

            IGraphRender Html5Render2 = new Html5GraphRender();
            Html5Render.RenderGraph(Canvas, ResurrectedNodes, DisplaySettings, "Flowchart_Resurrected.html");

            Console.WriteLine("Finished render to HTML5 to Flowchart_Resurrected.html");

            return;
        }

        public static void GenerateNodes(
            IList<Node> Nodes,
            int NumRootNodes,
            int MaxTreeDepth
            )
        {
            Node RootNode;
            for (int i = 0; i < NumRootNodes; i++)
            {
                RootNode = new Node();
                RootNode.NodeHeader = "Root_" + i;
                RootNode.NodeDetail = "Some detail ...";
                RootNode.NodeHyperLink = "http://somewhere.com";
                RootNode.Depth = 0;
                Nodes.Add(RootNode);
                GenerateSingleGraph(Nodes, RootNode, MaxTreeDepth);
            }

            return;
        }

        public static void GenerateSingleGraph(
            IList<Node> Nodes,
            Node RootNode,
            int MaxTreeDepth
            )
        {
            int CurrentDepth = 0;
            RecursiveGenerateGraph(Nodes, RootNode, MaxTreeDepth, ref CurrentDepth);
            return;
        }

        public static void RecursiveGenerateGraph(
            IList<Node> Nodes,
            Node Node,
            int MaxTreeDepth,
            ref int CurrentDepth
            )
        {
            CurrentDepth++;

            Random rnd = new Random(DateTime.Now.Second);
            if (CurrentDepth < MaxTreeDepth)
            {
                int NumChildren = rnd.Next(5);
                for (int i = 0; i < NumChildren; i++)
                {
                    Node Child = new Node();
                    Child.NodeHeader = Node.NodeHeader + "." + "Child_Level" + CurrentDepth + "_Num" + i;
                    Child.NodeDetail = "Some detail ...";
                    Child.NodeHyperLink = "http://somewhere.com";
                    Child.Depth = CurrentDepth;

                    Child.ParentNodes.Add(Node);
                    Node.ChildNodes.Add(Child);

                    Nodes.Add(Child);

                    int CopyCurrentDepeth = CurrentDepth;
                    RecursiveGenerateGraph(Nodes, Child, MaxTreeDepth, ref CopyCurrentDepeth);
                }
            }

            return;
        }
    }
}
