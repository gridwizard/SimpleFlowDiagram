using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFlowDiagramLib
{
    public class GridSizingInfo
    {
        public IDictionary<int, double> MaxColumnWidths = new Dictionary<int, double>();
        public IDictionary<int, double> MaxRowHeights = new Dictionary<int, double>();

        public void UpdateSizingInfo(Node Node)
        {
            #region Set SizingInfo.MaxColumnWidths and MaxRowHeights
            if (!this.MaxColumnWidths.ContainsKey(Node.xSlot))
            {
                this.MaxColumnWidths.Add(Node.xSlot, Node.Width);
            }
            else
            {
                if (this.MaxColumnWidths[Node.xSlot] < Node.Width)
                {
                    this.MaxColumnWidths[Node.xSlot] = Node.Width;
                }
            }

            if (!this.MaxRowHeights.ContainsKey(Node.ySlot))
            {
                this.MaxRowHeights.Add(Node.ySlot, Node.Height);
            }
            else
            {
                if (this.MaxRowHeights[Node.ySlot] < Node.Height)
                {
                    this.MaxRowHeights[Node.ySlot] = Node.Height;
                }
            }
            #endregion
        }
    }
}
