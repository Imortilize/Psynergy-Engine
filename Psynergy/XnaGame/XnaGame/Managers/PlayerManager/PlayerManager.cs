using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

/* input Library */
using Psynergy.Input;

/* Camera Library */
using Psynergy.Camera;

namespace XnaGame
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private int m_MaxPlayers = 2;
        private List<Player> m_Players = new List<Player>();
        private Player m_ActivePlayer = null;
        private int m_NumPlayersToUse = 1;

        public PlayerManager()
        {
            // Add players here for now
            for (int i = 0; i < m_MaxPlayers; i++)
                AddPlayer(new GamePlayer("Player" + i));
        }

        public override void Initialise()
        {
            base.Initialise();

            foreach (Player player in m_Players)
                player.Initialise();
        }

        public override void Reset()
        {
            base.Reset();

            foreach (Player player in m_Players)
                player.Reset();
        }

        public override void Load()
        {
            base.Load();

            foreach (Player player in m_Players)
                player.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();

            foreach (Player player in m_Players)
                player.UnLoad();
        }

        public override void Update(GameTime deltaTime)
        {
            // We shall put the selection code in here for now as it is a 
            // player related task.
            if (InputHandle.GetMouse(0))
            {
                RenderManager renderManager = RenderManager.Instance;

                // Get camera manager
                CameraManager camera = CameraManager.Instance;

                if (camera != null)
                {
                    // Cast a ray into the scene for picking
                    camera.CastRay(renderManager.GraphicsDevice.Viewport);
                }
            }

            //foreach (Player player in m_Players)
               // player.Update(deltaTime);

            // Run active player code if an active player is set 
            //if (m_ActivePlayer != null)
            //m_ActivePlayer.Update(deltaTime);
        }

        public void AddPlayer(Player player)
        {
            Debug.Assert((player != null), "Can't add a null player");

            if (player != null)
            {
                Debug.Assert((m_Players.Count < m_MaxPlayers), "Maximum players already added");

                if (m_Players.Count < m_MaxPlayers)
                {
                    m_Players.Add(player);
                    player.Index = ((PlayerIndex)(m_Players.Count - 1));
                }
            }
        }

        public void RemovePlayer(Player player)
        {
            if (player != null)
            {
                if (m_Players.Contains(player))
                    m_Players.Remove(player);
            }
        }

        public void RemovePlayerAtIndex(PlayerIndex playerIndex)
        {
            if ((int)playerIndex < m_Players.Count)
                m_Players.RemoveAt((int)playerIndex);
        }

        public void SetActivePlayer(PlayerIndex index)
        {
            int i = (int)index;

            Debug.Assert((m_Players.Count > i), "Player " + i + "doesn't exist!");

            if (m_Players.Count > i)
                m_ActivePlayer = m_Players[i];
        }

        public void ShowActivePlayer()
        {
            if (m_ActivePlayer != null)
                m_ActivePlayer.OnSelect();
        }

        public void SetAllPlayersInActive()
        {
            m_ActivePlayer = null;
        }

        public void SetCameraFocus(PlayerIndex index)
        {
            int i = (int)index;

            Debug.Assert((m_Players.Count > i), "Player " + i + "doesn't exist!");

            if (m_Players.Count > i)
            {
                BaseCamera gameCamera = CameraManager.Instance.ActiveCamera;

                if (gameCamera != null)
                    gameCamera.SetFocus(m_Players[i].GetFocus());
            }
        }

        public void SetInstantCameraFocus(PlayerIndex index)
        {
            int i = (int)index;

            Debug.Assert((m_Players.Count > i), "Player " + i + "doesn't exist!");

            if (m_Players.Count > i)
            {
                BaseCamera gameCamera = CameraManager.Instance.ActiveCamera;

                if (gameCamera != null)
                    gameCamera.SetInstantFocus(m_Players[i].GetFocus());
            }
        }

        public void AddPlayersToScene(Scene scene)
        {
            if (m_Players.Count > 0)
            {
                if (scene != null)
                {
                    for (int i = 0; i < m_NumPlayersToUse; i++)
                    {
                        Player player = m_Players[i];

                        // Add this player to the scene
                        player.AddToScene(scene);
                    }
                }
            }
        }

        public void RemovePlayersFromScene()
        {
            foreach (Player player in m_Players)
                player.RemoveFromScene();
        }

        public void ResetPlayersInScene(Scene scene)
        {
            // Remove all players from scene
            RemovePlayersFromScene();

            // Now add players again
            AddPlayersToScene(scene);
        }

        public void NextPlayer()
        {
            if (m_Players.Count > 0)
            {
                if (m_ActivePlayer != null)
                {
                    int index = (int)m_ActivePlayer.Index;

                    if (index >= 0)
                    {
                        int nextPlayerIndex = 0;
                        Player nextPlayer = m_ActivePlayer;

                        if ((m_NumPlayersToUse > (index + 1)) && (m_Players.Count > (index + 1)))
                            nextPlayerIndex = (index + 1);
                        else
                            nextPlayerIndex = 0;

                        // If it isn't the pawn already selected
                        if (nextPlayerIndex != index)
                        {
                            // Run deselect code 
                            if (m_ActivePlayer != null)
                                m_ActivePlayer.OnDeselect();

                            // Try to get the next player
                            nextPlayer = m_Players[nextPlayerIndex];

                            // If there is a next player to change to
                            if (nextPlayer != null)
                            {
                                // Set as active player
                                m_ActivePlayer = nextPlayer;
                            }
                        }
                    }

                    // Show this player as being active
                    ShowActivePlayer();
                }
            }
        }

        public Player GetPlayer(PlayerIndex index)
        {
            Player toRet = null;

            if ((int)index < m_Players.Count)
                toRet = m_Players[(int)index];

            return toRet;
        }

        #region Properties
        public int NumberPlayersToUse { get { return m_NumPlayersToUse; } set { m_NumPlayersToUse = value; } }
        public Player ActivePlayer { get { return m_ActivePlayer; } }
        #endregion
    }
}
