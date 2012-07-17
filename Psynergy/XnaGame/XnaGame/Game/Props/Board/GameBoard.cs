using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

namespace XnaGame
{
    public class GameBoard : Node3D
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("gameboard", "GameBoardName");
            factory.RegisterClass(typeof(ModelNode), "gameboardmodel", "GameBoardModel");
            factory.RegisterVector3("tree", "Tree");

            base.ClassProperties(factory);
        }
        #endregion

        private String m_GameBoardName = "";
        private GameBoardResource m_GameBoardResource = null;
        private ModelNode m_GameBoardModel = null;

        // Trees on the game board
        private List<LTree> m_Trees = new List<LTree>();

        public GameBoard() : base("")
        {
        }

        public GameBoard(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            // Add a tree
            //Tree = new Vector3(-10, 0, 0);
        }

        public override void  Reset()
        {
            base.Reset();

           /* Scene scene = SceneManager.Instance.CurrentScene;

            if ( scene != null )
            {
                for ( int i = 0; i < m_Trees.Count; i++ )
                {
                    LTree tree = m_Trees[i];

                    tree.AddToScene(scene);
                }
            }*/
        }

        protected override void LoadSpecific()
        {
            base.LoadSpecific();

            // Test purposes
            Scene scene = SceneManager.Instance.FindScene("GameScene");

            // Load the game board items
            if (m_GameBoardName != "")
            {
                if (m_GameBoardResource == null)
                {
                    m_GameBoardResource = new GameBoardResource(m_GameBoardName, this);
                    m_GameBoardResource.Load();
                }
            }

            // Continue to load the game board model now
            if (m_GameBoardModel != null)
            {
                // Initialise and load
                m_GameBoardModel.Initialise();

                // Add as a child to the game board object
                AddChild(m_GameBoardModel);
            }
        }

        public override void Update(GameTime deltaTime)
        {
            // Update base class where the main calculations occur.
            base.Update(deltaTime);
        }

        public void AddSquare(BoardSquare boardSquare)
        {
            if (boardSquare != null)
                AddChild(boardSquare);
        }

        public BoardSquare GetBoardSquare(String name)
        {
            BoardSquare boardSquare = null;

            if (Children.Count > 0)
            {
                Node child = FindChild(name);

                if (child != null)
                {
                    Type type = child.GetType();

                    if ((type == typeof(BoardSquare)) || (type.IsSubclassOf(typeof(BoardSquare))))
                        boardSquare = (child as BoardSquare);
                }
            }

            return boardSquare;
        }

        // Load all the board connections
        public void LoadBoardConnections()
        {
            if (Children.Count > 0)
            {
                foreach (Node node in Children)
                {
                    if ( (node.GetType() == typeof(BoardSquare)) || (node.GetType().IsSubclassOf(typeof(BoardSquare))) )
                    {
                        // Run board connection code
                        BoardSquare boardSquare = (node as BoardSquare);
                        boardSquare.LoadBoardConnection(this);
                    }
                }
            }
        }
 
        #region Properties
        public String GameBoardName { get { return m_GameBoardName; } set { m_GameBoardName = value; } }
        public ModelNode GameBoardModel 
        { 
            get 
            {
                return m_GameBoardModel; 
            } 
            set 
            {
                m_GameBoardModel = value;
            } 
        }

        public Vector3 Tree
        {
            get 
            {
                return Vector3.Zero;
            }
            set
            {
                // Create a new tree at this position
                TreeManager treeManager = TreeManager.Instance;

                if (treeManager != null)
                {
                    LTree newTree = treeManager.CreateTree(value);

                    if ( newTree != null )
                        m_Trees.Add(newTree);
                }
            }
        }
        #endregion
    }
}
