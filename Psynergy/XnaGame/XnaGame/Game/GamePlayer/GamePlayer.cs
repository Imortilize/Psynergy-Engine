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

/* Psynergy Engine*/
using Psynergy;
using Psynergy.Camera;
using Psynergy.Graphics;
using Psynergy.AI;
using Psynergy.Events;
using Psynergy.Menus;

namespace XnaGame
{
    public class GamePlayer : Player, IListener<PlayerWonEvent>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        public enum PlayerColor
        {
            Blue = 0,
            Red = 1
        };

        public enum UIInterfaceObject
        {
            PlayerIndicator = 0,
            PlayerIndicator2 = 1,
            PlayerIndicator3 = 2,
            Pawn1 = 3,
            Pawn2 = 4,
            RollDice = 5,
            TopBar = 6
        };

        // Game board variables
        private GameBoard m_GameBoard = null;

        // Dice rolling variables
        private int m_PreDecisionRoll = 0;
        private int m_OriginalDiceRoll = 0;
        private int m_CurrentDiceRoll = 0;
        private BoardSquare.DecisionData m_Data;

        // Game Pawns
        private List<GamePawn> m_Pawns = new List<GamePawn>();
        private GamePawn m_ActivePawn = null;

        // UI Interface objects related to this player
        private SortedList<UIInterfaceObject, String> m_UIInterfaceObjects = new SortedList<UIInterfaceObject, String>();

        // Camera modification value
        private float m_CameraModifier = 1;
        private Random m_CameraModifierGenerator = new Random();

        // state machine
        protected String m_StateMachineName = "";
        protected StateMachine<GamePlayer> m_StateMachine = null;

        protected bool m_Selected = false;

        // Player colour
        PlayerColor m_PlayerColour = PlayerColor.Blue;

        // For now each GamePlayer can have their own dice
        private DiceJigLibX m_Dice = null;

        private int m_PawnsAtEnd = 0;

        public GamePlayer() : base("")
        {
            m_Pawns.Add(new GamePawn("Pawn1"));
            m_Pawns.Add(new GamePawn("Pawn2"));
        }

        public GamePlayer(String name) : base(name)
        {
            m_Pawns.Add(new GamePawn(name + "Pawn1"));
            m_Pawns.Add(new GamePawn(name + "Pawn2"));
        }

        public override void Initialise()
        {
            // Create the game pawn controller
            Node3DController controller = new Node3DController(this);
            controller.MovementSpeed = 8.0f;
            controller.MaxVelocity = 20.0f;
            controller.TerrainHeightOffset = 0.0f;

            // Save controller reference
            Controller = controller;

            // Set position
            SetPosition(new Vector3(0, 0, 50));

            // Create the spline
            m_Spline = new SplineAsset(("Player" + (int)m_PlayerIndex + "Spline"), true);
            m_Spline.Loop = false;

            foreach (GamePawn pawn in m_Pawns)
            {
                pawn.Initialise();

                // Set player index properties on their game pawns
                pawn.SetPlayer(m_PlayerIndex);

            }

            // Player colour is same as player index but easier enum
            m_PlayerColour = (PlayerColor)m_PlayerIndex;

            String playerColor = "";

            if (m_PlayerColour == PlayerColor.Blue)
                playerColor = "blue";
            else if (m_PlayerColour == PlayerColor.Red)
                playerColor = "red";

            // Add user interface references
            m_UIInterfaceObjects.Add(UIInterfaceObject.PlayerIndicator, (playerColor + "playerindicator"));
            m_UIInterfaceObjects.Add(UIInterfaceObject.PlayerIndicator2, (playerColor + "playerindicator2"));
            m_UIInterfaceObjects.Add(UIInterfaceObject.PlayerIndicator3, (playerColor + "playerindicator3"));
            m_UIInterfaceObjects.Add(UIInterfaceObject.Pawn1, (playerColor + "pawn1"));
            m_UIInterfaceObjects.Add(UIInterfaceObject.Pawn2, (playerColor + "pawn2"));
            m_UIInterfaceObjects.Add(UIInterfaceObject.RollDice, "rolldice");
            m_UIInterfaceObjects.Add(UIInterfaceObject.TopBar, "topbar");

            // Create and initialise state machine
            m_StateMachineName = "Resources/StateMachines/GamePlayerStates.xml";
            CreateStateMachine(m_StateMachineName);

            if (m_StateMachine != null)
                m_StateMachine.Initialise();

            // Not selected
            m_Selected = false;

            // Create dice
            m_Dice = new DiceJigLibX("player" + (int)(m_PlayerIndex + 1) + "dice");
            m_Dice.Initialise();

            base.Initialise();
        }

