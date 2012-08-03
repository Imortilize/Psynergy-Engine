using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Graphics;

namespace Middleware
{
    public class TileMap : SpriteNode, IRegister<TileMap>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector2("gridsize", "GridSize");

            base.ClassProperties(factory);
        }
        #endregion

        private List<MapRow> m_Rows = new List<MapRow>();
        private int m_Width = 50;
        private int m_Height = 50;

        private Vector2 m_GridSize = Vector2.Zero;

        public TileMap()
        {        
        }

        public override void Initialise()
        {
            base.Initialise();

            // Create tile map
            CreateTileMap();
        }

        private void CreateTileMap()
        {
            for (int i = 0; i < m_Height; i++)
            {
                MapRow row = new MapRow();

                for (int j = 0; j < m_Width; j++)
                {
                    // Add a map cell defaulted to index 0
                    row.Columns.Add(new MapCell(0));
                }

                // Save row item
                m_Rows.Add(row);
            }

            // Create Sample Map Data
            m_Rows[0].Columns[3].TileID = 3;
            m_Rows[0].Columns[4].TileID = 3;
            m_Rows[0].Columns[5].TileID = 1;
            m_Rows[0].Columns[6].TileID = 1;
            m_Rows[0].Columns[7].TileID = 1;

            m_Rows[1].Columns[3].TileID = 3;
            m_Rows[1].Columns[4].TileID = 1;
            m_Rows[1].Columns[5].TileID = 1;
            m_Rows[1].Columns[6].TileID = 1;
            m_Rows[1].Columns[7].TileID = 1;

            m_Rows[2].Columns[2].TileID = 3;
            m_Rows[2].Columns[3].TileID = 1;
            m_Rows[2].Columns[4].TileID = 1;
            m_Rows[2].Columns[5].TileID = 1;
            m_Rows[2].Columns[6].TileID = 1;
            m_Rows[2].Columns[7].TileID = 1;

            m_Rows[3].Columns[2].TileID = 3;
            m_Rows[3].Columns[3].TileID = 1;
            m_Rows[3].Columns[4].TileID = 1;
            m_Rows[3].Columns[5].TileID = 2;
            m_Rows[3].Columns[6].TileID = 2;
            m_Rows[3].Columns[7].TileID = 2;

            m_Rows[4].Columns[2].TileID = 3;
            m_Rows[4].Columns[3].TileID = 1;
            m_Rows[4].Columns[4].TileID = 1;
            m_Rows[4].Columns[5].TileID = 2;
            m_Rows[4].Columns[6].TileID = 2;
            m_Rows[4].Columns[7].TileID = 2;

            m_Rows[5].Columns[2].TileID = 3;
            m_Rows[5].Columns[3].TileID = 1;
            m_Rows[5].Columns[4].TileID = 1;
            m_Rows[5].Columns[5].TileID = 2;
            m_Rows[5].Columns[6].TileID = 2;
            m_Rows[5].Columns[7].TileID = 2;

            // End Create Sample Map Data
        }

        public override void Render(GameTime deltaTime)
        {   
            // Full override of sprite nodes render function for now
            if (m_Graphics != null)
            {
                if (m_Graphics.GraphicsDevice != null)
                    m_Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            }

            if (m_CurrentTexture != null)
            {
                Vector2 firstSquare = new Vector2(0 / 32, 0 / 32);
                Vector2 squareOffset = new Vector2(0 % 32, 0 % 32);

                for (int y = 0; y < m_GridSize.Y; y++)
                {
                    for (int x = 0; x < m_GridSize.X; x++)
                    {
                        int rowNum = (y + (int)firstSquare.Y);
                        int columnNum = (x + (int)firstSquare.X);

                        if ((m_Rows.Count > rowNum) && (m_Rows[rowNum].Columns.Count > columnNum))
                        {
                            int tileIndex = m_Rows[rowNum].Columns[columnNum].TileID;
                            Debug.Assert(tileIndex >= 0);

                            if ( tileIndex >= 0 )
                                m_SpriteBatch.Draw(m_CurrentTexture, new Rectangle(((x * 32) - (int)squareOffset.X), ((y * 32) - (int)squareOffset.Y), 32, 32), GetSourceRectangle(tileIndex), m_ActualColor);

                        }
                    }
                }
            }
        }

        public Rectangle GetSourceRectangle(int tileIndex)
        {
            return new Rectangle((tileIndex * 32), 0, 32, 32);
        }

        #region Property Set / Gets
        public Vector2 GridSize { get { return m_GridSize; } set { m_GridSize = value; } }
        #endregion
    }
}
