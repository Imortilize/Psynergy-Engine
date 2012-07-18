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

using Psynergy;
using Psynergy.Events;
using Psynergy.Camera;
using Psynergy.Graphics;

namespace XnaGame
{
    public class BoardSquare : ModelNode, IListener<FirstQuestionPassedEvent>
    {
        public enum Route
        {
            eDefault = 0,
            eAlternative1,
            eAlternative2,
            eAlternative3
        };

        public struct DecisionData
        {
            public DecisionData(Route inRoute)
            {
                route = inRoute;
                diceRoll = 0;
            }

            public DecisionData( int inDiceRoll )
            {
                route = Route.eDefault;
                diceRoll = inDiceRoll;
            }

            public DecisionData( Route inRoute, int inDiceRoll )
            {
                route = inRoute;
                diceRoll = inDiceRoll;
            }

            public void Reset()
            {
                route = Route.eDefault;
                diceRoll = 0;
            }

            public Route route;
            public int diceRoll;
        };

        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterClass(typeof(BoardTrail), "trail", "Trail");
            factory.RegisterBool("question", "QuestionSquare");
            factory.RegisterBool("firstquestionsquare", "FirstQuestionSquare");

            base.ClassProperties(factory);
        }
        #endregion

        private SortedList<Route, BoardSquare> m_ParentSquares = new SortedList<Route, BoardSquare>();
        private SortedList<Route, BoardSquare> m_ConnectedSquares = new SortedList<Route, BoardSquare>();
        private List<BoardTrail> m_Trails = new List<BoardTrail>();  // The trail from the previous point to this point ( If there isn't a previous point then it is null )

        // Multiple pawn position
        private List<GamePawn> m_PawnsPresent = new List<GamePawn>();
        private List<Vector3> m_PawnPositions = new List<Vector3>();

        // Wether it is a question square or not
        private bool m_QuestionSquare = false;
        private bool m_FirstQuestionSquare = false;
        private bool m_FirstQuestionPassed = false;
        private ParticleSystemEffect m_QuestionEffect = null;

        public BoardSquare() : base("")
        {
        }

        public BoardSquare(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            m_ModelName = "Models/FBX/cubemodel";

            Scale = new Vector3(5, 0.1f, 5);

            float spacing = 5;

            // Add position
            m_PawnPositions.Add(Position + new Vector3(-spacing, 0, spacing));
            m_PawnPositions.Add(Position + new Vector3(-spacing, 0, -spacing));
            m_PawnPositions.Add(Position + new Vector3(spacing, 0, spacing));
            m_PawnPositions.Add(Position + new Vector3(spacing, 0, -spacing));

            // If this square is a question square, add a particle effect for it
            if (m_QuestionSquare)
                m_QuestionEffect = ParticleEngine.Instance.AddParticleSystem("questionsquare");
        }

        public override void Reset()
        {
            base.Reset();

            if (Name != "Start")
            {
                // Number of pawns present should be none herem_PawnsPresent
                m_PawnsPresent.Clear();
            }

            m_FirstQuestionPassed = false;

            // Rotate 180 degrees on Y
            Matrix rot = Matrix.CreateFromQuaternion(Quaternion.Identity);
            Matrix newRot = Matrix.CreateRotationY(MathHelper.ToRadians(180));
            rot *= newRot;

            m_Rotation = Quaternion.CreateFromRotationMatrix(rot);
        }

        protected override void LoadTextures()
        {
            base.LoadTextures();

            if ((m_Model != null) && (m_Textures.Count > 0))
            {
                foreach (ModelMesh mesh in m_Model.Meshes)
                {
                    foreach (ModelMeshPart meshpart in mesh.MeshParts)
                    {
                        if (meshpart.Effect.Parameters["Texture"] != null )
                            meshpart.Effect.Parameters["Texture"].SetValue(m_Textures[0]);
                    }
                }
            }
        }

        public void AddConnectedSquare(Route routeIndex, BoardSquare boardSquare)
        {
            if (boardSquare != null)
            {
                if (!m_ConnectedSquares.ContainsKey(routeIndex))
                {
                    m_ConnectedSquares.Add(routeIndex, boardSquare);

                    // Add this as the parent square to the child square just added
                    boardSquare.AddParentSquare(routeIndex, this);
                }
            }
        }

        public void AddParentSquare(Route routeIndex, BoardSquare boardSquare)
        {
            if (boardSquare != null)
            {
                if (!m_ParentSquares.ContainsKey(routeIndex))
                    m_ParentSquares.Add(routeIndex, boardSquare);
            }
        }

       /* public void SetTrail(BoardTrail boardTrail)
        {
            if (boardTrail != null)
                m_Trail = boardTrail;
        }*/

        public virtual DecisionData Decision(int rollNumber)
        {
            return new DecisionData(rollNumber);
        }

        public virtual Route GetRoute( int x )
        {
            return Route.eDefault;
        }

        public BoardSquare PreviousSquare()
        {
            BoardSquare previousSquare = null;

            // If there are any connected squares get the next one ( needs to be made decision based )
            if (m_ParentSquares.ContainsKey(Route.eDefault))
            {
                int index = m_ParentSquares.IndexOfKey(Route.eDefault);

                // The next square value
                previousSquare = m_ParentSquares.Values[index];
            }

            return previousSquare;
        }