        public override void Reset()
        {
            base.Reset();

            m_PreDecisionRoll = m_OriginalDiceRoll = m_CurrentDiceRoll = 0;

            foreach (GamePawn pawn in m_Pawns)
                pawn.Reset();

            m_CameraModifier = 1;

            // Zoom camera out
            ZoomOut();

            if (m_StateMachine != null)
                m_StateMachine.Reset();

            if (m_Dice != null)
                m_Dice.Reset();

            m_PawnsAtEnd = 0;
            m_Selected = false;

            // Make sure player indicator 2 and 3 are removed
            UIManager.Instance.FlagToRemoveUIObject(GetUIInterfaceObjectName(UIInterfaceObject.PlayerIndicator2));
            UIManager.Instance.FlagToRemoveUIObject(GetUIInterfaceObjectName(UIInterfaceObject.PlayerIndicator3));

            // Now remove them
            //RemoveUIInterfaceObjects();

            if (m_PlayerIndex == PlayerIndex.One)
            {
                // Add the Top bar itself
                UIManager.Instance.AddUIObject(GetUIInterfaceObjectName(UIInterfaceObject.TopBar), new Vector3(0, 0, 0), UIManager.TextureAlignment.eTopLeft);

                // Add top texts
                UIManager.Instance.AddTextObject("dicerolltext", "Test", new Vector2(270, 80), Microsoft.Xna.Framework.Color.Black, 1.5f);
                UIManager.Instance.AddTextObject("currentsquaretext", "Test", new Vector2(585, 80), Microsoft.Xna.Framework.Color.Black, 1.5f);
                UIManager.Instance.AddTextObject("nextsquaretext", "Test", new Vector2(960, 80), Microsoft.Xna.Framework.Color.Black, 1.5f);

                // Make sure player indicator is showing
                UIManager.Instance.AddUIObject(GetUIInterfaceObjectName(UIInterfaceObject.PlayerIndicator), new Vector3(4, 22, 0), UIManager.TextureAlignment.eTopLeft);
            }
            else if (m_PlayerIndex == PlayerIndex.Two)
            {
                Viewport viewPort = RenderManager.Instance.GraphicsDevice.Viewport;

                // Make sure player indicator is showing
                UIManager.Instance.AddUIObject(GetUIInterfaceObjectName(UIInterfaceObject.PlayerIndicator), new Vector3(viewPort.Width - 4, 22, 0), UIManager.TextureAlignment.eTopRight);
            }

            // UnFocus the player object to start
            UnFocusUIInterfaceObject(UIInterfaceObject.PlayerIndicator);
        }

