using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Camera
{
    public interface ICamera : IInterface
    {
        // The movespeed of the camera, it will tween to its destination
        float MoveSpeed { get; set; }

        // In view interface
        bool IsInView(GameObject obj);

        // Set focus
        IFocusable IFocus { get; set; }
    }

    public interface ICamera2D : IInterface
    {
        // Gets the Origin of the viewport (accounts for scale)
        Vector2 Origin { get; }

        // Gets or sets the scale of the camera
        float CameraScale { get; set; }

        // Get the screen center ( does not account for scale )
        Vector2 ScreenCenter { get; }

        // Get or sets the rotation of the camera
        float Rotation { get; set; }
    }

    public interface ICamera3D : IInterface
    {
        // Gets the Origin of the viewport
        Vector3 Origin { get; }
    }
}
