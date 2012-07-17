using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaGame
{
    class StateManager : Object
    {
        private SortedList<String, State> m_States = new SortedList<String, State>();
        private State m_CurrentState = null;
        private State m_PreviousState = null;

        public StateManager( String name ) : base( name )
        {
        }

        public override void Initialise()
        {
            AddState("Game", new GameState( "Game" ));

            // Set default state state
            ChangeState("Game");
        }

        public override void Load()
        {
            for (int i = 0; i < m_States.Count; i++)
            {
                if (m_States.ElementAt(i).Value != null)
                    m_States.ElementAt(i).Value.Load();
            }
        }

        public override void UnLoad()
        {
            for (int i = 0; i < m_States.Count; i++)
            {
                if (m_States.ElementAt(i).Value != null)
                    m_States.ElementAt(i).Value.UnLoad();
            }
        }

        public override void Reset()
        {
            for (int i = 0; i < m_States.Count; i++)
            {
                if ( m_States.ElementAt(i).Value != null )
                    m_States.ElementAt(i).Value.Reset();
            }
        }

        public void AddState(String stateID, State state)
        {
            // Default the state id to the next number if one hasn't been set
            if (stateID == "")
                stateID = m_States.Count.ToString();
    
            // Add to the state buffer.
            m_States.Add(stateID, state);

            // Initiliase the state ( should never crash but safety check none the less )
            if (  m_States.ElementAt( (m_States.Count - 1) ).Value != null )
                m_States.ElementAt ( (m_States.Count - 1) ).Value.Initialise();
        }

        public override void Update(GameTime deltaTime)
        {
            // Update the current state if it exists
            if ( m_CurrentState != null )
                m_CurrentState.Update( deltaTime );
        }

        public void ChangeState(String stateID)
        {
            // Try and find the state is
            if (m_States.ContainsKey(stateID))
            {
                // Close the current state 
                if (m_CurrentState != null)
                    m_CurrentState.OnExit();

                // Save it as the previous state
                m_PreviousState = m_CurrentState;

                // Try and get the current state
               bool result = m_States.TryGetValue(stateID, out m_CurrentState);

               // Open the new state
               m_CurrentState.OnEnter();
            }
        }
    }
}