        public override void Load()
        {
            base.Load();

            foreach (GamePawn pawn in m_Pawns)
                pawn.Load();

            if (m_StateMachine != null)
                m_StateMachine.Load();

            if ( m_Dice != null )
                m_Dice.Load();

            // Load UI OBJECT ( UI stuff will change eventually, its a mess atm )
            UIManager.Instance.LoadTexture(("player" + ((int)m_PlayerIndex + 1) + "text"), ("player" + ((int)m_PlayerIndex + 1) + "text"));

            String playerColor = "";

            if (m_PlayerColour == PlayerColor.Blue)
                playerColor = "blue";
            else if (m_PlayerColour == PlayerColor.Red)
                playerColor = "red";

            UIManager.Instance.LoadTexture((playerColor + "playerindicator"), (playerColor + "playerindicator"), Vector3.Zero, new UIObjectEffectFadeInFadeOut(1.0f), 0.5f);
            UIManager.Instance.LoadTexture((playerColor + "playerindicator2"), (playerColor + "playerindicator2"), Vector3.Zero, new UIObjectEffectFadeInFadeOut(1.0f), 0.49f);
            UIManager.Instance.LoadTexture((playerColor + "playerindicator3"), (playerColor + "playerindicator3"), Vector3.Zero, new UIObjectEffectFadeInFadeOut(1.0f), 0.48f);
            UIManager.Instance.LoadSelectableTexture(playerColor + "pawn1", "arrowleft", "arrowleftselected", new MenuActionPreviousPawn(), new UIObjectEffectFadeInFadeOut(1.0f));
            UIManager.Instance.LoadSelectableTexture(playerColor + "pawn2", "arrowright", "arrowrightselected", new MenuActionNextPawn(), new UIObjectEffectFadeInFadeOut(1.0f));
            UIManager.Instance.LoadSelectableTexture("rolldice", "rolldice", "rolldiceselected", new MenuActionRollDice(), new UIObjectEffectFadeInFadeOut(1.0f));
            
            if ( m_PlayerIndex == PlayerIndex.One )
                UIManager.Instance.LoadTexture("topbar", "topbar", Vector3.Zero, new UIObjectEffectFadeInFadeOut(1.0f), 1.0f);
        }

        /* Abstract creation of state machine */
        protected void CreateStateMachine(string fileName)
        {
            Debug.Assert(fileName != "", "[WARNING] - State machine file name should be empty!");

            // Create the state machine
            m_StateMachine = new StateMachine<GamePlayer>("StateMachine - " + m_Name, this, fileName);
        }

        public override void OnSelect()
        {
            base.OnSelect();

            // change to the selected state
            ChangeState_Selected();
        }

        public override void OnDeselect()
        {
            base.OnDeselect();

            // change to the selected state
            ChangeState_Deselected();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Update the current state 
            if (m_StateMachine != null)
                m_StateMachine.Update(deltaTime);

            // Update pawns
            //foreach (GamePawn pawn in m_Pawns)
                //pawn.Update(deltaTime);

            // Update dice ( if applicable )
            if (m_Dice != null)
                m_Dice.Update(deltaTime);

            // If in active rotate camera for now
            UpdateCamera(deltaTime);

            if (Selected)
            {
                String diceRoll = "--";

                if (m_PreDecisionRoll != 0)
                    diceRoll = m_PreDecisionRoll.ToString();

                UIManager.Instance.ChangeTextObject("dicerolltext", diceRoll);

                if (m_ActivePawn != null)
                    m_ActivePawn.ShowTopText();
            }
        }

