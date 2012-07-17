using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

namespace Psynergy.AI
{
    public class StateMachine<T> : GameObject
    {
        private SortedList<String, State<T>> m_States = new SortedList<String, State<T>>();
        private State<T> m_CurrentState = null;
        private State<T> m_PreviousState = null;
        private String m_DefaultState = null;

        private String m_StateToChangeTo = "";

        // The resource used to load the state machine states
        private StateResource<T> m_Resource = null;

        // The object reference this state machine is linked to
        private T m_ObjectReference = default(T);

        /* Different possible state machine interfaces */
        public StateMachine(String name) : base(name)
        {
            m_ObjectReference = default(T);
        }

        public StateMachine(String name, String resource) : base(name)
        {
            m_ObjectReference = default(T);

            if (resource != null)
                m_Resource = new StateResource<T>(resource, this);
        }

        public StateMachine( String name, T objRef ) : base( name )
        {
            m_ObjectReference = objRef;
        }

        public StateMachine(String name, T objRef, String resource) : base(name)
        {
            m_ObjectReference = objRef;

            if (resource != null)
                m_Resource = new StateResource<T>(resource, this);
        }

        public override void Initialise()
        {
        }

        public override void Load()
        {
            if (m_Resource != null)
                m_Resource.Load();

            for (int i = 0; i < m_States.Count; i++)
            {
                if (m_States.ElementAt(i).Value != null)
                    m_States.ElementAt(i).Value.Load();
            }

            // Change to the default state
            if (m_DefaultState != null)
                ChangeState(m_DefaultState);
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

            if ((m_DefaultState != "") && (m_CurrentState != null))
            {
                if (m_CurrentState.Name != m_DefaultState)
                    ChangeState(m_DefaultState);
            }

            if (m_PreviousState != null)
                m_PreviousState = null;
        }

        public void AddState(String stateID, State<T> state)
        {
            if (state != null)
            {
                // Default the state id to the next number if one hasn't been set
                if (stateID == "")
                    stateID = m_States.Count.ToString();

                // Save the state ID as the state name
                state.Name = stateID;

                // Add to the state buffer.
                m_States.Add(stateID, state);

                // Initiliase the state ( should never crash but safety check none the less )
                state.Initialise();
            }
        }

        public void ChangeState(String stateID)
        {
            // Try and find the state is
            if (m_States.ContainsKey(stateID))
            {
                // Close the current state 
                if (m_CurrentState != null)
                    m_CurrentState.OnExit( m_ObjectReference );

                // Save it as the previous state
                m_PreviousState = m_CurrentState;

                // Remove the current state
                m_CurrentState = null;

                // Set the state to change to
                m_StateToChangeTo = stateID;
            }
        }

        public override void Update(GameTime deltaTime)
        {
            // Make sure the previous state has been closed
            if (m_PreviousState != null)
            {
                if (!m_PreviousState.IsClosed())
                {
                    m_PreviousState.Update(deltaTime, m_ObjectReference);

                    return;
                }
                else
                {
                    if (!m_PreviousState.IsFinished())
                        m_PreviousState.OnFinish();
                }
            }

            // Check if there wants to be a state change
            if ( m_StateToChangeTo != "" )
            {
                 // Try and get the current state
                bool result = m_States.TryGetValue(m_StateToChangeTo, out m_CurrentState);
                Debug.Assert(result, "State " + m_StateToChangeTo + " does not exist!");

                // Open the new state
                m_CurrentState.OnEnter( m_ObjectReference );

                // No state to change to anymore
                m_StateToChangeTo = "";
            }

            // Otherwise Update the current state if it exists
            if (m_CurrentState != null)
                m_CurrentState.Update(deltaTime, m_ObjectReference);
        }

        public override void Render(GameTime deltaTime)
        {
            // Update the current state if it exists
            if (m_CurrentState != null)
                m_CurrentState.Render(deltaTime, m_ObjectReference);
        }

        public void SetDefaultState(String stateID)
        {
            m_DefaultState = stateID;
        }
    }
}
