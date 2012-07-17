using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Psynergy;
using Psynergy.Events;
using Psynergy.Graphics;

namespace Middleware
{
    public class TerrainManager : Singleton<TerrainManager>, IListener<TerrainLoadedEvent>
    {
        private Terrain m_Terrain = null;

        public TerrainManager() : base()
        {
            int spug = 0;
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();

            // All Node3DContollers listen for terrain load events
            EventManager.Instance.Subscribe<TerrainLoadedEvent>(this);
        }

        public virtual void Handle(TerrainLoadedEvent message)
        {
            m_Terrain = (message.Terrain as Terrain);
        }

        public void GetHeight()
        {
            if (m_Terrain != null)
            {

            }
        }

        #region Property Set / Gets
        public Terrain Terrain { get { return m_Terrain; } }
        #endregion
    }
}
