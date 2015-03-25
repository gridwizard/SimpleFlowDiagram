using System;
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
        static void Main(string[] args)
        {
            IList<Node> Nodes = new List<Node>();

            GenerateNodes(Nodes, 3, 3);
            Console.WriteLine("Finished generating dummy nodes, # Nodes: " + Nodes.Count);

            CanvasDefinition Canvas = DiagramCanvasEngine.GenerateLayout(
                        Nodes,
                        Node.DEFAULT_NODE_HEIGHT / 2,
                        CanvasDefinition.LayoutDirection.TopToBottom
                        );
            Console.WriteLine("Finished calculating layout");

            GraphDisplayFormatSettings DisplaySettings = new GraphDisplayFormatSettings();
            DisplaySettings.NodeHeaderSettings.ForeColorName = "Black";
            DisplaySettings.NodeDetailSettings.ForeColorName = "Black";

            IGraphRender Html5Render = new Html5GraphRender();
            Html5Render.RenderGraph(Canvas, Nodes, DisplaySettings, "Flowchart.html");

            Console.WriteLine("Finished render to HTML5");
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
