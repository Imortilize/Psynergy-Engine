using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Graphics;
using Psynergy.Camera;
using Psynergy.Input;

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

        private Vector2 m_GridSize = Vector2.Zero;
        private Vector2 m_TileSize = new Vector2(64, 64);
        private Vector2 m_TileStep = new Vector2(64, 16);
        private Vector2 m_OddRowOffset = new Vector2(32, 0);
        private Vector2 m_BaseOffset = new Vector2(-32, -64);
        private int m_HeightTileOffset = 32;
        private float m_HeightRowDepthMod = 0.0000001f;
        private float m_MaxDepth = 0.0f;

        #region Tile coordinates
        private bool m_ShowCoordinates = false;
        private SpriteFont m_Font = null;
        #endregion

        #region Tile Picking
        private bool m_AllowPicking = true;

        private Texture2D m_MouseMap = null;
        private Texture2D m_Hilight = null;
        #endregion

        public TileMap()
        {        
        }

        public override void Initialise()
        {
            base.Initialise();

            // Create tile map
            CreateTileMap();

            // Store a reference to the tile map in the isometric camera
            BaseCamera camera = CameraManager.Instance.ActiveCamera;

            if (camera != null)
            {
                IsometricCamera isometricCamera = (camera as IsometricCamera);

                if (isometricCamera != null)
                    isometricCamera.TileMap = this;
            }

            // Calculate the maximum tile depth
            m_MaxDepth = ((m_GridSize.X + 1) + ((m_GridSize.Y + 1) * m_TileSize.X)) * 10;
        }

        private void CreateTileMap()
        {
            for (int i = 0; i < m_GridSize.Y; i++)
            {
                MapRow row = new MapRow();

                for (int j = 0; j < m_GridSize.X; j++)
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

            // Add a tile id
            /*m_Rows[3].Columns[5].AddTile(30);
            m_Rows[4].Columns[5].AddTile(27);
            m_Rows[5].Columns[5].AddTile(28);

            m_Rows[3].Columns[6].AddTile(25);
            m_Rows[5].Columns[6].AddTile(24);

            m_Rows[3].Columns[7].AddTile(31);
            m_Rows[4].Columns[7].AddTile(26);
            m_Rows[5].Columns[7].AddTile(29);

            m_Rows[4].Columns[6].AddTile(104);*/

            m_Rows[16].Columns[4].AddHeightTile(54);

            m_Rows[17].Columns[3].AddHeightTile(54);

            m_Rows[15].Columns[3].AddHeightTile(54);
            m_Rows[16].Columns[3].AddHeightTile(53);

            m_Rows[15].Columns[4].AddHeightTile(54);
            m_Rows[15].Columns[4].AddHeightTile(54);
            m_Rows[15].Columns[4].AddHeightTile(51);

            m_Rows[18].Columns[3].AddHeightTile(51);
            m_Rows[19].Columns[3].AddHeightTile(50);
            m_Rows[18].Columns[4].AddHeightTile(55);

            m_Rows[14].Columns[4].AddHeightTile(54);

            m_Rows[14].Columns[5].AddHeightTile(62);
            m_Rows[14].Columns[5].AddHeightTile(61);
            m_Rows[14].Columns[5].AddHeightTile(63);

            m_Rows[17].Columns[4].AddTopperTile(114);
            m_Rows[16].Columns[5].AddTopperTile(115);
            m_Rows[14].Columns[4].AddTopperTile(125);
            m_Rows[15].Columns[5].AddTopperTile(91);
            m_Rows[16].Columns[6].AddTopperTile(94);
             
            // End Create Sample Map Data
        }

        #region Load
        public override void Load()
        {
            base.Load();

            m_Font = RenderManager.Instance.LoadFont("Pericles6");

            // Load mouse map ( assuming there is one )
            try
            {
                m_MouseMap = RenderManager.Instance.LoadTexture2D("Textures/TileSets/mousemap");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Load hi light
            try
            {
                m_Hilight = RenderManager.Instance.LoadTexture2D("Textures/TileSets/hilight");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        #region Render
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
                BaseCamera camera = CameraManager.Instance.ActiveCamera;

                if (camera != null)
                {
                    IsometricCamera isometricCamera = (camera as IsometricCamera);

                    if ( isometricCamera != null )
                    {
                        Vector2 cameraPos = isometricCamera.GetPos2D();
                        Vector2 firstSquare = new Vector2(cameraPos.X / m_TileStep.X, cameraPos.Y / m_TileStep.Y);
                        Vector2 squareOffset = new Vector2(cameraPos.X % m_TileStep.X, cameraPos.Y % m_TileStep.Y);

                        float depthOffset = 0.0f;

                        for (int y = 0; y < m_GridSize.Y; y++)
                        {
                            int mapY = (y + (int)firstSquare.Y);
                            int rowOffset = 0;

                            if ((mapY % 2) == 1)
                                rowOffset = (int)m_OddRowOffset.X;

                            for (int x = 0; x < m_GridSize.X; x++)
                            {
                                int mapX = (x + (int)firstSquare.X);

                                if ((mapY >= 0) && (mapX >= 0))
                                {
                                    if ((m_Rows.Count > mapY) && (m_Rows[mapY].Columns.Count > mapX))
                                    {
                                        if ((mapX >= m_GridSize.X) || (mapY >= m_GridSize.Y))
                                            continue;

                                        // Draw terrain layers
                                        foreach (int tileIndex in m_Rows[mapY].Columns[mapX].BaseTiles)
                                        {
                                            Debug.Assert(tileIndex >= 0);

                                            if (tileIndex >= 0)
                                            {
                                                Vector2 worldPosition = isometricCamera.WorldToScreen(new Vector2((mapX * m_TileStep.X) + rowOffset, mapY * m_TileStep.Y));
                                                Rectangle sourceRect = GetSourceRectangle(tileIndex);

                                                if (sourceRect != Rectangle.Empty)
                                                {
                                                    // Calculate render depth
                                                    m_RenderDepth = 1.0f;

                                                    // Draw sprite
                                                    m_SpriteBatch.Draw(m_CurrentTexture, worldPosition, sourceRect, m_ActualColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, MathHelper.Clamp(m_RenderDepth, 0.0f, 1.0f));
                                                }
                                            }
                                        }

                                        // Calculate the depth offset
                                        depthOffset = 0.7f - ((mapX + (mapY * m_TileSize.X)) / m_MaxDepth);

                                        // Height row
                                        int heightRow = 0;

                                        // Draw height layers
                                        foreach (int tileIndex in m_Rows[mapY].Columns[mapX].HeightTiles)
                                        {
                                            Debug.Assert(tileIndex >= 0);

                                            if (tileIndex >= 0)
                                            {
                                                Vector2 worldPosition = isometricCamera.WorldToScreen(new Vector2((mapX * m_TileStep.X) + rowOffset, mapY * m_TileStep.Y - (heightRow * m_HeightTileOffset)));
                                                Rectangle sourceRect = GetSourceRectangle(tileIndex);

                                                if (sourceRect != Rectangle.Empty)
                                                {
                                                    // Calculate render depth
                                                    m_RenderDepth = (depthOffset - ((float)heightRow * m_HeightRowDepthMod));

                                                    // Draw sprite
                                                    m_SpriteBatch.Draw(m_CurrentTexture, worldPosition, sourceRect, m_ActualColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, MathHelper.Clamp(m_RenderDepth, 0.0f, 1.0f));
                                                }
                                            }

                                            // Increment height row
                                            heightRow++;
                                        }

                                        // Draw height layers
                                        foreach (int tileIndex in m_Rows[mapY].Columns[mapX].TopperTiles)
                                        {
                                            Debug.Assert(tileIndex >= 0);

                                            if (tileIndex >= 0)
                                            {
                                                Vector2 worldPosition = isometricCamera.WorldToScreen(new Vector2((mapX * m_TileStep.X) + rowOffset, mapY * m_TileStep.Y - (heightRow * m_HeightTileOffset)));
                                                Rectangle sourceRect = GetSourceRectangle(tileIndex);

                                                if (sourceRect != Rectangle.Empty)
                                                {
                                                    // Calculate render depth
                                                    m_RenderDepth = (depthOffset - ((float)heightRow * m_HeightRowDepthMod));

                                                    // Draw sprite
                                                    m_SpriteBatch.Draw(m_CurrentTexture, worldPosition, sourceRect, m_ActualColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, MathHelper.Clamp(m_RenderDepth, 0.0f, 1.0f));
                                                }
                                            }
                                        }

                                        // If tile picking is allowed
                                        if (m_AllowPicking && (m_Hilight != null))
                                        {
                                            InputManager input = InputManager.Instance;

                                            if ( input != null )
                                            {
                                                Vector2 hilightLocation = isometricCamera.ScreenToWorld(input.GetCurrentMousePos());
                                                Point hilightPoint = WorldToMapCell(new Point((int)hilightLocation.X, (int)hilightLocation.Y));

                                                // Calculate whether to offset the highlight or not
                                                int hilightrowOffset = 0;
                                                if ((hilightPoint.Y) % 2 == 1)
                                                    hilightrowOffset = (int)m_OddRowOffset.X;
                                                
                                                Vector2 screenPosition = isometricCamera.WorldToScreen(new Vector2((hilightPoint.X * m_TileStep.X) + hilightrowOffset, (hilightPoint.Y + 2) * m_TileStep.Y));

                                                m_SpriteBatch.Draw(
                                                                m_Hilight,
                                                                screenPosition,
                                                                new Rectangle(0, 0, 64, 32),
                                                                (Color.White * 0.3f),
                                                                0.0f,
                                                                Vector2.Zero,
                                                                1.0f,
                                                                SpriteEffects.None,
                                                                0.0f);
                                            }
                                        }

                                        // Use to show tile coordinates
                                        if (m_ShowCoordinates)
                                        {
                                            // Draw tile coordinates
                                            m_SpriteBatch.DrawString(m_Font, mapX.ToString() + ", " + mapY.ToString(),
                                                                    new Vector2((x * m_TileStep.X) - squareOffset.X + rowOffset + m_BaseOffset.X + 24,
                                                                    (y * m_TileStep.Y) - squareOffset.Y + m_BaseOffset.Y + 48),
                                                                    Color.White,
                                                                    0f,
                                                                    Vector2.Zero,
                                                                    1.0f,
                                                                    SpriteEffects.None,
                                                                    0.0f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public Rectangle GetSourceRectangle(int tileIndex)
        {
            Rectangle toRet = Rectangle.Empty;

            if (m_CurrentTexture != null)
            {
                int tileY = tileIndex / (m_CurrentTexture.Width / (int)m_TileSize.X);
                int tileX = tileIndex % (m_CurrentTexture.Width / (int)m_TileSize.X);

                toRet = new Rectangle((tileX * (int)m_TileSize.X), (tileY * (int)m_TileSize.Y), (int)m_TileSize.X, (int)m_TileSize.Y);
            }

            return toRet;
        }
        #endregion

        #region Tile Picking
        public Point WorldToMapCell(Point worldPoint)
        {
            Point dummy;
            return WorldToMapCell(worldPoint, out dummy);
        }

        private Point WorldToMapCell(Point worldPoint, out Point localPoint)
        {
            Point mapCell = new Point((int)(worldPoint.X / m_MouseMap.Width),
                                      (int)(worldPoint.Y / m_MouseMap.Height) * 2);

            int localPointX = (worldPoint.X % m_MouseMap.Width);
            int localPointY = (worldPoint.Y % m_MouseMap.Height);

            int dx = 0;
            int dy = 0;

            uint[] myUInt = new uint[1];

            if (new Rectangle(0, 0, m_MouseMap.Width, m_MouseMap.Height).Contains(localPointX, localPointY))
            {
                m_MouseMap.GetData(0, new Rectangle(localPointX, localPointY, 1, 1), myUInt, 0, 1);

                if (myUInt[0] == 0xFF0000FF) // Red
                {
                    dx = -1;
                    dy = -1;
                    localPointX = (localPointX + (m_MouseMap.Width / 2));
                    localPointY = (localPointY + (m_MouseMap.Height / 2));
                }

                if (myUInt[0] == 0xFF00FF00) // Green
                {
                    dx = -1;
                    localPointX = (localPointX + (m_MouseMap.Width / 2));
                    dy = 1;
                    localPointY = (localPointY - (m_MouseMap.Height / 2));
                }

                if (myUInt[0] == 0xFF00FFFF) // Yellow
                {
                    dy = -1;
                    localPointX = (localPointX - (m_MouseMap.Width / 2));
                    localPointY = (localPointY + (m_MouseMap.Height / 2));
                }

                if (myUInt[0] == 0xFFFF0000) // Blue
                {
                    dy = +1;
                    localPointX = (localPointX - (m_MouseMap.Width / 2));
                    localPointY = (localPointY - (m_MouseMap.Height / 2));
                }
            }

            mapCell.X += dx;
            mapCell.Y += (dy - 2);

            // New local point
            localPoint = new Point(localPointX, localPointY);

            // Return the map cell
            return mapCell;
        }
        #endregion

        #region Property Set / Gets
        public Vector2 GridSize { get { return m_GridSize; } set { m_GridSize = value; } }
        public Vector2 TileSize { get { return m_TileSize; } set { m_TileSize = value; } }
        public Vector2 TileStep { get { return m_TileStep; } set { m_TileStep = value; } }
        public Vector2 BaseOffset { get { return m_BaseOffset; } }
        #endregion
    }
}
