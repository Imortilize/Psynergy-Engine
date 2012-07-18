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
using Psynergy.Graphics;
using Psynergy.AI;
using Psynergy.Menus;

namespace XnaGame
{
    public class GamePawn : Player, IListener<SelectObjectEvent>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        // Game board variables
        private GameBoard m_GameBoard = null;
        private BoardSquare m_CurrentSquare = null;
        //private String m_RedTextureName = "Red";
        //private String m_BlueTextureName = "Blue";
        private String m_DesiredTexture = "default";
        private String m_CurrentTexture = "default";

        private ParticleSystemEffect m_TrailEffect = null;
        private ParticleSystemEffect m_SelectedEffect = null;

        // Textures
        private Texture2D m_RedTexture = null;
        private Texture2D m_BlueTexture = null;

        public GamePawn() : base("")
        {
        }

        public GamePawn(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            RenderGroupName = "skinned";

            ModelName = "Models/FBX/Skinned/GamePawn/pawn_pose";
            Scale *= 0.05f;

            base.Initialise();

            // Create the game pawn controller
            Controller = new Node3DController(this);
            Controller.MovementSpeed = 50.0f;
            Controller.MaxVelocity = 30.0f;
            Controller.RotationVelocity = 2.5f;
            Controller.TerrainHeightOffset = 0.0f;

            // Set position
            SetPosition(new Vector3(0, 0, 50));

            // Create the spline
            m_Spline = new SplineAsset(("Player" + (int)m_PlayerIndex + "Spline"), true);
            m_Spline.Loop = false;

            // Don't use parent scale
            m_UseParentScale = false;
        }

        public override void Reset()
        {
            base.Reset();

            if (Spline != null)
            {
                // Stop any movement
                StopMovement();

                // Make sure spline is cleared
                Spline.ClearSpline();
            }

            if (m_GameBoard != null)
            {
                m_CurrentSquare = m_GameBoard.GetBoardSquare("Start");

                if (m_CurrentSquare != null)
                    Position = m_CurrentSquare.EnterSquare(this);
            }
        }

        public override void Load()
        {
            base.Load();

            if (RenderManager.Instance.RenderEngineType == RendererEngineType.Deferred)
            {
                Texture2D texture = null;

                if (m_DesiredTexture == "blue")
                    texture = m_BlueTexture;
                else if (m_DesiredTexture == "red")
                    texture = m_RedTexture;

                if (texture != null)
                {
                    if (m_Mesh != null)
                        m_Mesh.SetTexture(texture);
                }
            }

            // Show default animation
            ShowDefault();
        }

        protected override void LoadPostTagTextures()
        {
            // Load the face textures
            LoadColourTextures();
        }

        private void LoadColourTextures()
        {
            // RED TEXTURE
            String redTextureName = "red_playpiece";
            m_RedTexture = RenderManager.Instance.LoadTexture2D("Models/FBX/Skinned/GamePawn/" + redTextureName);

            // BLUE TEXTURE
            String blueTextureName = "blue_playpiece";
            m_BlueTexture = RenderManager.Instance.LoadTexture2D("Models/FBX/Skinned/GamePawn/" + blueTextureName);
        }
 
        public override void Update(GameTime deltaTime)
        {
            // Update base class where the main calculations occur.
            base.Update(deltaTime);
        }

        public void EndOfTurn()
        {
            if (m_Controller != null)
                m_Controller.Reset();
 
            // Change to idle anim
            ShowIdle();
        }

        public BoardSquare PreviousSquare()
        {
            // Commit player turn.
            BoardSquare previousSquare = m_CurrentSquare.PreviousSquare();

            if (previousSquare != null)
            {
                if (previousSquare.Trail != null)
                {
                    List<Vector3> trailPoints = new List<Vector3>();
                    trailPoints.AddRange(m_CurrentSquare.Trail.GetPoints());
    
                    if (trailPoints.Count > 0)
                    {
                        // Reverse the list so they can be added backwards for going backwards
                        trailPoints.Reverse();

                        if (Spline != null)
                        {
                            Spline.ClearSpline();
                            Spline.AddControlPoints(trailPoints, null);

                            // Set desired position to first point in the trail
                            SetDesiredPosition(Spline.GetNextControlPointPosition(Position));

                            // Leave current square
                            m_CurrentSquare.LeaveSquare(this);

                            // Set the current square as the selected next square
                            m_CurrentSquare = previousSquare;
                        }
                    }
                }
            }

            return previousSquare;
        }

        public BoardSquare NextSquare( BoardSquare.DecisionData data )
        {
            // Commit player turn.
            BoardSquare nextSquare = m_CurrentSquare.NextSquare(data.route);

            if (nextSquare != null)
            {
                // Leave current square
                m_CurrentSquare.LeaveSquare(this);

                // Begin trail collection
                BoardTrail trail = GetTrail(m_CurrentSquare, nextSquare);

                if (trail != null)
                {
                    List<Vector3> trailPoints = new List<Vector3>();
                    
                    // Add points
                    trailPoints.AddRange(trail.GetPoints());

                    for (int i = 0; i < trailPoints.Count; i++ )
                    {
                        Vector3 point = trailPoints[i];
                        trailPoints[i] = new Vector3(point.X, PosY, point.Z);
                    }

                    if (trailPoints.Count > 0)
                    {
                        // Find square positioning (replace last position of trail to the desired square location
                        trailPoints[trailPoints.Count - 1] = nextSquare.EnterSquare(this);

                        if (Spline != null)
                        {
                            Spline.ClearSpline();
                            Spline.AddControlPoints(trailPoints, null);

                            // Set desired position to first point in the trail
                            SetDesiredPosition(Spline.GetNextControlPointPosition(Position));

                            // Set the current square as the selected next square
                            m_CurrentSquare = nextSquare;
                        }
                    }
                }
            }

            return nextSquare;
        }

