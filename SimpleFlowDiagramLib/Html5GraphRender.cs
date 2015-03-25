using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFlowDiagramLib
{
    public class Html5GraphRender : IGraphRender
    {
        public void RenderGraph(
            CanvasDefinition Canvas,
            IList<Node> Nodes,
            GraphDisplayFormatSettings DisplaySettings,
            string OutputPath
            )
        {
            string RenderedHtml = null;
            const string HTMLTEMPLATE = @"<!DOCTYPE HTML>
                                    <html>
                                      <script type='text/javascript'>
                                        function roundRect(ctx, x, y, w, h, radius, lineColor, lineWidth)
                                        {
                                          var r = x + w;
                                          var b = y + h;
                                          ctx.beginPath();
                                          ctx.strokeStyle=lineColor;
                                          ctx.lineWidth=lineWidth;
                                          ctx.moveTo(x+radius, y);
                                          ctx.lineTo(r-radius, y);
                                          ctx.quadraticCurveTo(r, y, r, y+radius);
                                          ctx.lineTo(r, y+h-radius);
                                          ctx.quadraticCurveTo(r, b, r-radius, b);
                                          ctx.lineTo(x+radius, b);
                                          ctx.quadraticCurveTo(x, b, x, b-radius);
                                          ctx.lineTo(x, y+radius);
                                          ctx.quadraticCurveTo(x, y, x+radius, y);
                                          ctx.stroke();
                                        }
                                        </script>
                                      </head>
                                      <body>
                                        <canvas id='GraphCanvas' width='$OVERALL_WIDTH$' height='$OVERALL_HEIGHT$'></canvas>
                                        <script>
                                          var canvas = document.getElementById('GraphCanvas');
                                          var context = canvas.getContext('2d');
	  
	                                      $DRAWING_CODE$
                                        </script>
                                      </body>
                                    </html>";

            Canvas.Width = Canvas.Width * 1.25;
            Canvas.Height = Canvas.Height * 1.25;

            RenderedHtml = HTMLTEMPLATE;
            RenderedHtml = RenderedHtml.Replace("$OVERALL_WIDTH$", Canvas.Width.ToString());
            RenderedHtml = RenderedHtml.Replace("$OVERALL_HEIGHT$", Canvas.Height.ToString());

            string RenderedNodesHtml = RenderNodes(Canvas, Nodes, DisplaySettings);
            RenderedHtml = RenderedHtml.Replace("$DRAWING_CODE$", RenderedNodesHtml);

            System.IO.File.WriteAllText(OutputPath, RenderedHtml);

            return;
        }

        protected string RenderNodes(CanvasDefinition Canvas, IList<Node> Nodes, GraphDisplayFormatSettings DisplaySettings)
        {
            string RenderedNodesHtml = "";
            foreach (Node n in Nodes)
            {
                RecursiveRenderNodes(Canvas, n, DisplaySettings, ref RenderedNodesHtml);
            }

            return RenderedNodesHtml;
        }

        protected void RecursiveRenderNodes(
            CanvasDefinition Canvas, 
            Node Node, 
            GraphDisplayFormatSettings DisplaySettings, 
            ref string RenderedNodesHtml)
        {
            string RenderedNode = RenderSingleNode(Node, Canvas, DisplaySettings);

            RenderedNodesHtml += Environment.NewLine;
            RenderedNodesHtml += RenderedNode;

            if (Node.ChildNodes.Count > 0)
            {
                foreach (Node ChildNode in Node.ChildNodes)
                {
                    RecursiveRenderNodes(Canvas, ChildNode, DisplaySettings, ref RenderedNodesHtml);
                }
            }

            return;
        }

        protected string RenderSingleNode(Node Node, CanvasDefinition Canvas, GraphDisplayFormatSettings DisplaySettings)
        {
            string RenderedNode = null;
            const string NODE_TEMPLATE = @"context.beginPath();
                                            context.fillStyle = '$NODE_BACKCOLOR$';
                                            roundRect(context, $NODE_X$, $NODE_Y$, $NODE_WIDTH$, $NODE_HEIGHT$, $RADIUS$, '$NODE_BORDERCOLOR$', $LINEWIDTH$);
                                            context.fill();

	                                        context.font='$NODE_HEADER_FONTSTRING$';
	                                        context.fillStyle = '$NODE_HEADER_FONTCOLOR$';
	                                        context.fillText('$NODE_HEADER$',$NODE_HEADER_X$,$NODE_HEADER_Y$);

                                            context.font='$NODE_DETAIL_FONTSTRING$';
	                                        context.fillStyle = '$NODE_DETAIL_FONTCOLOR$';
	                                        context.fillText('$NODE_DETAIL$',$NODE_DETAIL_X$,$NODE_DETAIL_Y$);

                                            $LINE_TO_CHILDREN$
                                             ";
            RenderedNode = NODE_TEMPLATE;

            RenderedNode = RenderedNode.Replace("$LINEWIDTH$", DisplaySettings.LineWidth.ToString());
            RenderedNode = RenderedNode.Replace("$NODE_BACKCOLOR$", DisplaySettings.NodeBackcolorName);
            RenderedNode = RenderedNode.Replace("$NODE_BORDERCOLOR$", DisplaySettings.NodeBorderColorName);
            RenderedNode = RenderedNode.Replace("$RADIUS$", (Node.Height/4).ToString());

            RenderedNode = RenderedNode.Replace("$NODE_X$", Node.x.ToString());
            RenderedNode = RenderedNode.Replace("$NODE_Y$", Node.y.ToString());
            RenderedNode = RenderedNode.Replace("$NODE_WIDTH$", Node.Width.ToString());
            RenderedNode = RenderedNode.Replace("$NODE_HEIGHT$", Node.Height.ToString());

            string NodeHeaderFontString = DisplaySettings.NodeHeaderSettings.FontStyleString + " " + DisplaySettings.NodeHeaderSettings.FontSize.ToString() + "px " + DisplaySettings.NodeHeaderSettings.FontFamilyName;
            // For NodeDetail, FontStyle never Bold (Always Regular, so don't render it)
            string NodeDetailFontString =DisplaySettings.NodeDetailSettings.FontSize.ToString() + "px " + DisplaySettings.NodeDetailSettings.FontFamilyName;

            RenderedNode = RenderedNode.Replace("$NODE_HEADER$", Node.NodeHeader);
            RenderedNode = RenderedNode.Replace("$NODE_HEADER_FONTSTRING$", NodeHeaderFontString);
            RenderedNode = RenderedNode.Replace("$NODE_HEADER_FONTCOLOR$", DisplaySettings.NodeHeaderSettings.ForeColorName);
            RenderedNode = RenderedNode.Replace("$NODE_HEADER_X$", (Node.x + 5).ToString());
            RenderedNode = RenderedNode.Replace("$NODE_HEADER_Y$", (Node.y + 15).ToString());

            RenderedNode = RenderedNode.Replace("$NODE_DETAIL$", string.IsNullOrEmpty(Node.NodeDetail)?"---":Node.NodeDetail);
            RenderedNode = RenderedNode.Replace("$NODE_DETAIL_FONTSTRING$", NodeDetailFontString);
            RenderedNode = RenderedNode.Replace("$NODE_DETAIL_FONTCOLOR$", DisplaySettings.NodeDetailSettings.ForeColorName);
            RenderedNode = RenderedNode.Replace("$NODE_DETAIL_X$", (Node.x + 5).ToString());
            RenderedNode = RenderedNode.Replace("$NODE_DETAIL_Y$", (Node.y + 25).ToString());

            const string LINE_TEMPLATE = @"context.beginPath();
                                            context.moveTo($X1$,$Y1$);
                                            context.lineTo($X2$,$Y2$);
                                            context.stroke();";
            string LineToChildren = null;
            string RenderedLine = null;
            if (Node.ChildNodes.Count > 0)
            {
                foreach (Node ChildNode in Node.ChildNodes)
                {
                    RenderedLine = LINE_TEMPLATE;
                    switch(Canvas.Direction)
                    {
                        case CanvasDefinition.LayoutDirection.LeftToRight:
                            RenderedLine = RenderedLine.Replace("$X1$", (Node.x + Node.Width).ToString());
                            RenderedLine = RenderedLine.Replace("$Y1$", (Node.y + Node.Height/2).ToString());

                            RenderedLine = RenderedLine.Replace("$X2$", (ChildNode.x).ToString());
                            RenderedLine = RenderedLine.Replace("$Y2$", (ChildNode.y + ChildNode.Height/2).ToString());
                            break;
                        case CanvasDefinition.LayoutDirection.TopToBottom:
                            RenderedLine = RenderedLine.Replace("$X1$", (Node.x + Node.Width/2).ToString());
                            RenderedLine = RenderedLine.Replace("$Y1$", (Node.y + Node.Height).ToString());

                            RenderedLine = RenderedLine.Replace("$X2$", (ChildNode.x + ChildNode.Width/2).ToString());
                            RenderedLine = RenderedLine.Replace("$Y2$", (ChildNode.y).ToString());
                            break;
                    }

                    LineToChildren += (Environment.NewLine + RenderedLine);
                }

                RenderedNode = RenderedNode.Replace("$LINE_TO_CHILDREN$", LineToChildren);
            }
            else
            {
                RenderedNode = RenderedNode.Replace("$LINE_TO_CHILDREN$", "");
            }

            return RenderedNode;
        }
    }
}
