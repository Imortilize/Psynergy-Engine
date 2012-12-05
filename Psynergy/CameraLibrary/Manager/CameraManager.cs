using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* Main Library */
using Psynergy;

/* Input Library */
using Psynergy.Input;

/* Event Library */
using Psynergy.Events;

/* needs to go at some point ( should not be included in input library ) */
//using XnaGame;

namespace Psynergy.Camera
{
    public class CameraManager : Singleton<CameraManager>, IListener<CameraFocusEvent>
    {
        private GraphicsDevice m_GraphicsDevice = null;
        private SortedList<String, BaseCamera> m_Cameras = new SortedList<String, BaseCamera>();
        private CameraResource m_CameraResource = null;
        private BaseCamera m_ActiveCamera = null;

        public CameraManager()
        {
        }

        public CameraManager(GraphicsDevice graphicsDevice)
        {
            m_GraphicsDevice = graphicsDevice;
        }

        public override void Initialise()
        {
            base.Initialise();

            m_CameraResource = new CameraResource("Resources/GameCameras.xml", m_GraphicsDevice);
        }

        public override void Load()
        {
            base.Load();

            if (m_CameraResource != null)
            {
                m_CameraResource.Load();

                // Copy cameras to the camera manager
                m_Cameras = m_CameraResource.LoadedCameras;

                // Set active camera
                if ( m_Cameras.Count > 0 )
                    m_ActiveCamera = m_Cameras.Values[0];
            }
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (m_ActiveCamera != null)
                m_ActiveCamera.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            if (m_ActiveCamera != null)
                m_ActiveCamera.Render(deltaTime);
        }

        public bool ChangeCamera(String cameraName)
        {
            bool toRet = false;
            bool allowChange = true;

            if (m_ActiveCamera != null)
                allowChange = m_ActiveCamera.CanChange;

            if (allowChange)
            {
                if (cameraName != "")
                {
                    if (m_Cameras.ContainsKey(cameraName))
                    {
                        BaseCamera previousCamera = m_ActiveCamera;

                        // Switch the active camera
                        m_Cameras.TryGetValue(cameraName, out m_ActiveCamera);

                        // Call the on Change To method
                        m_ActiveCamera.OnChangedTo(previousCamera);

                        // Set up camera initially
                        m_ActiveCamera.SetUpCamera(new GameTime());

                        // Retunr that it changed ok
                        toRet = true;
                    }
                }
            }

            return toRet;
        }

        public void SetDefaultCamera()
        {
            ChangeCamera("Default");
        }

        #region Ray Casting
        public Ray CastRay(Viewport viewPort)
        {
            Ray ray = new Ray();

            // Check that there is an active camera
            if (m_ActiveCamera != null)
            {
                Type cameraType = m_ActiveCamera.GetType();

                if (cameraType == typeof(Camera3D) || cameraType.IsSubclassOf(typeof(Camera3D)))
                {
                    Camera3D camera = (m_ActiveCamera as Camera3D);

                    Vector2 currentMouseState = InputHandle.MousePosition;
                    Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                    Vector3 nearPoint = new Vector3(mousePosition, viewPort.MinDepth);
                    Vector3 farPoint = new Vector3(mousePosition, viewPort.MaxDepth);

                    nearPoint = viewPort.Unproject(nearPoint, camera.Projection, camera.View, Matrix.Identity);
                    farPoint = viewPort.Unproject(farPoint, camera.Projection, camera.View, Matrix.Identity);

                    Vector3 direction = (farPoint - nearPoint);
                    direction.Normalize();

                    // Save the ray values accordingly
                    ray.Position = nearPoint;
                    ray.Direction = direction;

                    // TEST - fire ray cast
                    //SelectObjectEvent selectEvent = new SelectObjectEvent(ray);
                    //EventManager.Instance.SendMessage(selectEvent);
                }
            }

            return ray;
        }
        #endregion

        #region event handlers
        public virtual void Handle(CameraFocusEvent message)
        {
            if (m_ActiveCamera != null)
            {
                Node focusObject = message.FocusObject;

                if (focusObject != null)
                    m_ActiveCamera.SetFocus(focusObject);
            }
        }
        #endregion

        #region Property Set / Gets
        public SortedList<String, BaseCamera> Cameras { get { return m_Cameras; } }
        public BaseCamera ActiveCamera { get { return m_ActiveCamera; } }
        #endregion
    }
}