        public void ShowTopText()
        {
            String currentSquare = "";
            String nextSquareText = "";

            if (m_CurrentSquare != null)
            {
                currentSquare = m_CurrentSquare.GetSquareText();

                BoardSquare nextSquare = m_CurrentSquare.NextSquare(BoardSquare.Route.eDefault);

                if (nextSquare != null)
                    nextSquareText = nextSquare.GetSquareText();
            }

            UIManager.Instance.ChangeTextObject("currentsquaretext", currentSquare);
            UIManager.Instance.ChangeTextObject("nextsquaretext", nextSquareText);
        }

        public override void AddToScene(Scene scene)
        {
            base.AddToScene(scene);

            if (scene != null)
            {
                if (scene.GetType() == typeof(Scene3D))
                {
                    Scene3D scene3D = (scene as Scene3D);

                    if (scene3D.Hierarchy != null)
                    {
                        m_GameBoard = (scene3D.Hierarchy.RootNode.FindChild("GameBoard") as GameBoard);

                        if (m_GameBoard != null)
                            m_GameBoard.AddChild(this);
                    }
                }
            }
        }

        public BoardTrail GetTrail( BoardSquare currentSquare, BoardSquare nextSquare )
        {
            BoardTrail toRet = null;

            if ((currentSquare != null) && (nextSquare != null))
            {
                foreach (BoardTrail trail in nextSquare.Trails)
                {
                    if (currentSquare.Name == trail.Connection)
                        toRet = trail;
                }
            }

            return toRet;
        }

        #region Event Handlers
        public void Handle(SelectObjectEvent message)
        {
            /*Ray castedRay = message.CastedRay;
            float? intersection = castedRay.Intersects(m_Cube.BoundingBox);

            // If this objects sphere is hit then select it
            if (intersection != null)
            {
                Camera activeCamera = RenderManager.Instance.GetActiveCamera();

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
            }*/
        }
        #endregion

        #region Player Handler
        public void SetPlayer(PlayerIndex index)
        {
            switch (index)
            {
                case PlayerIndex.One:
                    {
                        ShowBlueTexture();
                    }
                    break;
                case PlayerIndex.Two:
                    {
                        ShowRedTexture();
                    }
                    break;
            }
        }
        #endregion

        #region Texture handler
        public void ShowRedTexture()
        {
            // Try change to the red texture
            ChangeTexture("red");
            ChangeSelectedParticleEffect("characterselectred");
            ChangeTrailParticleEffect("magictrailred");
        }

        public void ShowBlueTexture()
        {
            // Try change to the blue texture
            ChangeTexture("blue");
            ChangeSelectedParticleEffect("characterselectblue");
            ChangeTrailParticleEffect("magictrailblue");
        }

        public override void ChangeTexture(String textureName)
        {
            if (textureName != "")
                m_DesiredTexture = textureName;
        }

        public void ChangeTrailParticleEffect(String effectName)
        {
            if (effectName != "")
                m_TrailEffect = ParticleEngine.Instance.AddParticleSystem(effectName);
        }

        public void ChangeSelectedParticleEffect(String effectName)
        {
            if ( effectName != "" )
                m_SelectedEffect = ParticleEngine.Instance.AddParticleSystem(effectName);
        }
        #endregion

        #region Animation Handlers
        public float ShowDefault()
        {
            SetAnimationSpeed(1.0f);

            return PlayAnimation("idle", true);
        }

        public float ShowIdle() 
        {
            SetAnimationSpeed(3.0f);

            return PlayAnimation("idle", true); 
        }

        public float ShowMovement()
        {
            SetAnimationSpeed(4.0f);

            return PlayAnimation("jump", true);
        }
        public float ShowJump() 
        {
            SetAnimationSpeed(1.0f);

            return PlayAnimation("lose", false);
        }

        public float ShowReachedEnd()
        {
            SetAnimationSpeed(1.0f);

            return PlayAnimation("win", false);
        }

        public float ShowWon()
        {
            SetAnimationSpeed(1.0f);

            return PlayAnimation("win", true);
        }

        public float ShowLost()
        {
            SetAnimationSpeed(1.0f);

            return PlayAnimation("lose", true);
        }
        #endregion

        #region Particle Handlers
        public void ShowTrail()
        {
            if (m_TrailEffect != null)
            {
                Vector3 trailPos = (Position + new Vector3(0, 4, 0));

                // Trigger trail effect
                m_TrailEffect.Trigger(trailPos);
            }
        }

        public void ShowSelected()
        {
            if (m_SelectedEffect != null)
            {
                Vector3 pos = (Position + new Vector3(0, 6, 0));

                // Trigger effect
                m_SelectedEffect.Trigger(pos);
            }
        }

        #endregion

        #region Properties
        public RenderNode Cube { get { return this; } }
        public BoardSquare CurrentSquare { get { return m_CurrentSquare; } set { m_CurrentSquare = value; } }
        #endregion
    }
}
