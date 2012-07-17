using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

/* JigLibX libraries */
using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

namespace Psynergy.Physics
{
    public interface JibLibXActor : PhysicsActor
    {
        Body Body { get; }
        CollisionSkin Skin { get; }
    }
}
