using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Input Library */
using Psynergy;
using Psynergy.Input;

namespace Psynergy.Camera
{
    public class Camera3D : BaseCamera, ICamera3D, IRegister<Camera3D>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("currenttargetpos", "CurrentTargetPosition");
            factory.RegisterVector3("target", "Target");

            base.ClassProperties(factory);
        }
        #endregion

        private Matrix m_Transform = Matrix.Identity;
        private Matrix m_EyeTransform = Matrix.Identity;   // Inverse of the transform
        private Matrix m_EyeProjectionTransform = Matrix.Identity;

        //protected Matrix m_ViewMatrix = Matrix.Identity;
        protected Matrix m_ProjectionMatrix = Matrix.Identity;
        protected Matrix m_ShadowProjectionMatrix = Matrix.Identity;    // not ideal place but for sharper shadows for now
        protected bool m_ProjectionDirty = false;

        protected Matrix m_ViewProjection = Matrix.Identity;

        protected Vector3 m_Translation = Vector3.Zero;

        protected Quaternion m_Rotation = Quaternion.Identity;
        protected Quaternion m_StartRotation = Quaternion.Identity;

        protected float m_Roll = 0.0f;
        protected float m_RollSpeed = 0.0f;
        protected float m_StartRoll = 0.0f;
        protected float m_StartRollSpeed = 0.0f;

        protected float m_Yaw = 0.0f;
        protected float m_YawSpeed = 0.0f;
        protected float m_StartYaw = 0.0f;
        protected float m_StartYawSpeed = 0.0f;

        protected float m_Pitch = 0.0f;
        protected float m_PitchSpeed = 0.0f;
        protected float m_StartPitch = 0.0f;
        protected float m_StartPitchSpeed = 0.0f;

        protected float m_NearPlane = 1f;
        protected float m_FarPlane = 3000;
        protected float m_FOV = 45;
        protected float m_AspectRatio = 1;

        protected float m_PercentageDistanceCovered = 0.0f;         // Percentage of distance covered by camera from point A to point B
        protected Vector3 m_PositionOnEnter = Vector3.Zero;
        protected Quaternion m_RotationOnEnter = Quaternion.Identity;

        // Rasterizer state
        protected RasterizerState m_RasterizerState = new RasterizerState();
        protected CullMode m_CullMode = CullMode.None;

        // Debug options
        protected FillMode m_FillMode = FillMode.Solid;

        public Camera3D() : base("")
        {
        }

        public Camera3D(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            // Set the start rotation
            m_StartRotation = Rotation;

            // Set the start rotation builder values
            m_StartRoll = Roll;
            m_StartRollSpeed = RollSpeed;
            m_StartYaw = Yaw;
            m_StartYawSpeed = YawSpeed;
            m_StartPitch = Pitch;
            m_StartPitchSpeed = PitchSpeed;

            // Store the graphics device
            m_ProjectionDirty = true;

            // Generate frustum corners
            Frustum = new BoundingFrustum( Matrix.Identity );

            base.Initialise();
        }

        public override void Reset()
        {
            base.Reset();

            Rotation = m_StartRotation;
            Roll = m_StartRoll;
            RollSpeed = m_StartRollSpeed;
            Yaw = m_StartYaw;
            YawSpeed = m_StartYawSpeed;
            Pitch = m_StartPitch;
            PitchSpeed = m_StartPitchSpeed;
            m_PercentageDistanceCovered = 0.0f;

            m_Translation = Vector3.Zero;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Any code that needs to happen after setting up the camera...
            // Get the relative effect
            /*Effect effect = RenderManager.Instance.GetEffect("generic");

            Debug.Assert(effect != null, "Effect was not loaded!");

            if (effect != null)
            {
                // Set the relative parameters
                effect.Parameters["xView"].SetValue(View);
                effect.Parameters["xProjection"].SetValue(Projection);
            }*/

            // Check camera render options
            UpdateRenderOptions();
        }

        protected override void UpdateEnter(GameTime deltaTime)
        {
            base.UpdateEnter(deltaTime);

            if (m_Tween)
            {
                // Set start position when entering the camera state
                m_PositionOnEnter = CurrentTargetPosition;
            }
            else
                m_PositionOnEnter = Position;
        }

        protected override void Move(GameTime deltaTime)
        {
            base.Move(deltaTime);

            // Get the next target position desired
            Vector3 targetVector = (Target - CurrentTargetPosition);

            // Distance of target direction
            float distanceToTarget = targetVector.Length();

            // Normalize target direction vector
            Vector3 targetDirection = targetVector;
            targetDirection.Normalize();

            // Update the current target position if the camera distance is greater then 0.5
            if (m_Tween && (distanceToTarget > 1))
            {
                // Move the Camera to the position that it needs to go
                var delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                // Start to slow down once it is almost at its location
                if (m_PercentageDistanceCovered < 50)
                    Velocity += (MoveSpeed * delta);
                else
                {
                    if (Velocity > 0.5f)
                    {
                        Velocity -= (MoveSpeed * delta);

                        if (Velocity < 0.5f)
                            Velocity = 0.5f;
                    }
                }

                CurrentTargetPosition += (targetDirection * Velocity);

                Vector3 originalDistanceVector = (Target - PositionOnTargetSet);
                Vector3 currentDistanceVector = (CurrentTargetPosition - PositionOnTargetSet);

                float originalDistance = originalDistanceVector.Length();
                float currentDistance = currentDistanceVector.Length();

                // Save the percentage distance covered
                m_PercentageDistanceCovered = ((currentDistance / originalDistance) * 100);

                if (currentDistance > originalDistance)
                {
                    // Stop tweening
                    m_Tween = false;

                    // Set the current target position to the actual target position
                    CurrentTargetPosition = Target;

                    // Reset the velocity
                    Velocity = 0.0f;

                    // Percentage at 100
                    m_PercentageDistanceCovered = 100.0f;
                }
            }
            else
            {
                // Stop tweening
                m_Tween = false;

                // Set the current target position to the actual target position
                CurrentTargetPosition = Target;

                // Reset the velocity
                Velocity = 0.0f;
            }
        }

        protected override void Rotate(GameTime deltaTime)
        {
            // increase rotation
            Pitch += (m_PitchSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);
            Yaw += (m_YawSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);
            Roll += (m_RollSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);

            // Make sure the rotations stay within the 0 - 360 boundary
            Pitch = ClampWithin0to360(Pitch);
            Yaw = ClampWithin0to360(Yaw);
            Roll = ClampWithin0to360(Roll);

            // Clamp values ( overriable so values can be clamped before building rotation quarternion
            Clamp();
  
            // normal rotation
            Rotation = Quaternion.CreateFromYawPitchRoll(m_Yaw, m_Pitch, m_Roll);
        }

        protected virtual void Clamp()
        {
        }

        protected float ClampWithin0to360(float radialAngle)
        {
            float degrees = MathHelper.ToDegrees(radialAngle);

            if (degrees < 0)
                degrees += 360;
            else if (degrees > 360)
                degrees -= 360;

            return MathHelper.ToRadians(degrees);
        }

        public override void SetUpCamera(GameTime deltaTime)
        {
            // Setup the view matrix
            GenerateViewMatrix(deltaTime);

            // Only re calculate camera projection if it has changed.
            if (m_ProjectionDirty)
            {
                // Create projection matrix
                GeneratePerspectiveProjectionMatrix(m_FOV);
            }
        }

        protected virtual void GenerateViewMatrix(GameTime deltaTime)
        {
            Vector3 camUp = Vector3.Transform(Vector3.Up, Matrix.CreateFromQuaternion(Rotation));

            View = Matrix.CreateLookAt(Position, Vector3.Zero, camUp);
        }

        private void GeneratePerspectiveProjectionMatrix(float fov)
        {
            if (m_GraphicsDevice != null)
            {
                PresentationParameters pp = m_GraphicsDevice.PresentationParameters;

                m_AspectRatio = ((float)pp.BackBufferWidth / (float)pp.BackBufferHeight);

                m_ShadowProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), m_AspectRatio, m_NearPlane, 400);
                Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), m_AspectRatio, m_NearPlane, m_FarPlane);

                // Eye projection transform
                m_EyeProjectionTransform = Matrix.Multiply(m_EyeTransform, Projection);

                // Project has been recalculated
                m_ProjectionDirty = false;
            }
        }

        private void GenerateFrustum()
        {
            // Calculate the view-projection matrix
            m_ViewProjection = (View * Projection);

            // Now create the bounding frustum for the camera
            Frustum = new BoundingFrustum(Matrix.Multiply(m_EyeTransform, Projection));

            // And create the smaller shadow frustum for sharper shadows
            ShadowFrustum = new BoundingFrustum(Matrix.Multiply(m_EyeTransform, m_ShadowProjectionMatrix));

            // Computer the frustum corners
           // ComputeFrustumCorners();
        }

        private void UpdateRenderOptions()
        {
            if (InputManager.Instance.KeyPressed(Keys.F1))
            {
                if (m_FillMode == FillMode.Solid)
                    m_FillMode = FillMode.WireFrame;
                else
                    m_FillMode = FillMode.Solid;
            }
        }

        public bool BoundingVolumeIsInView(BoundingSphere sphere)
        {
            return (Frustum.Contains(sphere) != ContainmentType.Disjoint);
        }

        public bool BoundingVolumeIsInView(BoundingBox box)
        {
            return (Frustum.Contains(box) != ContainmentType.Disjoint);
        }

        public void ProjectBoundingSphereOnScreen(BoundingSphere boundingSphere, out Vector2 topLeft, out Vector2 size)
        {
            //l is the bounding sphere's position in eye space
            Vector3 l = Vector3.Transform(boundingSphere.Center, m_EyeTransform);

            //Store the coordinates of the scissor rectangle
            //Start by setting them to the outside of the screen
            float scissorLeft = -1.0f;
            float scissorRight = 1.0f;

            float scissorBottom = -1.0f;
            float scissorTop = 1.0f;

            //r is the radius of the bounding sphere
            float r = boundingSphere.Radius;

            //halfNearPlaneHeight is half the height of the near plane, i.e. from the centre to the top
            float halfNearPlaneHeight = m_NearPlane * (float)Math.Tan(MathHelper.ToRadians(m_FOV * 0.5f));
            float halfNearPlaneWidth = halfNearPlaneHeight * m_AspectRatio;

            //All calculations in eye space


            //We wish to find 2 planes parallel to the Y axis which are tangent to the bounding sphere
            //of the light and pass through the origin (camera position)

            //plane normal. Of the form (x, 0, z)
            Vector3 normal;

            //Calculate the discriminant of the quadratic we wish to solve to find nx(divided by 4)
            float d = (l.Z * l.Z) * ((l.X * l.X) + (l.Z * l.Z) - r * r);

            //If d>0, solve the quadratic to get the normal to the plane
            if (d > 0.0f)
            {
                float rootD = (float)Math.Sqrt(d);

                //Loop through the 2 solutions
                for (int i = 0; i < 2; ++i)
                {
                    //Calculate the normal
                    if (i == 0)
                        normal.X = (r * l.X + rootD);
                    else
                        normal.X = (r * l.X - rootD);

                    normal.X /= (l.X * l.X + l.Z * l.Z);

                    normal.Z = (r - normal.X * l.X);
                    normal.Z /= l.Z;

                    //We need to divide by normal.X. If ==0, no good
                    if (normal.X == 0.0f)
                        continue;

                    // p is the point of tangency
                    Vector3 p;

                    p.Z = ((l.X * l.X) + (l.Z * l.Z) - r * r);
                    p.Z /= (l.Z - ((normal.Z / normal.X) * l.X));

                    //If the point of tangency is behind the camera, no good
                    if (p.Z >= 0.0f)
                        continue;

                    p.X = (-p.Z * normal.Z / normal.X);

                    //Calculate where the plane meets the near plane
                    //divide by the width to give a value in [-1, 1] for values on the screen
                    float screenX = (normal.Z * m_NearPlane / (normal.X * halfNearPlaneWidth));

                    //If this is a left bounding value (p.X<l.X) and is further right than the
                    //current value, update
                    if ((p.X < l.X) && (screenX > scissorLeft))
                        scissorLeft = screenX;

                    //Similarly, update the right value
                    if ((p.X > l.X) && (screenX < scissorRight))
                        scissorRight = screenX;
                }
            }


            //Repeat for planes parallel to the x axis
            //normal is now of the form(0, y, z)
            normal.X = 0.0f;

            //Calculate the discriminant of the quadratic we wish to solve to find ny(divided by 4)
            d = ((l.Z * l.Z) * ((l.Y * l.Y) + (l.Z * l.Z) - r * r));

            //If d>0, solve the quadratic to get the normal to the plane
            if (d > 0.0f)
            {
                float rootD = (float)Math.Sqrt(d);

                //Loop through the 2 solutions
                for (int i = 0; i < 2; ++i)
                {
                    //Calculate the normal
                    if (i == 0)
                        normal.Y = (r * l.Y + rootD);
                    else
                        normal.Y = (r * l.Y - rootD);

                    normal.Y /= (l.Y * l.Y + l.Z * l.Z);

                    normal.Z = (r - normal.Y * l.Y);
                    normal.Z /= l.Z;

                    //We need to divide by normal.Y. If ==0, no good
                    if (normal.Y == 0.0f)
                        continue;


                    //p is the point of tangency
                    Vector3 p;

                    p.Z = ((l.Y * l.Y) + (l.Z * l.Z) - r * r);
                    p.Z /= (l.Z - ((normal.Z / normal.Y) * l.Y));

                    //If the point of tangency is behind the camera, no good
                    if (p.Z >= 0.0f)
                        continue;

                    p.Y = (-p.Z * normal.Z / normal.Y);

                    //Calculate where the plane meets the near plane
                    //divide by the height to give a value in [-1, 1] for values on the screen
                    float screenY = (normal.Z * m_NearPlane / (normal.Y * halfNearPlaneHeight));

                    //If this is a bottom bounding value (p.Y<l.Y) and is further up than the
                    //current value, update
                    if ((p.Y < l.Y) && (screenY > scissorBottom))
                        scissorBottom = screenY;

                    //Similarly, update the top value
                    if ((p.Y > l.Y) && (screenY < scissorTop))
                        scissorTop = screenY;
                }
            }

            //compute the width & height of the rectangle
            size.X = (scissorRight - scissorLeft);
            size.Y = (scissorTop - scissorBottom);

            topLeft.X = scissorLeft;
            topLeft.Y = (-scissorBottom - size.Y);

        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            // Set rasterizer state
            SetRasterizerState();
        }

        public virtual void SetRasterizerState()
        {
            RasterizerState rasterizerState = new RasterizerState();

            rasterizerState.CullMode = m_CullMode;
            rasterizerState.FillMode = m_FillMode;

            // Set options
            m_RasterizerState = rasterizerState;
        }

        public virtual float GetDistance()
        {
            return 0.0f;
        }

        #region Projection
        public Vector3 Project(Viewport viewPort, Vector3 pos)
        {
            // Projection from 3D to 2D space
            return viewPort.Project(pos, Projection, View, Matrix.CreateWorld(Position, Transform.Forward, Transform.Up));
        }
        #endregion

        #region Properties
        public Vector3 Origin { get; set; }

        public Quaternion Rotation { get { return m_Rotation; } set { m_Rotation = value; } }
        public Vector3 RotationDegrees
        {
            set
            {
                m_Pitch = value.X;
                m_Yaw = value.Y;
                m_Roll = value.Z;
                m_Rotation = Quaternion.CreateFromYawPitchRoll(m_Yaw, m_Pitch, m_Roll);
            }
        }
        public float RotX { get { return m_Rotation.X; } set { m_Rotation.X = value; } }
        public float RotY { get { return m_Rotation.Y; } set { m_Rotation.Y = value; } }
        public float RotZ { get { return m_Rotation.Z; } set { m_Rotation.Z = value; } }
        public float RotW { get { return m_Rotation.W; } set { m_Rotation.W = value; } }

        public float Roll { get { return m_Roll; } set { m_Roll = value; } }
        public float RollSpeed { get { return m_RollSpeed; } set { m_RollSpeed = value; } }

        public float Yaw { get { return m_Yaw; } set { m_Yaw = value; } }
        public float YawSpeed { get { return m_YawSpeed; } set { m_YawSpeed = value; } }

        public float Pitch { get { return m_Pitch; } set { m_Pitch = value; } }
        public float PitchSpeed { get { return m_PitchSpeed; } set { m_PitchSpeed = value; } }

        public Vector3 RotationSpeeds
        {
            set
            {
                m_PitchSpeed = value.X;
                m_YawSpeed = value.Y;
                m_RollSpeed = value.Z;
            }
        }

        public float NearPlane { get { return m_NearPlane; } set { m_NearPlane = value; } }
        public float FarPlane { get { return m_FarPlane; } set { m_FarPlane = value; } }

        public float FOV { get { return m_FOV; } }
        public float AspectRatio { get { return m_AspectRatio; } set { m_AspectRatio = value; } }

        public Matrix View 
        { 
            get 
            { 
                return m_EyeTransform;
            }
            set
            {
                m_EyeTransform = value;

                // Generate the frustum
                GenerateFrustum();

                // Generate projection matrix
                //GeneratePerspectiveProjectionMatrix(45);
                m_ProjectionDirty = true;
            }
        }
        public Matrix Projection 
        { 
            get 
            { 
                return m_ProjectionMatrix; 
            }
            set
            {
                m_ProjectionMatrix = value;

                // Generate the frustum
                GenerateFrustum();

                // Generate projection matrix
                //GeneratePerspectiveProjectionMatrix(45);
                m_ProjectionDirty = true;
            }
        }

        public override Matrix Transform 
        {
            get { return m_Transform; }
            set 
            {
                m_Transform = value;
                View = Matrix.Invert(m_Transform);
                m_EyeProjectionTransform = Matrix.Multiply(m_EyeTransform, m_ProjectionMatrix);

                Frustum.Matrix = (m_EyeTransform * m_ProjectionMatrix);
            } 
        }

        public override void SetFocus(IFocusable focus)
        {
            if ( Focus != focus )
                Focus = (focus as IFocusable3D);

            if (Focus != null)
                Focus.Focused = true;
        }

        public Matrix ViewProjection { get { return m_ViewProjection; } set { m_ViewProjection = value; } }
        public Vector3 Up { get { return Transform.Up; } }
        public Vector3 Right { get { return Transform.Right; } }

        public BoundingFrustum Frustum { get; private set; }
        public BoundingFrustum ShadowFrustum { get; private set; }
        public IFocusable3D Focus 
        { 
            get 
            { 
                return (IFocus as IFocusable3D); 
            } 
            set 
            {
                if (IFocus != value)
                {
                    IFocusable focusBeforeSet = IFocus;

                    // If there was no focus before setting then set the initial camera position
                    if (focusBeforeSet != null)
                        focusBeforeSet.Focused = false;

                    // Now set the new focus
                    IFocus = value;

                    // If there is currently a focus then defocus it
                    if (IFocus != null)
                    {
                        IFocus.Focused = true;

                        // Set the target that the camera is at when a new focus target is set
                        PositionOnTargetSet = CurrentTargetPosition;

                        // Set to tween
                        m_Tween = true;
                    }
                }

                // Set movement velocity to 0 
                //Velocity *= 0.5f;
            } 
        }

        public override void OnChangedTo(BaseCamera previousCamera)
        {
            OnEnter();
        }

        public void TweenTo( Vector3 startPosition, Vector3 position, float velocityFactor)
        {
            // Set to move back to the start position
            Target = position;

            // Half the speed to give less of a jolt
            Velocity *= velocityFactor;

            // Set to tween
            m_Tween = true;

            // Set the current target position
            CurrentTargetPosition = startPosition;

            // Position the target is at when tweened to
            PositionOnTargetSet = CurrentTargetPosition;
        }

        // Specified target
        public virtual Vector3 Target 
        { 
            get 
            {
                Vector3 toRet = Vector3.Zero;

                if ( Focus != null )
                    toRet = Focus.WorldMatrix.Translation;

                return toRet; 
            } 
            set { } 
        }
        public Vector3 CurrentTargetPosition { get; set; }
        public Vector3 PositionOnTargetSet { get; set; }

        public float DistanceToTarget { get { return (Target - CurrentTargetPosition).Length(); } }
        public CullMode CullMode { get { return m_CullMode; } set { m_CullMode = value; } }
        public RasterizerState RasterizerState { get { return m_RasterizerState; } }
        #endregion
    }
}
