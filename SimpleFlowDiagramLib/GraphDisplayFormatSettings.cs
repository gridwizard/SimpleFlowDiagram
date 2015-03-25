using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace SimpleFlowDiagramLib
{
    public class GraphDisplayFormatSettings
    {
        public DisplayFormatSetting NodeHeaderSettings = new DisplayFormatSetting() { FontSize = 12, FontStyleString = "Bold" };
        public DisplayFormatSetting NodeDetailSettings = new DisplayFormatSetting() { FontSize = 12, FontStyleString = "Regular" };

        public int LineWidth = 1;
        
        public string NodeBorderColorName = "DimGray";
        public Color NodeBorderColor
        {
            get
            {
                Color cr = Color.Black;
                cr = Color.FromName(NodeBorderColorName);
                return cr;
            }
        }

        public string NodeBackcolorName = "LightGray";
        public Color NodeBackcolor
        {
            get
            {
                Color cr = Color.Black;
                cr = Color.FromName(NodeBackcolorName);
                return cr;
            }
        }
    }
}
