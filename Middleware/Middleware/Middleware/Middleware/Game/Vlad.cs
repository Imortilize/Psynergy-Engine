using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Graphics;
using Psynergy.Camera;
using Psynergy.Input;

namespace Middleware
{
    public class Vlad : SpriteNode, IRegister<Vlad>
    {
        public Vlad() : base()
        {
        }

        public Vlad(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();

            int width = 48;
            int height = 48;

            // Add animations
            AddAnimation("WalkEast", 0, 48 * 0, width, height, 8, 0.1f);
            AddAnimation("WalkNorth", 0, 48 * 1, width, height, 8, 0.1f);
            AddAnimation("WalkNorthEast", 0, 48 * 2, width, height, 8, 0.1f);
            AddAnimation("WalkNorthWest", 0, 48 * 3, width, height, 8, 0.1f);
            AddAnimation("WalkSouth", 0, 48 * 4, width, height, 8, 0.1f);
            AddAnimation("WalkSouthEast", 0, 48 * 5, width, height, 8, 0.1f);
            AddAnimation("WalkSouthWest", 0, 48 * 6, width, height, 8, 0.1f);
            AddAnimation("WalkWest", 0, 48 * 7, width, height, 8, 0.1f);

            AddAnimation("IdleEast", 0, 48 * 0, width, height, 1, 0.2f);
            AddAnimation("IdleNorth", 0, 48 * 1, width, height, 1, 0.2f);
            AddAnimation("IdleNorthEast", 0, 48 * 2, width, height, 1, 0.2f);
            AddAnimation("IdleNorthWest", 0, 48 * 3, width, height, 1, 0.2f);
            AddAnimation("IdleSouth", 0, 48 * 4, width, height, 1, 0.2f);
            AddAnimation("IdleSouthEast", 0, 48 * 5, width, height, 1, 0.2f);
            AddAnimation("IdleSouthWest", 0, 48 * 6, width, height, 1, 0.2f);
            AddAnimation("IdleWest", 0, 48 * 7, width, height, 1, 0.2f);

            DrawOffset = new Vector2(-24, (-38 * ScaleY));
            CurrentAnimation = "WalkEast";
            IsAnimating = true;

            
            TileMap tileMap = TileMapManager.Instance.GetCurrentTileMap();

            if (tileMap != null)
                tileMap.Vlad = this;
        }

        #region Update
        public override void Update(GameTime deltaTime)
        {
            float moveSpeed = 3;
            Vector2 moveVector = Vector2.Zero;
            Vector2 moveDirection = Vector2.Zero;
            String animation = "";

            if (InputHandle.GetKey(Keys.NumPad7)) 
            {
                moveDirection = new Vector2(-1, -1) * 0.5f;
                animation = "WalkNorthWest";
            }
            if (InputHandle.GetKey(Keys.NumPad8))
            {
                moveDirection = new Vector2(0, -1);
                animation = "WalkNorth";
            }

            if (InputHandle.GetKey(Keys.NumPad9))
            {
                moveDirection = new Vector2(1, -1) * 0.5f;
                animation = "WalkNorthEast";
            }

            if (InputHandle.GetKey(Keys.NumPad4))
            {
                moveDirection = new Vector2(-1, 0);
                animation = "WalkWest";
            }

            if (InputHandle.GetKey(Keys.NumPad6))
            {
                moveDirection = new Vector2(1, 0);
                animation = "WalkEast";
            }

            if (InputHandle.GetKey(Keys.NumPad1))
            {
                moveDirection = new Vector2(-1, 1) * 0.5f;
                animation = "WalkSouthWest";
            }

            if (InputHandle.GetKey(Keys.NumPad2))
            {
                moveDirection = new Vector2(0, 1);
                animation = "WalkSouth";
            }

            if (InputHandle.GetKey(Keys.NumPad3))
            {
                moveDirection = new Vector2(1, 1) * 0.5f;
                animation = "WalkSouthEast";
            }

            if (moveDirection.Length() != 0)
            {
                // Scale by move speed
                moveDirection *= moveSpeed;

                TileMap tileMap = TileMapManager.Instance.GetCurrentTileMap();

                if (tileMap != null)
                {
                    Vector2 pos2D = Position2D;

                    if (tileMap.GetCellAtWorldPoint(pos2D + moveDirection).Walkable == false)
                        moveDirection = Vector2.Zero;
                    else if (Math.Abs(tileMap.GetOverallHeight(pos2D) - tileMap.GetOverallHeight(pos2D + moveDirection)) > 10)
                        moveDirection = Vector2.Zero;
                }

                MoveBy((int)moveDirection.X, (int)moveDirection.Y);
                if (CurrentAnimation != animation)
                    CurrentAnimation = animation;
            }
            else
            {
                CurrentAnimation = "Idle" + CurrentAnimation.Substring(4);
            }
 
            GraphicsDevice graphicsDevice = RenderManager.Instance.GraphicsDevice;

            if ( graphicsDevice != null )
            {
                IsometricCamera isometricCamera = (CameraManager.Instance.ActiveCamera as IsometricCamera);

                if (isometricCamera != null)
                {
                    Viewport viewPort = graphicsDevice.Viewport;

                    // Clamp positions to be inside the screen
                    PosX = MathHelper.Clamp(PosX, -DrawOffset.X, (isometricCamera.WorldSize.X - DrawOffset.X));
                    PosY = MathHelper.Clamp(PosY, -DrawOffset.Y, (isometricCamera.WorldSize.Y - DrawOffset.Y));

                    Vector2 testPosition = isometricCamera.WorldToScreen(Position2D);

                    if (testPosition.X < 100)
                    {
                        isometricCamera.Move(new Vector2(testPosition.X - 100, 0));
                    }

                    if (testPosition.X > (viewPort.Width - 100))
                    {
                        isometricCamera.Move(new Vector2(testPosition.X - (viewPort.Width - 100), 0));
                    }

                    if (testPosition.Y < 100)
                    {
                        isometricCamera.Move(new Vector2(0, testPosition.Y - 100));
                    }

                    if (testPosition.Y > (viewPort.Height - 100))
                    {
                        isometricCamera.Move(new Vector2(0, testPosition.Y - (viewPort.Height - 100)));
                    }
                }
            }

            base.Update(deltaTime);
        }
        #endregion

        #region Render
        public override void Render(GameTime deltaTime)
        {
            // Get the current tile map
            TileMap tileMap = TileMapManager.Instance.GetCurrentTileMap();

            // use for getting the height on the tile being stood on
            if (tileMap != null)
                Offset = new Vector2(0, -tileMap.GetOverallHeight(Position2D));

            base.Render(deltaTime);
        }
        #endregion

        #region Override Functions
        public override Vector2 GetWorld2D()
        {
            Vector2 toRet = Position2D;
            IsometricCamera isometricCamera = (CameraManager.Instance.ActiveCamera as IsometricCamera);

            if (isometricCamera != null)
                toRet = isometricCamera.WorldToScreen(toRet);

            return toRet;
        }
        #endregion

    }
}
