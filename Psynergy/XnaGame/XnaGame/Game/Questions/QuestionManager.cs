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
    public class QuestionManager : Singleton<QuestionManager>
    {
        private QuestionResource m_QuestionResource = null;

        private SortedList<int, Question> m_Questions = new SortedList<int, Question>();
        private Question m_Show = null;
        private Question m_Current = null;
        private Question m_Previous = null;
        private Question m_Removing = null;

        private bool m_Pause = false;

        public QuestionManager() : base()
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            String filename = "Resources/Game/Questions.xml";

            // Load scene fragment
            m_QuestionResource = new QuestionResource(filename);

            // Add questions
            //AddQuestion(new Question("Question1", "Tutorials/tutorial1", 1.0f));

            //foreach (Question question in m_Questions.Values)
                //question.Initialise();
        }

        public override void Load()
        {
            base.Load();

            if (m_QuestionResource != null)
                m_QuestionResource.Load();

            List<Question> questions = m_QuestionResource.Questions;

            // Add questions from the resource
            foreach (Question question in questions)
                m_Questions.Add(m_Questions.Count, question);
        }

        public override void Reset()
        {          
            base.Reset();

            foreach (Question question in m_Questions.Values)
                question.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (m_Show != null)
            {
                if (!TutorialManager.Instance.IsTutorialOpen())
                    Show();
            }
            else if (m_Current != null)
            {
                m_Current.Update(deltaTime);

                if (m_Current.QuestionData.Answered)
                {
                    // Remove
                    Remove();
                }
            }
            else if (m_Removing != null)
            {
                if (!UIManager.Instance.IsShowing(m_Removing))
                {
                    // Null the removing question
                    m_Removing = null;

                    // UnPause game
                    m_Pause = false;

                    // UnPause rendering
                    InputManager.Instance.PauseRendering(false);

                    // Pause roll dice image for now
                    GamePlayer gamePlayer = (PlayerManager.Instance.ActivePlayer as GamePlayer);

                    if (gamePlayer != null)
                        gamePlayer.EnableSelectableUIObjects();
                }
            }
        }

        public void AddQuestion(Question question)
        {
            m_Questions.Add(m_Questions.Count, question);
        }

        public void ShowQuestion()
        {
            Random randomHelper = new Random();

            List<Question> unShownQuestons = new List<Question>();

            foreach (Question question in m_Questions.Values)
            {
                if (!question.Shown)
                    unShownQuestons.Add(question);
            }

            // Check that some answers were added
            if (unShownQuestons.Count == 0)
            {
                // Reset the questions
                foreach (Question question in m_Questions.Values)
                {
                    question.Reset();
                    question.Shown = false;

                    // Add the question
                    unShownQuestons.Add(question);
                }
            }

            int number = randomHelper.Next(0, unShownQuestons.Count);

            if (number < unShownQuestons.Count)
            {
                Question questionToShow = unShownQuestons[number];
                int questionIndex = m_Questions.IndexOfValue(questionToShow);

                if ( questionIndex >= 0 )
                    FlagToShow(questionIndex);
            }
        }

        private void FlagToShow(int question)
        {
            int stage = question;

            if (stage < m_Questions.Count)
            {
                int index = m_Questions.IndexOfKey(question);

                if (index >= 0)
                {
                    Question image = m_Questions.Values[index];

                    // Tutorial has been shown now
                    image.Shown = true;

                    if (image != null)
                        m_Show = image;
                }
            }
        }

        public void Show()
        {
            if (IsQuestionToShow())
            {
                // Add to the UI manager
                UIManager.Instance.AddUIObject(m_Show, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);

                // Add the answer textures
                m_Show.ShowAnswerTextures();

                // Remove any current tutorial
                Remove();

                // Set current tutorial
                m_Current = m_Show;

                // Remove that the question is to show
                m_Show = null;

                // Pause game
                m_Pause = true;

                // UnPause rendering
                InputManager.Instance.PauseRendering(true);

                // Pause roll dice image for now
                GamePlayer gamePlayer = (PlayerManager.Instance.ActivePlayer as GamePlayer);

                if (gamePlayer != null)
                    gamePlayer.DisableSelectableUIObjects();
            }
        }

        public void Remove()
        {
            if (m_Current != null)
            {
                // Add to the UI manager
                UIManager.Instance.FlagToRemoveUIObject(m_Current.Name);

                // remove the answer textures
                m_Current.RemoveAnswerTextures();

                // Set this tutorial to the removing tutorial variable
                m_Previous = m_Removing = m_Current;

                // Null the current tutorial 
                m_Current = null;
            }
        }

        public bool IsOpen()
        {
            return (m_Current != null) || (m_Removing != null); ;
        }

        public bool HasBeenShown(int stage)
        {
            bool toRet = false;

            int index = m_Questions.IndexOfKey(stage);

            if (index >= 0)
                toRet = m_Questions.Values[index].Shown;

            return toRet;
        }

        public bool IsQuestionToShow()
        {
            return (m_Show != null);
        }

        public bool IsQuestionAnswered()
        {
            bool toRet = false;

            if (m_Previous != null)
                toRet = m_Previous.QuestionData.Answered;

            return toRet;
        }

        public bool QuestionAnswer()
        {
            bool toRet = false;

            if (m_Previous != null)
                toRet = m_Previous.QuestionData.Correct;

            return toRet;
        }

        #region Property Set / Gets
        public bool Pause { get { return m_Pause; } set { m_Pause = value; } }
        #endregion
    }
}