        public BoardSquare NextSquare(Route value)
        {
            BoardSquare nextSquare = null;

            // If there are any connected squares get the next one ( needs to be made decision based )
            if (m_ConnectedSquares.ContainsKey(value))
            {
                int index = m_ConnectedSquares.IndexOfKey(value);
                
                // The next square value
                nextSquare = m_ConnectedSquares.Values[index];
            }

            return nextSquare;
        }

        public override void Update(GameTime deltaTime)
        {
            // Update base class where the main calculations occur.
            base.Update(deltaTime);

            // Update any effects that apply to this square (name question squares at the moment )
            if (m_QuestionSquare && (m_FirstQuestionSquare || m_FirstQuestionPassed))
            {
                if (m_QuestionEffect != null)
                    m_QuestionEffect.Trigger(Position);
            }
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            if (m_ConnectedSquares.Count > 0)
            {
                foreach (BoardSquare boardSquare in m_ConnectedSquares.Values)
                    RenderTrails(boardSquare, deltaTime);
            }
        }

        private void RenderTrails(BoardSquare boardSquare, GameTime deltaTime)
        {
            foreach (BoardTrail trail in boardSquare.Trails)
                trail.Render(deltaTime);
        }

        public void LoadBoardConnection(GameBoard gameBoard)
        {
            if (gameBoard != null)
            {
                // Go through all trails and load all connections between them
                foreach (BoardTrail trail in m_Trails)
                {
                    List<String> connections = trail.Connections;

                    foreach (String connection in connections)
                    {
                        if (connection != "")
                        {
                            BoardSquare boardSquare = (gameBoard.FindChild(connection) as BoardSquare);

                            // If the connected square was found add this as a connected square.
                            if (boardSquare != null)
                            {
                                Route route = Route.eDefault;

                                if (trail != null)
                                    route = trail.Route;

                                // Add connected square to the parent board square
                                boardSquare.AddConnectedSquare(route, this);
                            }
                        }
                    }
                }
            }
        }

        public Vector3 EnterSquare(GamePawn pawn)
        {
            Debug.Assert(pawn != null, "Game pawn should not be null!");

            Vector3 toRet = Position;

            if (pawn != null)
            {
                // Check first if there is one player or more
                // Pawn number
                int pawnIndex = -1;

                if (!m_PawnsPresent.Contains(pawn))
                {
                    Debug.Assert(m_PawnPositions.Count > pawnIndex, "No more positions for pawns to move to!");

                    pawnIndex = m_PawnsPresent.Count;
                }
                else
                    pawnIndex = m_PawnsPresent.IndexOf(pawn);

                Debug.Assert(pawnIndex >= 0, "Pawn index must be 0 or greater!");

                if (pawnIndex >= 0)
                {
                    if (m_PawnPositions.Count > pawnIndex)
                    {
                        // If there is already a pawn present on this square
                        if (pawnIndex > 0)
                            toRet = m_PawnPositions[pawnIndex];

                        // Add pawn to pawns present
                        AddActivePawn(pawn);
                    }
                }
            }

            return toRet;
        }

        public void LeaveSquare( GamePawn pawn )
        {
            Debug.Assert(pawn != null, "Game pawn should not be null!");

            if (pawn != null)
            {
                RemoveActivePawn(pawn);
            }
        }

        private void AddActivePawn(GamePawn pawn)
        {
            Debug.Assert(pawn != null, "Game pawn should not be null!");

            if (pawn != null)
            {
                if (!m_PawnsPresent.Contains(pawn))
                    m_PawnsPresent.Add(pawn);
            }
        }

        private void RemoveActivePawn(GamePawn pawn)
        {
            Debug.Assert(pawn != null, "Game pawn should not be null!");

            if (pawn != null)
            {
                if (m_PawnsPresent.Contains(pawn))
                    m_PawnsPresent.Remove(pawn);
            }
        }

        public virtual String GetSquareText()
        {
            return "";
        }

        #region Event Handlers
        public void Handle(SelectObjectEvent message)
        {
            Ray castedRay = message.CastedRay;
            float? intersection = castedRay.Intersects(m_BoundingBox);

            // If this objects sphere is hit then select it
            if (intersection != null)
            {
                BaseCamera activeCamera = RenderManager.Instance.GetActiveCamera();

                if (activeCamera != null)
                {
                    if (activeCamera.CameraType == CameraType.eCameraType_World)
                    {
                        // Change to the on exit state
                        activeCamera.OnChange();
                    }

                    // Set this as the focus
                    activeCamera.SetFocus(this);
                }

                // Run on selection code
                OnSelect();
            }
        }
        #endregion
 
        #region Properties
        public BoardTrail Trail 
        { 
            get 
            {
                BoardTrail trail = null;

                if (m_Trails.Count > 0)
                    trail = m_Trails[0];

                return trail; 
            } 
            set 
            { 
                m_Trails.Add(value); 
            } 
        }

        #region Event handlers
        public virtual void Handle(FirstQuestionPassedEvent message)
        {
            m_FirstQuestionPassed = true;
        }
        #endregion

        public List<BoardTrail> Trails { get { return m_Trails; } }
        public bool QuestionSquare { get { return m_QuestionSquare; } set { m_QuestionSquare = value; } }
        public bool FirstQuestionSquare { get { return m_FirstQuestionSquare; } set { m_FirstQuestionSquare = value; } }
        public bool FirstQuestionPassed { get { return m_FirstQuestionPassed; } }
        #endregion
    }
}
