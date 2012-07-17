using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Middleware
{
    public enum QuadNodeType
    {
        FullNode = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 3,
        BottomRight = 4,
    };

    public struct QuadNodeVertex
    {
        public int Index { get; set; }
        public bool Activated { get; set; }
    }

    public class QuadNode
    {
        private QuadNode m_ParentNode = null;
        private QuadTree m_ParentTree = null;
        private int m_PositionIndex = -1;

        private int m_NodeDepth = 0;
        private int m_NodeSize = 0;

        private bool m_HasChildren = false;

        private QuadNodeVertex m_VertexTopLeft;
        private QuadNodeVertex m_VertexTop;
        private QuadNodeVertex m_VertexTopRight;
        private QuadNodeVertex m_VertexLeft;
        private QuadNodeVertex m_VertexCenter;
        private QuadNodeVertex m_VertexRight;
        private QuadNodeVertex m_VertexBottomLeft;
        private QuadNodeVertex m_VertexBottom;
        private QuadNodeVertex m_VertexBottomRight;

        private QuadNode m_ChildTopLeft;
        private QuadNode m_ChildTopRight;
        private QuadNode m_ChildBottomLeft;
        private QuadNode m_ChildBottomRight;

        private QuadNode m_NeighbourTop;
        private QuadNode m_NeighbourRight;
        private QuadNode m_NeighbourBottom;
        private QuadNode m_NeighbourLeft;

        private BoundingBox m_BoundingBox;

        /// <summary>
        /// QuadNode constructor
        /// </summary>
        /// <param name="nodeType">Type of node.</param>
        /// <param name="nodeSize">Width/Height of node (# of vertices across - 1).</param>
        /// <param name="nodeDepth">Depth of current node</param>
        /// <param name="parent">Parent QuadNode</param>
        /// <param name="parentTree">Top level Tree.</param>
        /// <param name="positionIndex">Index of top left Vertice in the parent tree Vertices array</param>
        public QuadNode(QuadNodeType nodeType, int nodeSize, int nodeDepth, QuadNode parent, QuadTree parentTree, int positionIndex)
        {
            NodeType = nodeType;
            m_NodeSize = nodeSize;
            m_NodeDepth = nodeDepth;
            m_PositionIndex = positionIndex;

            m_ParentNode = parent;
            m_ParentTree = parentTree;

            // Add the 9 vertices
            AddVertices();

            m_BoundingBox = new BoundingBox(m_ParentTree[VertexTopLeft.Index].Position, m_ParentTree[VertexBottomLeft.Index].Position);
            m_BoundingBox.Min.Y = -950f;
            m_BoundingBox.Max.Y = 950f;

            if (m_NodeSize > 4)
                AddChildren();

            //Make call to UpdateNeighbors from the parent node.
            //This will update all neighbors recursively for the
            //children as well.  This ensures all nodes are created 
            //prior to updating neighbors.
            if (m_NodeDepth == 1)
            {
                AddNeighbours();

                // Activate nodes inside node
                m_VertexTopLeft.Activated = true;
                m_VertexTopRight.Activated = true;
                m_VertexCenter.Activated = true;
                m_VertexBottomLeft.Activated = true;
                m_VertexBottomRight.Activated = true;
            }
        }

        private void AddVertices()
        {
            switch (NodeType)
            {
                case QuadNodeType.TopLeft:
                    {
                        m_VertexTopLeft = m_ParentNode.VertexTopLeft;
                        m_VertexTopRight = m_ParentNode.VertexTop;
                        m_VertexBottomLeft = m_ParentNode.VertexLeft;
                        m_VertexBottomRight = m_ParentNode.VertexCenter;
                    }
                    break;

                case QuadNodeType.TopRight:
                    {
                        m_VertexTopLeft = m_ParentNode.VertexTop;
                        m_VertexTopRight = m_ParentNode.VertexTopRight;
                        m_VertexBottomLeft = m_ParentNode.VertexCenter;
                        m_VertexBottomRight = m_ParentNode.VertexRight;
                    }
                    break;

                case QuadNodeType.BottomLeft:
                    {
                        m_VertexTopLeft = m_ParentNode.VertexLeft;
                        m_VertexTopRight = m_ParentNode.VertexCenter;
                        m_VertexBottomLeft = m_ParentNode.VertexBottomLeft;
                        m_VertexBottomRight = m_ParentNode.VertexBottom;
                    }
                    break;

                case QuadNodeType.BottomRight:
                    {
                        m_VertexTopLeft = m_ParentNode.VertexCenter;
                        m_VertexTopRight = m_ParentNode.VertexRight;
                        m_VertexBottomLeft = m_ParentNode.VertexBottom;
                        m_VertexBottomRight = m_ParentNode.VertexBottomRight;
                    }
                    break;

                default:
                    {
                        m_VertexTopLeft = new QuadNodeVertex { Activated = true, Index = 0 };
                        m_VertexTopRight = new QuadNodeVertex
                        {
                            Activated = true,
                            Index = VertexTopLeft.Index + m_NodeSize
                        };

                        m_VertexBottomLeft = new QuadNodeVertex
                        {
                            Activated = true,
                            Index = (m_ParentTree.TopNodeSize + 1) * m_ParentTree.TopNodeSize
                        };

                        m_VertexBottomRight = new QuadNodeVertex
                        {
                            Activated = true,
                            Index = VertexBottomLeft.Index + m_NodeSize
                        };
                    }
                    break;

            }

            m_VertexTop = new QuadNodeVertex
            {
                Activated = false,
                Index = (int)(VertexTopLeft.Index + (m_NodeSize * 0.5f))
            };

            m_VertexLeft = new QuadNodeVertex
            {
                Activated = false,
                Index = (int)(VertexTopLeft.Index + (m_ParentTree.TopNodeSize + 1) * (m_NodeSize * 0.5f))
            };

            m_VertexCenter = new QuadNodeVertex
            {
                Activated = false,
                Index = (int)(VertexLeft.Index + (m_NodeSize * 0.5f))
            };

            m_VertexRight = new QuadNodeVertex
            {
                Activated = false,
                Index = VertexLeft.Index + m_NodeSize
            };

            m_VertexBottom = new QuadNodeVertex
            {
                Activated = false,
                Index = (int)(VertexBottomLeft.Index + (m_NodeSize * 0.5f))
            };
        }

        private void AddChildren()
        {
            //Add top left (northwest) child
            m_ChildTopLeft = new QuadNode(QuadNodeType.TopLeft, (int)(m_NodeSize * 0.5f), m_NodeDepth + 1, this, m_ParentTree, VertexTopLeft.Index);

            //Add top right (northeast) child
            m_ChildTopRight = new QuadNode(QuadNodeType.TopRight, (int)(m_NodeSize * 0.5f), m_NodeDepth + 1, this, m_ParentTree, VertexTop.Index);

            //Add bottom left (southwest) child
            m_ChildBottomLeft = new QuadNode(QuadNodeType.BottomLeft, (int)(m_NodeSize * 0.5f), m_NodeDepth + 1, this, m_ParentTree, VertexLeft.Index);

            //Add bottom right (southeast) child
            m_ChildBottomRight = new QuadNode(QuadNodeType.BottomRight, (int)(m_NodeSize * 0.5f), m_NodeDepth + 1, this, m_ParentTree, VertexCenter.Index);

            // State that this node has children
            m_HasChildren = true;
        }

        private void AddNeighbours()
        {
            switch (NodeType)
            {
                case QuadNodeType.TopLeft: //Top Left Corner
                    //Top neighbor
                    if (m_ParentNode.NeighbourTop != null)
                        m_NeighbourTop = m_ParentNode.NeighbourTop.ChildBottomLeft;

                    //Right neighbor
                    m_NeighbourRight = m_ParentNode.ChildTopRight;

                    //Bottom neighbor
                    m_NeighbourBottom = m_ParentNode.ChildBottomLeft;

                    //Left neighbor
                    if (m_ParentNode.NeighbourLeft != null)
                        m_NeighbourLeft = m_ParentNode.NeighbourLeft.ChildTopRight;

                    break;

                case QuadNodeType.TopRight: //Top Right Corner
                    //Top neighbor
                    if (m_ParentNode.NeighbourTop != null)
                        m_NeighbourTop = m_ParentNode.NeighbourTop.ChildBottomRight;

                    //Right neighbor
                    if (m_ParentNode.NeighbourRight != null)
                        m_NeighbourRight = m_ParentNode.NeighbourRight.ChildTopLeft;

                    //Bottom neighbor
                    m_NeighbourBottom = m_ParentNode.ChildBottomRight;

                    //Left neighbor
                    m_NeighbourLeft = m_ParentNode.ChildTopLeft;

                    break;

                case QuadNodeType.BottomLeft: //Bottom Left Corner
                    //Top neighbor
                    m_NeighbourTop = m_ParentNode.ChildTopLeft;

                    //Right neighbor
                    m_NeighbourRight = m_ParentNode.ChildBottomRight;

                    //Bottom neighbor
                    if (m_ParentNode.NeighbourBottom != null)
                        m_NeighbourBottom = m_ParentNode.NeighbourBottom.ChildTopLeft;

                    //Left neighbor
                    if (m_ParentNode.NeighbourLeft != null)
                        m_NeighbourLeft = m_ParentNode.NeighbourLeft.ChildBottomRight;

                    break;

                case QuadNodeType.BottomRight: //Bottom Right Corner
                    //Top neighbor
                    m_NeighbourTop = m_ParentNode.ChildTopRight;

                    //Right neighbor
                    if (m_ParentNode.NeighbourRight != null)
                        m_NeighbourRight = m_ParentNode.NeighbourRight.ChildBottomLeft;

                    //Bottom neighbor
                    if (m_ParentNode.NeighbourBottom != null)
                        m_NeighbourBottom = m_ParentNode.NeighbourBottom.ChildTopRight;

                    //Left neighbor
                    m_NeighbourLeft = m_ParentNode.ChildBottomLeft;

                    break;
            }

            if (m_HasChildren)
            {
                m_ChildTopLeft.AddNeighbours();
                m_ChildTopRight.AddNeighbours();
                m_ChildBottomLeft.AddNeighbours();
                m_ChildBottomRight.AddNeighbours();
            }
        }

        #region VERTICES
        public QuadNodeVertex VertexTopLeft { get { return m_VertexTopLeft; } }
        public QuadNodeVertex VertexTop { get { return m_VertexTop; } }
        public QuadNodeVertex VertexTopRight { get { return m_VertexTopRight; } }
        public QuadNodeVertex VertexLeft { get { return m_VertexLeft; } }
        public QuadNodeVertex VertexCenter { get { return m_VertexCenter; } }
        public QuadNodeVertex VertexRight { get { return m_VertexRight; } }
        public QuadNodeVertex VertexBottomLeft { get { return m_VertexBottomLeft; } }
        public QuadNodeVertex VertexBottom { get { return m_VertexBottom; } }
        public QuadNodeVertex VertexBottomRight { get { return m_VertexBottomRight; } }
        #endregion

        #region CHILDREN
        public QuadNode ChildTopLeft { get { return m_ChildTopLeft; } }
        public QuadNode ChildTopRight { get { return m_ChildTopRight; } }
        public QuadNode ChildBottomLeft { get { return m_ChildBottomLeft; } }
        public QuadNode ChildBottomRight { get { return m_ChildBottomRight; } }
        #endregion

        #region NEIGHBOURS
        public QuadNode NeighbourTop { get { return m_NeighbourTop; } }
        public QuadNode NeighbourRight { get { return m_NeighbourRight; } }
        public QuadNode NeighbourBottom { get { return m_NeighbourBottom; } }
        public QuadNode NeighbourLeft { get { return m_NeighbourLeft; } }
        #endregion

        public BoundingBox Bounds { get { return m_BoundingBox; } }
        public QuadNodeType NodeType { get; set; }
    }
}
