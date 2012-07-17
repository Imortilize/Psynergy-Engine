using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

/* Event Library */
using Psynergy.Events;

namespace Psynergy.Graphics
{
    public class TerrainSetEvent : IEvent
    {
        private TerrainNode m_Terrain;

        public TerrainSetEvent(TerrainNode terrain)
        {
            m_Terrain = terrain;
        }

        public void Fire()
        {
            EventManager.Instance.SendMessage(this);
        }

        public TerrainNode Terrain { get { return m_Terrain; } }
    }

    public class TerrainLoadedEvent : IEvent
    {
        private RenderNode m_Terrain;

        public TerrainLoadedEvent(RenderNode terrain)
        {
            m_Terrain = terrain;
        }

        public void Fire()
        {
            EventManager.Instance.SendMessage(this);
        }

        public RenderNode Terrain { get { return m_Terrain; } }
    }
}
