using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Camera
{
    public class TargetCamera : Camera3D
    {
        public TargetCamera() : base("")
        {
        }

        public TargetCamera(String name)
            : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            // Set that it does required focus
            RequiresFocus = true;
        }

        protected override void GenerateViewMatrix(GameTime deltaTime)
        {
            Debug.Assert( Focus != null, "[Warning] - Focus object is null on target camera " + Name + "." );

            if ( Focus != null )
            {
                Vector3 forward = (Focus.transform.Position - transform.Position);
                Vector3 side = Vector3.Cross(forward, Vector3.Up);
                Vector3 up = Vector3.Cross(forward, side);

                View = Matrix.CreateLookAt(transform.Position, Target, up);
            }
        }
    }
}
