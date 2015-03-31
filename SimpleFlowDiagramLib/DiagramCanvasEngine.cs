using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFlowDiagramLib
{
    // https://gridwizard.wordpress.com/2015/03/31/simpleflowdiagramlib-simple-c-library-to-serialize-graph-to-xml-and-vice-versa/
    public class DiagramCanvasEngine
    {
        // Set x,y,width,height of each Node
        public static CanvasDefinition GenerateLayout(
            IList<Node> Nodes,
            double Padding,
            CanvasDefinition.LayoutDirection Direction = CanvasDefinition.LayoutDirection.LeftToRight
            )
        {
            CanvasDefinition Canvas = null;

            if (Nodes == null || Nodes.Count == 0)
            {
                return null;
            }

            var RootNodes = from n in Nodes
                            where n.ParentNodes.Count==0
                            select n;
            if (RootNodes != null)
            {
                #region Assign individual Node to slots, and update Node.x/Node.y
                GridSizingInfo SizingInfo = GenerateGraph(RootNodes, Direction, Padding);

                Canvas = new CanvasDefinition() { Direction = Direction };
                Canvas.NumSlotsHorizontal = (from Entry in SizingInfo.MaxColumnWidths
                                select Entry.Value).Count();
                Canvas.NumSlotsVertical = (from Entry in SizingInfo.MaxRowHeights
                                 select Entry.Value).Count();
                Canvas.Width = (from Entry in SizingInfo.MaxColumnWidths
                                select Entry.Value).Sum() + Canvas.NumSlotsHorizontal * Padding;
                Canvas.Height = (from Entry in SizingInfo.MaxRowHeights
                                 select Entry.Value).Sum() + Canvas.NumSlotsVertical * Padding;
                #endregion
            }
            else
            {
                throw new ArgumentException("There must be at least one root node");
            }
            
            return Canvas;
        }

        protected static GridSizingInfo GenerateGraph(
            IEnumerable<Node> RootNodes, 
            CanvasDefinition.LayoutDirection Direction,
            double Padding)
        {
            int i = 0;
            int j = 0;
            GridSizingInfo SizingInfo = new GridSizingInfo();

            foreach (Node RootNode in RootNodes)
            {
                RootNode.Width = RootNode.Width * 1.25;
                RootNode.Height = RootNode.Height * 1.25;
            }

            RecursiveGenerateGrid(RootNodes, SizingInfo, Direction, i, j);
            RecursivePositionSlots(RootNodes, SizingInfo, Direction, Padding);

            return SizingInfo;
        }

        // Employ "Depth First" strategy (As supposed to "Breath First")
        protected static void RecursiveGenerateGrid(
            IEnumerable<Node> Nodes, 
            GridSizingInfo SizingInfo,
            CanvasDefinition.LayoutDirection Direction, 
            int i, int j)
        {
            int NumDecendantsLeaveNodes=0;
            
            foreach (Node n in Nodes)
            {
                NumDecendantsLeaveNodes = FindNumDecendantsLeaveNodes(n);

                #region Set Slot
                switch (Direction)
                {
                    case CanvasDefinition.LayoutDirection.LeftToRight:
                        n.xSlot = i;
                        n.ySlot = j;

                        SizingInfo.UpdateSizingInfo(n);

                        RecursiveGenerateGrid(n.ChildNodes, SizingInfo, Direction, i + 1, j);
                        j += NumDecendantsLeaveNodes > 0 ? NumDecendantsLeaveNodes : 1;
                        break;
                    case CanvasDefinition.LayoutDirection.TopToBottom:
                        n.xSlot = i;
                        n.ySlot = j;

                        SizingInfo.UpdateSizingInfo(n);

                        RecursiveGenerateGrid(n.ChildNodes, SizingInfo, Direction, i, j + 1);
                        i += NumDecendantsLeaveNodes > 0 ? NumDecendantsLeaveNodes : 1;
                        break;
                }
                #endregion
            }

            return;
        }

        protected static void RecursivePositionSlots(
            IEnumerable<Node> Nodes, 
            GridSizingInfo SizingInfo,
            CanvasDefinition.LayoutDirection Direction, 
            double Padding
            )
        {
            double CumulativeWidth = 0;
            double CumulativeHeight = 0;

            foreach (Node n in Nodes)
            {
                #region Set Slot Position
                CumulativeWidth = 0;
                CumulativeHeight = 0;

                CumulativeWidth = 0;
                for (int i = 0; i < n.xSlot; i++)
                {
                    CumulativeWidth += (SizingInfo.MaxColumnWidths[i] + Padding);
                }
                
                for (int j = 0; j < n.ySlot; j++)
                {
                    CumulativeHeight += (SizingInfo.MaxRowHeights[j] + Padding);
                }

                n.x = CumulativeWidth;
                n.y = CumulativeHeight;

                if (n.ChildNodes.Count > 0)
                {
                    #region Center node alignment
                    double Offset = 0;
                    int k = 0;
                    switch (Direction)
                    {
                        case CanvasDefinition.LayoutDirection.LeftToRight:
                            double ChildNodeHeight = 0;
                            foreach (Node ChildNode in n.ChildNodes)
                            {
                                ChildNodeHeight = FindChildHeight(ChildNode, Padding);
                                if (k < n.ChildNodes.Count - 1)
                                {
                                    Offset += (ChildNodeHeight + Padding);
                                }
                                else
                                {
                                    Offset += (ChildNodeHeight);
                                }

                                k++;
                            }

                            Offset -= (n.Height);
                            n.y += (Offset / 2.0);
                            break;
                        case CanvasDefinition.LayoutDirection.TopToBottom:
                            double ChildNodeWidth = 0;
                            foreach (Node ChildNode in n.ChildNodes)
                            {
                                ChildNodeWidth = FindChildWidth(ChildNode, Padding);
                                if (k < n.ChildNodes.Count - 1)
                                {
                                    Offset += (ChildNodeWidth + Padding);
                                }
                                else
                                {
                                    Offset += (ChildNodeWidth);
                                }

                                k++;
                            }

                            Offset -= (n.Width);
                            n.x += (Offset / 2.0);
                            break;
                    }
                    #endregion
                }

                RecursivePositionSlots(n.ChildNodes, SizingInfo, Direction, Padding);
                #endregion
            }

            return;
        }

        protected static int FindNumDecendantsLeaveNodes(Node Node)
        {
            int NumDecendantsLeafNodes = 0;

            if (Node.ChildNodes.Count > 0)
            {
                RecursiveFindNumDecendantsLeaveNodes(Node, ref NumDecendantsLeafNodes);
            }
            else
            {
                // The node is a leaf node
                return 0;
            }

            return NumDecendantsLeafNodes;
        }

        protected static void RecursiveFindNumDecendantsLeaveNodes(Node Node, ref int CountLeafNodes)
        {
            foreach (Node ChildNode in Node.ChildNodes)
            {
                if (ChildNode.ChildNodes.Count == 0) // Hitting a leaf node
                {
                    CountLeafNodes++;
                }
                else
                {
                    RecursiveFindNumDecendantsLeaveNodes(ChildNode, ref CountLeafNodes);
                }
            }
            
            return;
        }

        protected static int FindMaxDepth(Node Node)
        {
            int MaxDepth = 0;

            if (Node.ChildNodes.Count > 0)
            {
                RecursiveFindMaxDepth(Node, ref MaxDepth);
            }
            else
            {
                // The node is a leaf node
                return 0;
            }

            return MaxDepth;
        }

        protected static void RecursiveFindMaxDepth(Node Node, ref int MaxDepth)
        {
            int MaxChildNodeDepth = 0;

            if (Node.ChildNodes.Count > 0)
            {
                foreach (Node ChildNode in Node.ChildNodes)
                {
                    int ChildNodeDepth = 0;
                    RecursiveFindMaxDepth(ChildNode, ref ChildNodeDepth);
                    if (ChildNodeDepth > MaxChildNodeDepth)
                    {
                        MaxChildNodeDepth = ChildNodeDepth;
                    }
                }

                MaxDepth += (MaxChildNodeDepth + 1); // "1" for *Current* level
            }

            return;
        }

        protected static double FindCumulativeHeight(Node Node, double Padding)
        {
            double Height = 0;

            if (Node.ParentNodes.Count > 0)
            {
                RecursiveFindCumulativeHeight(Node, Padding, ref Height);
            }
            else
            {
                // The node is a leaf node
                return 0;
            }

            return Height;
        }

        protected static void RecursiveFindCumulativeHeight(Node Node, double Padding, ref double Height)
        {
            foreach (Node ParentNode in Node.ParentNodes)
            {
                Height += (ParentNode.Height + Padding);

                if (ParentNode.ParentNodes.Count > 0)
                {
                    RecursiveFindCumulativeHeight(ParentNode, Padding, ref Height);
                }
            }

            return;
        }

        protected static double FindCumulativeWidth(Node Node, double Padding)
        {
            double Width = 0;

            if (Node.ParentNodes.Count > 0)
            {
                RecursiveFindCumulativeWidth(Node, Padding, ref Width);
            }
            else
            {
                // The node is a leaf node
                return 0;
            }

            return Width;
        }

        protected static void RecursiveFindCumulativeWidth(Node Node, double Padding, ref double Width)
        {
            foreach (Node ParentNode in Node.ParentNodes)
            {
                Width += (ParentNode.Width + Padding);

                if (ParentNode.ParentNodes.Count > 0)
                {
                    RecursiveFindCumulativeWidth(ParentNode, Padding, ref Width);
                }
            }

            return;
        }

        protected static double FindChildHeight(Node Node, double Padding)
        {
            double Height = 0;

            if (Node.ChildNodes.Count > 0)
            {
                RecursiveFindChildHeight(Node, Padding, ref Height);
            }
            else
            {
                // The node is a leaf node
                return Node.Height;
            }

            return Height;
        }

        protected static void RecursiveFindChildHeight(Node Node, double Padding, ref double Height)
        {
            foreach (Node ChildNode in Node.ChildNodes)
            {
                Height += (ChildNode.Height + Padding);

                if (ChildNode.ChildNodes.Count > 0)
                {
                    RecursiveFindCumulativeWidth(ChildNode, Padding, ref Height);
                }
            }

            return;
        }

        protected static double FindChildWidth(Node Node, double Padding)
        {
            double Width = 0;

            if (Node.ChildNodes.Count > 0)
            {
                RecursiveFindChildWidth(Node, Padding, ref Width);
            }
            else
            {
                // The node is a leaf node
                return Node.Width;
            }

            return Width;
        }

        protected static void RecursiveFindChildWidth(Node Node, double Padding, ref double Width)
        {
            foreach (Node ChildNode in Node.ChildNodes)
            {
                Width += (ChildNode.Width + Padding);

                if (ChildNode.ChildNodes.Count > 0)
                {
                    RecursiveFindCumulativeWidth(ChildNode, Padding, ref Width);
                }
            }

            return;
        }
    }
}
