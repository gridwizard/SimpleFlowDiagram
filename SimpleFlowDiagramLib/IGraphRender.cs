using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFlowDiagramLib
{
    public interface IGraphRender
    {
        string RenderGraph(
            CanvasDefinition Canvas,
            IList<Node> Nodes,
            GraphDisplayFormatSettings DisplaySettings,
            string OutputPath
            );
    }
}
