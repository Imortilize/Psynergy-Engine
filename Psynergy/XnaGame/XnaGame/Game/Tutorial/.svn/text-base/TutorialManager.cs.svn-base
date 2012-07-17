using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Menus;
using Psynergy.Graphics;
using Psynergy.Input;

namespace XnaGame
{
    public enum TutorialStage
    {
        StartTutorial = 0,
        DeclareXTutorial = 1,
        EnterMain = 2,
        ArithmeticSquares = 3,
        IfSquare = 4,
        WhileSquare = 5,
        SwitchSquare = 6,
        Questions = 7
    };

    public class TutorialManager : Singleton<TutorialManager>
    {
        private SortedList<TutorialStage, Tutorial> m_TutorialsShown = new SortedList<TutorialStage, Tutorial>();
        private Tutorial m_CurrentTutorial = null;
        private Tutorial m_RemovingTutorial = null;

        private bool m_TutorialPause = false;

        public TutorialManager() : base()
        {
            AddTutorials();
        }

        public void AddTutorials()
        {
            // Add the tutorials that can be shown
            m_TutorialsShown.Add(TutorialStage.StartTutorial, new Tutorial("Tutorial1", "Tutorials/tutorial1", 1.0f));
            m_TutorialsShown.Add(TutorialStage.DeclareXTutorial, new Tutorial("Tutorial2", "Tutorials/tutorial2", 1.0f));
            m_TutorialsShown.Add(TutorialStage.EnterMain, new Tutorial("Tutorial3", "Tutorials/tutorial3", 1.0f));
            m_TutorialsShown.Add(TutorialStage.ArithmeticSquares, new Tutorial("Tutorial4", "Tutorials/tutorial4", 1.0f));
            m_TutorialsShown.Add(TutorialStage.IfSquare, new Tutorial("Tutorial5", "Tutorials/tutorial5", 1.0f));
            m_TutorialsShown.Add(TutorialStage.WhileSquare, new Tutorial("Tutorial6", "Tutorials/tutorial6", 1.0f));
            m_TutorialsShown.Add(TutorialStage.SwitchSquare, new Tutorial("Tutorial7", "Tutorials/tutorial7", 1.0f));
            m_TutorialsShown.Add(TutorialStage.Questions, new Tutorial("Tutorial8", "Tutorials/tutorial7", 1.0f));
        }

        public override void Initialise()
        {
            base.Initialise();

            foreach ( Tutorial tutorial in m_TutorialsShown.Values )
                tutorial.Initialise();
        }

        public override void  Reset()
        {
 	        base.Reset();

            if (m_CurrentTutorial != null)
            {
                UIManager.Instance.RemoveUIObject(m_CurrentTutorial);
                m_CurrentTutorial = null;
            }

            if (m_RemovingTutorial != null)
            {
                UIManager.Instance.RemoveUIObject(m_RemovingTutorial);
                m_RemovingTutorial = null;
            }

            // UnPause game
            m_TutorialPause = false;

            // UnPause rendering
            InputManager.Instance.PauseRendering(false);
        }

        public override void Load()
        {
            base.Load();

            foreach ( Tutorial tutorial in m_TutorialsShown.Values )
                tutorial.Load();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (m_CurrentTutorial != null)
            {
                m_CurrentTutorial.Update(deltaTime);

                if (m_CurrentTutorial.Close)
                {
                    // Not answered anymore
                    m_CurrentTutorial.Close = false;

                    // Remove tutorial
                    RemoveTutorial();
                }
            }
            else if (m_RemovingTutorial != null)
            {
                if (!UIManager.Instance.IsShowing(m_RemovingTutorial))
                {
                    m_RemovingTutorial = null;

                    // UnPause game
                    m_TutorialPause = false;

                    QuestionManager questions = QuestionManager.Instance;

                    // See if there are any questions to show
                    if (!questions.IsQuestionToShow() && !questions.IsOpen())
                    {
                        // UnPause rendering
                        InputManager.Instance.PauseRendering(false);

                        // Pause roll dice image for now
                        GamePlayer gamePlayer = (PlayerManager.Instance.ActivePlayer as GamePlayer);

                        if (gamePlayer != null)
                            gamePlayer.EnableSelectableUIObjects();
                    }
                    else
                    {
                        questions.Show();
                    }
                }
            }
        }

        public void ShowTutorial(TutorialStage tutorial, Vector3 pos, UIManager.TextureAlignment alignment )
        {
            int stage = (int)tutorial;

            if ( stage < m_TutorialsShown.Count )
            {
                int index = m_TutorialsShown.IndexOfKey(tutorial);

                if ( index >= 0 )
                {
                    Tutorial tutorialImage = m_TutorialsShown.Values[index];

                    // Tutorial has been shown now
                    tutorialImage.Shown = true;

                    if ( tutorialImage != null )
                    {
                        // Add to the UI manager
                        UIManager.Instance.AddUIObject(tutorialImage, pos, alignment);

                        // Remove any current tutorial
                        RemoveTutorial();

                        // Set current tutorial
                        m_CurrentTutorial = tutorialImage;

                        // Pause game
                        m_TutorialPause = true;

                        // UnPause rendering
                        InputManager.Instance.PauseRendering(true);

                        // Pause roll dice image for now
                        GamePlayer gamePlayer = (PlayerManager.Instance.ActivePlayer as GamePlayer);
                        
                        if ( gamePlayer != null )
                            gamePlayer.DisableSelectableUIObjects();
                    }
                }
            }
        }

        public void RemoveTutorial()
        {
            if ( m_CurrentTutorial != null )
            {
                // Add to the UI manager
                UIManager.Instance.FlagToRemoveUIObject(m_CurrentTutorial.Name);

                // Set this tutorial to the removing tutorial variable
                m_RemovingTutorial = m_CurrentTutorial;

                // Null the current tutorial 
                m_CurrentTutorial = null;
            }
        }

        public bool IsTutorialOpen()
        {
            return (m_CurrentTutorial != null) || (m_RemovingTutorial != null);
        }

        public bool HasTutorialBeenShown(TutorialStage stage)
        {
            bool toRet = false;

            int index = m_TutorialsShown.IndexOfKey(stage);

            if (index >= 0)
                toRet = m_TutorialsShown.Values[index].Shown;

            return toRet;
        }

        #region Property Set / Gets
        public bool TutorialPause { get { return m_TutorialPause; } set { m_TutorialPause = value; } }
        #endregion
    }
}
