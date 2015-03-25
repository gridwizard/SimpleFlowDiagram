using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFlowDiagramLib
{
    public class CanvasDefinition
    {
        public enum LayoutDirection
        {
            TopToBottom,
            LeftToRight
        }

        protected LayoutDirection _Direction = LayoutDirection.LeftToRight;

        public LayoutDirection Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public int NumSlotsHorizontal { get; set; }
        public int NumSlotsVertical { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
    }
}