        private void UpdateCamera(GameTime deltaTime)
        {
            if (Selected)
            {
                if (m_ActivePawn != null)
                {
                    if (CameraManager.Instance.ActiveCamera != null)
                    {
                        if (CameraManager.Instance.ActiveCamera.GetType() == typeof(FixedThirdPersonCamera))
                        {
                            float delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                            FixedThirdPersonCamera fixedCamera = (CameraManager.Instance.ActiveCamera as FixedThirdPersonCamera);

                            if (m_CurrentDiceRoll == 0)
                            {
                                // Rotate camera slowly
                                fixedCamera.IdleRotation(0.2f * m_CameraModifier * delta);
                            }
                            else
                            {
                                if (fixedCamera.FollowRotation(m_CameraModifier, delta))
                                {
                                    // Generate new camera modifier
                                    GenerateCameraModifier(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void BeginRollDice()
        {
            if (ActivePawn.CurrentSquare != null)
            {
                if ((Pawns.Count - PawnsAtEnd) > 1)
                {
                    // Remove player ui objects
                    RemoveUIInterfaceObject(GamePlayer.UIInterfaceObject.Pawn1);
                    RemoveUIInterfaceObject(GamePlayer.UIInterfaceObject.Pawn2);
                }

                // Remove roll dice button 
                RemoveUIInterfaceObject(GamePlayer.UIInterfaceObject.RollDice);

                // Change to the roll dice state
                ChangeState_RollDice();
            }
        }

        public void RollDice()
        {
            if (m_Dice != null)
            {
                // Set camera focus to the dice
                BaseCamera camera = CameraManager.Instance.ActiveCamera;

                if (camera != null)
                {
                    // Find a set distance in front of the camera 
                    Vector3 camPos = camera.Position;
                    Vector3 upVec = camera.Transform.Up;
                    Vector3 forwardVec = camera.Transform.Forward;
                    upVec.Normalize();
                    forwardVec.Normalize();

                    // Reset dice
                    m_Dice.Reset();

                    // use the cam position and normalised forward vec to determine
                    // dice start position
                    Vector3 startDicePos = camPos + (forwardVec * 100) + (upVec * 10);

                    // Set position
                    m_Dice.Position = startDicePos;

                    // Apply impulse
                    Vector3 impulse = (forwardVec * 350);
                    m_Dice.ApplyImpulse(impulse);

                    // Set start rot
                    Matrix diceRot = Matrix.CreateFromQuaternion(m_Dice.Rotation);
                    diceRot *= Matrix.CreateRotationZ(MathHelper.ToRadians(45));
                    m_Dice.Body.Orientation = diceRot;

                    // Set dice enabled
                    m_Dice.Enable();

                    // Set the dice as the camera focus
                    camera.SetFocus(m_Dice);

                    // Zoom in at a higher view
                    ZoomInTopView();
                }
            }
        }

        public void MoveToNextSquare()
        {
            if (ActivePawn != null)
            {
                if (ActivePawn.CurrentSquare != null)
                {
                    if (ActivePawn.CurrentSquare.GetType() != typeof(EndSquare))
                    {
                        // Then it is at the end of the set spline
                        // ( for now run the next square code again )
                        if (ActivePawn.NextSquare(Data) == null)
                        {
                            ChangeState_EndOfTurn();
                        }
                        else
                        {
                            // The game pawn wants to always move at full speed
                            //Controller.Velocity = Controller.MaxVelocity;

                            // Now find next spline position
                            //player.ActivePawn.SetNextSplinePosition(deltaTime);
                        }

                        Data.Reset();
                    }
                    else
                    {
                        // End of board
                        ChangeState_EndOfBoard();
                    }
                }
            }    
        }

        public void MoveToPreviousSquare()
        {
            // Then it is at the end of the set spline
            // ( for now run the next square code again )
            if (ActivePawn.PreviousSquare() == null)
            {
                ChangeState_EndOfTurn();
            }
            else
            {
                //player.ActivePawn.SetNextSplinePosition(deltaTime);
            }

            Data.Reset();
        }

        private void RenderState(GameTime deltaTime)
        {
            // Render the current state
            if (m_StateMachine != null)
                m_StateMachine.Render(deltaTime);
        }

        public override void AddToScene(Scene scene)
        {
            if (scene != null)
            {
                if (scene.GetType() == typeof(Scene3D))
                {
                    Scene3D scene3D = (scene as Scene3D);

                    foreach (GamePawn pawn in m_Pawns)
                        pawn.AddToScene(scene3D);

                    // Save which scene it has been added to
                    m_SceneAddedTo = scene3D;

                    if (scene3D.Hierarchy != null)
                    {
                        m_GameBoard = (scene3D.Hierarchy.RootNode.FindChild("GameBoard") as GameBoard);

                        if (m_GameBoard != null)
                            m_GameBoard.AddChild(this);
                    }
                }
            }
        }

        public override void RemoveFromScene()
        {
            if (m_SceneAddedTo != null)
            {
                // Add this way for now
                foreach (GamePawn pawn in m_Pawns)
                    pawn.RemoveFromScene();

                base.RemoveFromScene();
            }
        }

        public BoardTrail GetTrail(BoardSquare currentSquare, BoardSquare nextSquare)
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

        public override IFocusable3D GetFocus()
        {
            IFocusable3D focus = null;

            if (m_Pawns.Count > 0)
                focus = m_Pawns[0];

            // Overrite if an active pawn
            if (m_ActivePawn != null)
                focus = m_ActivePawn;

            return focus;
        }

        public void GenerateCameraModifier(bool follow)
        {
            int value = m_CameraModifierGenerator.Next(0, 10);

            if (value < 5)
            {
                if (!follow)
                    m_CameraModifier = 1.0f;
                else
                {
                    if (value < 2)
                        m_CameraModifier = 1.0f;
                    else
                        m_CameraModifier = 2.0f;

                    if ( ActivePawn != null )
                        m_CameraModifier *= ActivePawn.Rotation.Y;
                }
            }
            else
            {
                if (!follow)
                    m_CameraModifier = -1.0f;
                else
                {
                    if (value < 7)
                        m_CameraModifier = 4.0f;
                    else
                        m_CameraModifier = 6.0f;

                    if (ActivePawn != null)
                        m_CameraModifier *= ActivePawn.Rotation.Y;
                }
            }
        }

        public void GenerateDecisionData()
        {
            // Get the square decision data
            if (m_ActivePawn != null)
            {
                if ( m_ActivePawn.CurrentSquare != null )
                    m_Data = m_ActivePawn.CurrentSquare.Decision(m_PreDecisionRoll);
            }
        }

        public void ZoomIn()
        {
            BaseCamera camera = CameraManager.Instance.ActiveCamera;

            if (camera.GetType().IsSubclassOf(typeof(Camera3D)))
            {
                if (camera.GetType() == typeof(FixedThirdPersonCamera))
                {
                    FixedThirdPersonCamera fixedThirdPersonCamera = (camera as FixedThirdPersonCamera);
                    fixedThirdPersonCamera.DesiredDistance = 75;
                    fixedThirdPersonCamera.DesiredPitch = 345;
                    fixedThirdPersonCamera.Tween = true;
                }
            }
        }

        public void ZoomInTopView()
        {
            BaseCamera camera = CameraManager.Instance.ActiveCamera;

            if (camera.GetType().IsSubclassOf(typeof(Camera3D)))
            {
                if (camera.GetType() == typeof(FixedThirdPersonCamera))
                {
                    FixedThirdPersonCamera fixedThirdPersonCamera = (camera as FixedThirdPersonCamera);
                    fixedThirdPersonCamera.DesiredDistance = 75;
                    fixedThirdPersonCamera.DesiredPitch = 300;
                    fixedThirdPersonCamera.Tween = true;
                }
            }
        }

        public void ZoomInHalfWay()
        {
            BaseCamera camera = CameraManager.Instance.ActiveCamera;

            if (camera.GetType().IsSubclassOf(typeof(Camera3D)))
            {
                if (camera.GetType() == typeof(FixedThirdPersonCamera))
                {
                    FixedThirdPersonCamera fixedThirdPersonCamera = (camera as FixedThirdPersonCamera);
                    fixedThirdPersonCamera.DesiredDistance = 125;
                    fixedThirdPersonCamera.DesiredPitch = 315;
                    fixedThirdPersonCamera.Tween = true;
                }
            }
        }

        public void ZoomOut()
        {
            BaseCamera camera = CameraManager.Instance.ActiveCamera;

            if (camera != null)
            {
                if (camera.GetType().IsSubclassOf(typeof(Camera3D)))
                {
                    if (camera.GetType() == typeof(FixedThirdPersonCamera))
                    {
                        FixedThirdPersonCamera fixedThirdPersonCamera = (camera as FixedThirdPersonCamera);
                        fixedThirdPersonCamera.DesiredDistance = 175;
                        fixedThirdPersonCamera.DesiredPitch = 345;
                        fixedThirdPersonCamera.Tween = true;
                    }
                }
            }
        }

        public void SetActivePawn( int index )
        {
            if (index >= 0)
                m_ActivePawn = m_Pawns[index];
            else
                m_ActivePawn = null;
        }

        public String GetUIInterfaceObjectName(UIInterfaceObject interfaceObject)
        {
            String toRet = "";

            if (m_UIInterfaceObjects.ContainsKey(interfaceObject))
            {
                int index = m_UIInterfaceObjects.IndexOfKey(interfaceObject);

                // Get the value
                toRet = m_UIInterfaceObjects.Values[index];
            }

            return toRet;
        }

        public void FocusUIInterfaceObject(UIInterfaceObject interfaceObject)
        {
            int index = m_UIInterfaceObjects.IndexOfKey(interfaceObject);

            if (index >= 0)
                UIManager.Instance.FocusUIObject(m_UIInterfaceObjects.Values[index]);
        }

        public void UnFocusUIInterfaceObject(UIInterfaceObject interfaceObject)
        {
            int index = m_UIInterfaceObjects.IndexOfKey(interfaceObject);

            if (index >= 0)
                UIManager.Instance.UnFocusUIObject(m_UIInterfaceObjects.Values[index]);
        }

        public void RemoveUIInterfaceObject( UIInterfaceObject interfaceObject )
        {
            int index = m_UIInterfaceObjects.IndexOfKey(interfaceObject);

            if (index >= 0)
                UIManager.Instance.FlagToRemoveUIObject(m_UIInterfaceObjects.Values[index]);
        }

        public void RemoveUIInterfaceObjects()
        {
            foreach (String interfaceObject in m_UIInterfaceObjects.Values)
                UIManager.Instance.FlagToRemoveUIObject(interfaceObject);
        }

        public void SetUIObjectState(String name, UIObject.ActiveState activeState)
        {
            UIObject uiObject = UIManager.Instance.GetUIObject(name);

            if ( uiObject != null )
                uiObject.SetActiveState(activeState);
        }

        public void SetUIObjectToPreviousState(String name)
        {
            UIObject uiObject = UIManager.Instance.GetUIObject(name);

            if (uiObject != null)
                uiObject.GotoPreviousState();
        }

        public void DisableSelectableUIObjects()
        {
            foreach (String interfaceObject in m_UIInterfaceObjects.Values)
            {
                UIObject uiObject = UIManager.Instance.GetUIObject(interfaceObject);

                if (uiObject != null)
                {
                    if ( uiObject.GetType() == typeof(UIObjectSelectable) )
                        uiObject.SetActiveState(UIObject.ActiveState.eInActive);
                }
            }
        }

        public void EnableSelectableUIObjects()
        {
            foreach (String interfaceObject in m_UIInterfaceObjects.Values)
            {
                UIObject uiObject = UIManager.Instance.GetUIObject(interfaceObject);

                if (uiObject != null)
                {
                    if ( uiObject.GetType() == typeof(UIObjectSelectable) )
                        uiObject.GotoPreviousState();
                }
            }
        }

        public void FocusNextPawn()
        {
            int currentIndex = -1;

            if ( m_Pawns.Count > 1 )
            {
                if (m_ActivePawn != null)
                {
                    currentIndex = m_Pawns.IndexOf(m_ActivePawn);

                    if (currentIndex > -1)
                    {
                        int newIndex = currentIndex + 1;

                        // Reset to the start pawn
                        if (newIndex >= m_Pawns.Count)
                            newIndex = 0;

                        // Set active pawn
                        SetActivePawn(newIndex);

                        // Tell player manager to set the camera focus nicely
                        PlayerManager.Instance.SetCameraFocus(m_PlayerIndex);
                    }
                }
            }
        }

        public void ShowFirstPawnAtEnd()
        {
            if ( m_PlayerIndex == PlayerIndex.One )
                UIManager.Instance.AddUIObject(GetUIInterfaceObjectName(UIInterfaceObject.PlayerIndicator2), new Vector3(4, 22, 0), UIManager.TextureAlignment.eTopLeft);
            else if (m_PlayerIndex == PlayerIndex.Two)
            {
                if (m_GraphicsDevice != null)
                {
                    Viewport viewPort = m_GraphicsDevice.Viewport;

                    UIManager.Instance.AddUIObject(GetUIInterfaceObjectName(UIInterfaceObject.PlayerIndicator2), new Vector3(viewPort.Width - 4, 22, 0), UIManager.TextureAlignment.eTopRight);
                }
            }
        }

        public void ShowSecondPawnAtEnd()
        {
            if (m_PlayerIndex == PlayerIndex.One)
                UIManager.Instance.AddUIObject(GetUIInterfaceObjectName(UIInterfaceObject.PlayerIndicator3), new Vector3(4, 22, 0), UIManager.TextureAlignment.eTopLeft);
            else if (m_PlayerIndex == PlayerIndex.Two)
            {
                if (m_GraphicsDevice != null)
                {
                    Viewport viewPort = m_GraphicsDevice.Viewport;

                    UIManager.Instance.AddUIObject(GetUIInterfaceObjectName(UIInterfaceObject.PlayerIndicator3), new Vector3(viewPort.Width - 4, 22, 0), UIManager.TextureAlignment.eTopRight);
                }
            }
        }

        #region Event handlers
        public virtual void Handle(PlayerWonEvent message)
        {
            PlayerIndex playerWonIndex = message.Index;

            if (playerWonIndex == m_PlayerIndex)
            {
                // This player has won
                ChangeState_Won();
            }
            else
            {
                // This player has lost
                ChangeState_Lost();
            }
        }
        #endregion

        #region State Controllers
        public void ChangeState_Idle() { if ( StateMachine != null ) StateMachine.ChangeState("Idle"); }
        public void ChangeState_Selected() { if (StateMachine != null) StateMachine.ChangeState("Selected"); }
        public void ChangeState_Deselected() { if (StateMachine != null) StateMachine.ChangeState("Deselected"); }
        public void ChangeState_PreMovement() { if (StateMachine != null) StateMachine.ChangeState("PreMovement"); }
        public void ChangeState_Movement() { if (StateMachine != null) StateMachine.ChangeState("Movement"); }
        public void ChangeState_EndOfTurn() { if (StateMachine != null) StateMachine.ChangeState("EndOfTurn"); }
        public void ChangeState_Jump() { if (StateMachine != null) StateMachine.ChangeState("Jump"); }
        public void ChangeState_RollDice() { if (StateMachine != null) StateMachine.ChangeState("RollDice"); }
        public void ChangeState_EndOfBoard() { if (StateMachine != null) StateMachine.ChangeState("EndOfBoard"); }
        public void ChangeState_Won() { if (StateMachine != null) StateMachine.ChangeState("Won"); }
        public void ChangeState_Lost() { if (StateMachine != null) StateMachine.ChangeState("Lost"); }
        public void ChangeState_Question() { if (StateMachine != null) StateMachine.ChangeState("Question"); }
        #endregion

        #region Properties
        public StateMachine<GamePlayer> StateMachine { get { return m_StateMachine; } }
        //public String StartTurnTextureName { get { return m_StartTurnTextureName; } }
        public List<GamePawn> Pawns { get { return m_Pawns; } }
        public GamePawn ActivePawn { get { return m_ActivePawn; } }
        public int PreDecisionRoll { get { return m_PreDecisionRoll; } set { m_PreDecisionRoll = value; } }
        public int CurrentDiceRoll { get { return m_CurrentDiceRoll; } set { m_CurrentDiceRoll = value; } }
        public int OriginalDiceRoll { get { return m_OriginalDiceRoll; } set { m_OriginalDiceRoll = value; } }
        public BoardSquare.DecisionData Data { get { return m_Data; } set { m_Data = value; } }
        public bool Selected { get { return m_Selected; } set { m_Selected = value; } }
        public DiceJigLibX Dice { get { return m_Dice; } }
        public int PawnsAtEnd { get { return m_PawnsAtEnd; } set { m_PawnsAtEnd = value; } }
        public PlayerColor Color { get { return m_PlayerColour; } }
        #endregion
    }
}
