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
        // https://gridwizard.wordpress.com/2015/03/31/simpleflowdiagramlib-simple-c-library-to-serialize-graph-to-xml-and-vice-versa/
        static void Main(string[] args)
        {
            string GraphXmlFilePath = null;
            string OutputHtmlFlowchartPath = null;

            string sTmp = null;
            System.Drawing.Color NodeBackcolor = System.Drawing.Color.White;
            System.Drawing.Color NodeBorderColor = System.Drawing.Color.DarkGray;
            int LineWidth = 2;
            string NodeHeaderSettingsFontStyleString = "Bold";
            float NodeHeaderSettingsFontSize = 12;
            string NodeHeaderSettingsFontFamilyName = "Arial";
            string NodeHeaderSettingsForeColorName = "DarkGray";
            float NodeDetailSettingsFontSize = 8;
            string NodeDetailSettingsFontFamilyName = "Arial";
            string NodeDetailSettingsForeColorName = "LightGray";

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

                #region STEP 1. Read configurations from app.config
                #region Box settings
                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeBackcolor"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    NodeBackcolor = System.Drawing.Color.FromName(sTmp);
                }

                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeBorderColor"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    NodeBorderColor = System.Drawing.Color.FromName(sTmp);
                }

                sTmp = System.Configuration.ConfigurationSettings.AppSettings["LineWidth"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    if (!Int32.TryParse(sTmp, out LineWidth))
                    {
                        LineWidth = 1;
                    }
                }
                #endregion

                #region NodeHeader settings
                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeHeaderSettingsFontStyleString"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    #region Make sure sTmp correct
                    if (NodeHeaderSettingsFontStyleString == "Regular")
                    {
                        NodeHeaderSettingsFontStyleString = sTmp;
                    }
                    else if (NodeHeaderSettingsFontStyleString == "Bold")
                    {
                        NodeHeaderSettingsFontStyleString = sTmp;
                    }
                    else if (NodeHeaderSettingsFontStyleString == "Italic")
                    {
                        NodeHeaderSettingsFontStyleString = sTmp;
                    }
                    else if (NodeHeaderSettingsFontStyleString == "Underline")
                    {
                        NodeHeaderSettingsFontStyleString = sTmp;
                    }
                    else if (NodeHeaderSettingsFontStyleString == "Strikeout")
                    {
                        NodeHeaderSettingsFontStyleString = sTmp;
                    }
                    else
                    {
                        NodeHeaderSettingsFontStyleString = "Regular";
                    }
                    #endregion
                }

                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeHeaderSettingsFontSize"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    if (!float.TryParse(sTmp, out NodeHeaderSettingsFontSize))
                    {
                        NodeHeaderSettingsFontSize = 1;
                    }
                }

                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeHeaderSettingsFontFamilyName"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    NodeHeaderSettingsFontFamilyName = sTmp;
                }

                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeHeaderSettingsForeColorName"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    NodeHeaderSettingsForeColorName = sTmp;
                }
                #endregion

                #region NodeDetail settings
                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeDetailSettingsFontSize"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    if (!float.TryParse(sTmp, out NodeHeaderSettingsFontSize))
                    {
                        NodeDetailSettingsFontSize = 1;
                    }
                }

                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeDetailSettingsFontFamilyName"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    NodeDetailSettingsFontFamilyName = sTmp;
                }

                sTmp = System.Configuration.ConfigurationSettings.AppSettings["NodeDetailSettingsForeColorName"];
                if (!string.IsNullOrEmpty(sTmp))
                {
                    NodeDetailSettingsForeColorName = sTmp;
                }
                #endregion
                #endregion
                Console.WriteLine("STEP 1. Read configurations from app.config - done");

                #region STEP 2. Read XML input nodes
                FileStream GraphXmlStream = null;
                SimpleFlowDiagramGeneratorCompatibleGraphRender XmlConverter = new SimpleFlowDiagramGeneratorCompatibleGraphRender();
                GraphXmlStream = System.IO.File.OpenRead(GraphXmlFilePath);
                System.Xml.XmlReader XmlRdr = System.Xml.XmlReader.Create(GraphXmlStream);
                IList<Node> ResurrectedNodes = XmlConverter.ReadGraphXml(XmlRdr);
                #endregion
                Console.WriteLine("STEP 2. Read " + GraphXmlFilePath + " - done");

                #region STEP 3. Calculate layout Node.x/y
                CanvasDefinition Canvas = DiagramCanvasEngine.GenerateLayout(
                            ResurrectedNodes,
                            Node.DEFAULT_NODE_HEIGHT / 2,
                            CanvasDefinition.LayoutDirection.LeftToRight
                            );
                #endregion
                Console.WriteLine("STEP 3. Calculate layout Node.x/y - done");

                #region STEP 4. Render HTML5
                IGraphRender Html5Render = new Html5GraphRender();
                GraphDisplayFormatSettings DisplaySettings = new GraphDisplayFormatSettings();
                #region Apply presentation settings from App.config
                DisplaySettings.NodeBackcolorName = NodeBackcolor.Name;
                DisplaySettings.NodeBorderColorName = NodeBorderColor.Name;
                DisplaySettings.LineWidth = LineWidth;

                DisplaySettings.NodeHeaderSettings.FontStyleString = NodeHeaderSettingsFontStyleString;
                DisplaySettings.NodeHeaderSettings.FontSize = NodeHeaderSettingsFontSize;
                DisplaySettings.NodeHeaderSettings.FontFamilyName = NodeHeaderSettingsFontFamilyName;
                DisplaySettings.NodeHeaderSettings.ForeColorName = NodeHeaderSettingsForeColorName;

                DisplaySettings.NodeDetailSettings.FontSize = NodeDetailSettingsFontSize;
                DisplaySettings.NodeDetailSettings.FontFamilyName = NodeDetailSettingsFontFamilyName;
                DisplaySettings.NodeDetailSettings.ForeColorName = NodeDetailSettingsForeColorName;
                #endregion

                Html5Render.RenderGraph(Canvas, ResurrectedNodes, DisplaySettings, OutputHtmlFlowchartPath);
                #endregion
                Console.WriteLine("STEP 4. Render HTML5 - done");
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }

            return;
        }
    }
}
