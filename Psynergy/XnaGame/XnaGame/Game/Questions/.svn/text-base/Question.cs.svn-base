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
    public struct QuestionData
    {
        public void Reset()
        {
            Answered = false;
            Correct = false;
        }

        public void SetAnswer(bool answer)
        {
            Answered = true;
            Correct = answer;
        }

        public bool Answered { get; set; }
        public bool Correct { get; set; }
    };

    public class Question : UIObject
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterInt("answer", "Answer");
            factory.RegisterString("answer1texture", "Answer1Texture");
            factory.RegisterString("answer1selectedtexture", "Answer1SelectedTexture");
            factory.RegisterString("answer2texture", "Answer2Texture");
            factory.RegisterString("answer2selectedtexture", "Answer2SelectedTexture");

            base.ClassProperties(factory);
        }
        #endregion

        private bool m_Shown = false;
        private QuestionData m_QuestionData = new QuestionData();
        private int m_Answer = 0;
        private String m_Answer1 = "";
        private String m_Answer1Selected = "";
        private String m_Answer2 = "";
        private String m_Answer2Selected = "";

        public Question() : base("")
        {
        }

        public Question(String name) : base(name)
        {
        }

        public Question(String name, String textureName, float fadeRate) : base(name, textureName, Vector3.Zero)
        {
            UIObjectEffectFadeInFadeOut effect = new UIObjectEffectFadeInFadeOut(fadeRate);

            // Set the effect
            effect.SetUIObject(this);
        }

        public override void Initialise()
        {
            base.Initialise();

            // So they render above other UI objects
            m_RenderDepth = 0.1f;
        }

        public override void Reset()
        {
            base.Reset();

            // Reset question data
            QuestionData.Reset();
        }

        public override void Load()
        {
            base.Load();

            // Load question answer items
            UIManager.Instance.LoadSelectableTexture(Name + "answer1", m_Answer1, m_Answer1Selected, new MenuActionQuestionAnswer(this, 0, m_Answer), new UIObjectEffectFadeInFadeOut(1.0f), 0.0f);
            UIManager.Instance.LoadSelectableTexture(Name + "answer2", m_Answer2, m_Answer2Selected, new MenuActionQuestionAnswer(this, 1, m_Answer), new UIObjectEffectFadeInFadeOut(1.0f), 0.0f);
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            /*if (m_ActiveState == ActiveState.eIdle)
            {
                if (InputManager.Instance.IsAnyInput(PlayerIndex.One))
                {
                    m_QuestionData.SetAnswer(true);
                }
            }*/
        }

        public void ShowAnswerTextures()
        {
            Viewport viewport = RenderManager.Instance.GraphicsDevice.Viewport;

            float halfWidth = (viewport.Width * 0.5f);
            float halfHeight = (viewport.Height * 0.5f);

            // Add to the UI manager
            UIManager.Instance.AddUIObject((Name + "answer1"), new Vector3((halfWidth - 200), (viewport.Height * 0.65f), 0), UIManager.TextureAlignment.eCenter);
            UIManager.Instance.AddUIObject((Name + "answer2"), new Vector3((halfWidth + 200), (viewport.Height * 0.65f), 0), UIManager.TextureAlignment.eCenter);
        }

        public void RemoveAnswerTextures()
        {
            // Add to the UI manager
            UIManager.Instance.FlagToRemoveUIObject(Name + "answer1");
            UIManager.Instance.FlagToRemoveUIObject(Name + "answer2");
        }

        public void SetAnswer(bool answer)
        {
            m_QuestionData.SetAnswer(answer);
        }

        public bool Shown { get { return m_Shown; } set { m_Shown = value; } }
        public QuestionData QuestionData { get { return m_QuestionData; } set { m_QuestionData = value; } }
        public int Answer { get { return m_Answer; } set { m_Answer = value; } }
        public String Answer1Texture { get { return m_Answer1; } set { m_Answer1 = value; } }
        public String Answer1SelectedTexture { get { return m_Answer1Selected; } set { m_Answer1Selected = value; } }
        public String Answer2Texture { get { return m_Answer2; } set { m_Answer2 = value; } }
        public String Answer2SelectedTexture { get { return m_Answer2Selected; } set { m_Answer2Selected = value; } }
    }
}
