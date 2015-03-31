using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleFlowDiagramLib;

namespace SimpleFlowDiagramGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string GraphXmlFilePath = null;
            string OutputHtmlFlowchartPath = null;

            if (args != null && args.Length == 2)
            {
                GraphXmlFilePath = args[0];
                OutputHtmlFlowchartPath = args[1];

                Console.WriteLine("Input: GraphXmlFilePath=" + GraphXmlFilePath);
                Console.WriteLine("Output: OutputHtmlFlowchartPath=" + OutputHtmlFlowchartPath);
            }
            else
            {
                Console.WriteLine("Please specify GraphXmlFilePath (Input graph xml) file AND OutputHtmlFlowchartPath (Output HTML file containing the flow chart)");
                Console.ReadLine();
                return;
            }

            try
            {
                FileStream GraphXmlStream = null;
                SimpleFlowDiagramGeneratorCompatibleGraphRender XmlConverter = new SimpleFlowDiagramGeneratorCompatibleGraphRender();
                GraphXmlStream = System.IO.File.OpenRead(GraphXmlFilePath);
                System.Xml.XmlReader XmlRdr = System.Xml.XmlReader.Create(GraphXmlStream);
                IList<Node> ResurrectedNodes = XmlConverter.ReadGraphXml(XmlRdr);

                CanvasDefinition Canvas = DiagramCanvasEngine.GenerateLayout(
                            ResurrectedNodes,
                            Node.DEFAULT_NODE_HEIGHT / 2,
                            CanvasDefinition.LayoutDirection.LeftToRight
                            );

                IGraphRender Html5Render = new Html5GraphRender();
                GraphDisplayFormatSettings DisplaySettings = new GraphDisplayFormatSettings();
                DisplaySettings.NodeHeaderSettings.ForeColorName = "Black";
                DisplaySettings.NodeDetailSettings.ForeColorName = "Black";

                Html5Render.RenderGraph(Canvas, ResurrectedNodes, DisplaySettings, OutputHtmlFlowchartPath);
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }

            return;
        }
    }
}
