﻿//===================================================================
// This file contains the Particle System used to generate the effect
// for the DPSF Splash Screen.
//
// How To Use:
//  - Call the AutoInitialize() function to load the particle system.
//  - Clear the screen using the color of the BackgroundColor property.
//  - Draw the particle system. You do not need to specify the World, 
//  View, and Projection matrices as they are specified automatically
//  when the particle system is loaded.
//  - Check the SplashScreenIsDonePlaying property to determine when
//  the Splash Screen in finished and you can move to the next screen
//  to continue loading your game.
//
// NOTE: The particle system looks for the DPSFLogo.png image file in
//  a folder called "Textures" in the Content directory.  If you do 
//  not have the image stored there, you will need to change the
//  msTextureAssetName variable to use the correct path.
//===================================================================

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
	// Create a new type of Particle for this Particle System
	class DPSFSplashScreenParticle : DefaultAnimatedTexturedQuadParticle
	{
		// We need another variable to hold the Particle's untransformed Position (it's Emitter Independent Position)
		public Vector3 sImagePosition;
		public Quaternion cImageOrientation;
		public Vector3 sRotation;
		public bool bGoingToImagePosition;
		public bool bReachedImagePosition;
		public bool bReachedImageOrientation;
		public Quaternion cOrientationBeforeAutomaticMovement;

		public DPSFSplashScreenParticle()
		{
			Reset();
		}

		public override void Reset()
		{
			base.Reset();
			sImagePosition = Vector3.Zero;
			cImageOrientation = Quaternion.Identity;
			sRotation = Vector3.Zero;
			bGoingToImagePosition = bReachedImagePosition = bReachedImageOrientation = false;
			cOrientationBeforeAutomaticMovement = Quaternion.Identity;
		}

		public override void CopyFrom(DPSFParticle ParticleToCopy)
		{
			// Cast the Particle to the type it really is
			DPSFSplashScreenParticle cParticleToCopy = (DPSFSplashScreenParticle)ParticleToCopy;

			base.CopyFrom(cParticleToCopy);
			sImagePosition = cParticleToCopy.sImagePosition;
			cImageOrientation = cParticleToCopy.cImageOrientation;
			sRotation = cParticleToCopy.sRotation;
			bGoingToImagePosition = cParticleToCopy.bGoingToImagePosition;
			bReachedImagePosition = cParticleToCopy.bReachedImagePosition;
			bReachedImageOrientation = cParticleToCopy.bReachedImageOrientation;
			cOrientationBeforeAutomaticMovement = cParticleToCopy.cOrientationBeforeAutomaticMovement;
		}
	}

	/// <summary>
	/// Create a new Particle System class that inherits from a
	/// Default DPSF Particle System
	/// </summary>
	class DPSFSplashScreenParticleSystem : DPSFDefaultAnimatedTexturedQuadParticleSystem<DPSFSplashScreenParticle, DefaultTexturedQuadParticleVertex>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DPSFSplashScreenParticleSystem(Game cGame) : base(cGame) { }

		//===========================================================
		// Structures and Variables
		//===========================================================

		// The location of the DPSFLogo.png image file
		private const string msTextureAssetName = "Textures/DPSFLogo";

		// Effect Settings (do not change these)
		private const int miNumberOfRows = 64;
		private const int miNumberOfColumns = 64;
		private const int miWidthOfCompositeImage = 256;
		private const int miHeightOfCompositeImage = 128;
		private const float mfTimeBeforeMovingToImagePosition = 1.0f;
		private const float mfTotalTimeForIntro = 4.0f;
		private const int miNumberOfParticlesToEmitPerSecond = 10000;
		
		// Class variables
		private const int miNumberOfParticlesRequired = miNumberOfRows * miNumberOfColumns;
		private CullMode meDefaultCullMode;
		private bool mbIntroIsDonePlaying = false;

		// Camera Settings
		private Matrix msViewMatrix = Matrix.CreateLookAt(new Vector3(0, 50, 300), new Vector3(0, 50, 0), Vector3.Up);
		private Matrix msProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1.33333f, 1, 10000);
		private Color msBackgroundColor = Color.Black;

		//===========================================================
		// Overridden Particle System Functions
		//===========================================================
		protected override void InitializeRenderProperties()
		{
			base.InitializeRenderProperties();
			RenderProperties.RasterizerState.CullMode = CullMode.None;
			RenderProperties.DepthStencilState.DepthBufferWriteEnable = true;
		}

		protected override void SetEffectParameters()
		{
			base.SetEffectParameters();

			// Show only the Textures Color (do not blend with Particle Color)
			Effect.Parameters["xColorBlendAmount"].SetValue(0.0f);
		}

		//===========================================================
		// Initialization Functions
		//===========================================================
		public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{
			InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, miNumberOfParticlesRequired, 
					miNumberOfParticlesRequired, UpdateVertexProperties, msTextureAssetName);
			LoadDPSFIntro();
		}

		public void LoadDPSFIntro()
		{
			// Setup the Camera and specify the number of Particles to Emit Per Second
			SetWorldViewProjectionMatrices(Matrix.Identity, msViewMatrix, msProjectionMatrix);
			Emitter.ParticlesPerSecond = miNumberOfParticlesToEmitPerSecond;

			RemoveAllParticles();

			ParticleInitializationFunction = InitializeParticleDPSFIntro;
			ParticleEvents.RemoveAllEvents();
			ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration);
			ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
			ParticleEvents.AddEveryTimeEvent(RotateAroundOrigin, 100);
			ParticleEvents.AddEveryTimeEvent(MoveToFinalPosition, 200);

			ParticleSystemEvents.AddTimedEvent(mfTotalTimeForIntro, MarkSplashScreenAsDonePlaying);
		}

		public void InitializeParticleDPSFIntro(DPSFSplashScreenParticle cParticle)
		{
			// Fill in this Particles information about where it should be to form the composite image
			SetParticlePositionWidthHeightAndTextureCoordinatesToFormImage(cParticle);

			cParticle.Lifetime = 0;
			cParticle.RotationalVelocity = new Vector3(RandomNumber.Between(0, MathHelper.TwoPi), RandomNumber.Between(0, MathHelper.TwoPi), RandomNumber.Between(0, MathHelper.TwoPi));
			cParticle.sRotation = new Vector3(0, MathHelper.TwoPi, 0);
			cParticle.Velocity = new Vector3(0, RandomNumber.Next(1, 100), 0);

			// Have some particles start at the bottom of the screen, and others start at the top
			if (NumberOfActiveParticles % 2 == 0)
			{
				cParticle.Position = new Vector3(RandomNumber.Next(-100, 100), 0, RandomNumber.Next(-100, 100));
			}
			else
			{
				cParticle.Position = new Vector3(RandomNumber.Next(-100, 100), 100, RandomNumber.Next(-100, 100));
				cParticle.Velocity *= -1;
			}
		}

		//===========================================================
		// Particle Update Functions
		//===========================================================
		public void RotateAroundOrigin(DPSFSplashScreenParticle cParticle, float fElapsedTimeInSeconds)
		{
			// Calculate the Rotation Matrix to Rotate the Particle by
			Vector3 sAmountToRotate = cParticle.sRotation * fElapsedTimeInSeconds;
			Matrix sRotation = Matrix.CreateFromYawPitchRoll(sAmountToRotate.Y, sAmountToRotate.X, sAmountToRotate.Z);
			
			// Rotate the Particle around the origin
			cParticle.Position = PivotPoint3D.RotatePosition(sRotation, new Vector3(0, cParticle.Position.Y, 0), cParticle.Position);
		}

		public void MoveToFinalPosition(DPSFSplashScreenParticle cParticle, float fElapsedTimeInSeconds)
		{
			// If it is not time for the Particle to go to its final destination yet
			if (cParticle.ElapsedTime < mfTimeBeforeMovingToImagePosition ||
				(cParticle.bReachedImagePosition && cParticle.bReachedImageOrientation))
			{
				// Exit without doing anything
				return;
			}

			// If the Particle isn't going to its Final Position yet, but it should be
			if (!cParticle.bGoingToImagePosition)
			{
				// Make sure the Particle doesn't move on its own anymore (this function now controls it)
				cParticle.Acceleration = Vector3.Zero;
				cParticle.RotationalVelocity = Vector3.Zero;
				cParticle.RotationalAcceleration = Vector3.Zero;
				cParticle.sRotation = Vector3.Zero;

				// Make the Particle move towards its final destination
				cParticle.Velocity = cParticle.sImagePosition - cParticle.Position;

				Quaternion cRotationRequired = Orientation3D.GetRotationTo(Orientation3D.GetNormalDirection(cParticle.Orientation), Orientation3D.GetNormalDirection(cParticle.cImageOrientation));
				cRotationRequired *= Orientation3D.GetRotationTo(Orientation3D.GetUpDirection(cParticle.Orientation), Orientation3D.GetUpDirection(cParticle.cImageOrientation));
				cParticle.cOrientationBeforeAutomaticMovement = cRotationRequired;

				cParticle.bGoingToImagePosition = true;
			}

			// If the Particle hasn't made it to its Image Position yet
			if (!cParticle.bReachedImagePosition)
			{
				// Calculate the Vector from the current position to the Image Position
				Vector3 sVectorToFinalPosition = cParticle.sImagePosition - cParticle.Position;
				float fLength = sVectorToFinalPosition.LengthSquared();

				// If the Particle is pretty much in its final Position
				if (fLength < 1)
				{
					cParticle.Velocity = Vector3.Zero;
					cParticle.Position = cParticle.sImagePosition;
					cParticle.bReachedImagePosition = true;
				}
				// Else if the Particle is still fairly far from its final Position
				else if (fLength > 500)
				{
					cParticle.Velocity = sVectorToFinalPosition * 6;
				}
			}

			// If the Particle hasn't made it to its Image Orientation yet
			if (!cParticle.bReachedImageOrientation)
			{
				float fLerpAmount = (cParticle.ElapsedTime - mfTimeBeforeMovingToImagePosition) / 1.0f;
				if (fLerpAmount > 1.0f)
				{
					cParticle.Orientation = cParticle.cImageOrientation;
					cParticle.bReachedImageOrientation = true;
				}
				else
				{
					cParticle.Orientation = Quaternion.Slerp(cParticle.cOrientationBeforeAutomaticMovement, cParticle.cImageOrientation, fLerpAmount);
				}
			}
		}

		//===========================================================
		// Particle System Update Functions
		//===========================================================
		public void MarkSplashScreenAsDonePlaying(float fElapsedTimeInSeconds)
		{
			mbIntroIsDonePlaying = true;
		}

		//===========================================================
		// Other Particle System Functions
		//===========================================================

		// This function sets the Particle Properties so that when all Particles are viewed together,
		// they form the complete image of the texture
		private void SetParticlePositionWidthHeightAndTextureCoordinatesToFormImage(DPSFSplashScreenParticle cParticle)
		{
			// Calculate how big the Particles should be to achieve the desired size
			int iRequiredParticleWidth = miWidthOfCompositeImage / miNumberOfColumns;
			int iRequiredParticleHeight = miHeightOfCompositeImage / miNumberOfRows;

			// Calculate how big one Row and Column from the texture should be
			int iTextureRowSize = Texture.Height / miNumberOfRows;
			int iTextureColumnSize = Texture.Width / miNumberOfColumns;

			// Calculate which Row and Column this Particle should be at
			int iRow = NumberOfActiveParticles / miNumberOfColumns;
			int iColumn = NumberOfActiveParticles % miNumberOfColumns;

			// Calculate this Particle's Position to create the full Image
			int iY = (miNumberOfRows * iRequiredParticleHeight) - ((iRow * iRequiredParticleHeight) + (iRequiredParticleHeight / 2));
			int iX = (iColumn * iRequiredParticleWidth) + (iRequiredParticleWidth / 2);
			iX -= (miNumberOfColumns * iRequiredParticleWidth) / 2;    // Center the image

			// Calculate this Particle's Texture Coordinates to use
			float fTextureTop = (iRow * iTextureRowSize) / (float)Texture.Height;
			float fTextureLeft = (iColumn * iTextureColumnSize) / (float)Texture.Width;
			float fTextureBottom = ((iRow * iTextureRowSize) + iTextureRowSize) / (float)Texture.Height;
			float fTextureRight = ((iColumn * iTextureColumnSize) + iTextureColumnSize) / (float)Texture.Width;

			// Set the Particle's Properties to Form the complete Image
			cParticle.Width = iRequiredParticleWidth;
			cParticle.Height = iRequiredParticleHeight;
			cParticle.sImagePosition = new Vector3(iX, iY, 0);
			cParticle.cImageOrientation = Orientation3D.GetQuaternionWithOrientation(Vector3.Forward, Vector3.Up);
			cParticle.NormalizedTextureCoordinateLeftTop = new Vector2(fTextureLeft, fTextureTop);
			cParticle.NormalizedTextureCoordinateRightBottom = new Vector2(fTextureRight, fTextureBottom);
		}

		/// <summary>
		/// Get / Set if the Splash Screen is done playing
		/// </summary>
		public bool SplashScreenIsDonePlaying
		{
			get { return mbIntroIsDonePlaying; }
			set { mbIntroIsDonePlaying = value; }
		}

		/// <summary>
		/// Get the Background Color that should be used for the Splash Screen
		/// </summary>
		public Color BackgroundColor
		{
			get { return msBackgroundColor; }
		}
	}
}