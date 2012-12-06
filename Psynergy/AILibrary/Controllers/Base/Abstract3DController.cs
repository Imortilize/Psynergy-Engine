using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

namespace Psynergy.AI
{
    abstract public class Abstract3DController<T> : Controller where T : GameObject
    {
        public Abstract3DController() : base() { }
        public Abstract3DController(T node) : base(node) { }
        abstract public void SetDesiredPosition(Vector3 desiredPos);
        abstract public void StopMovement();
        abstract public void SetDesiredRotation(Vector3 from, Vector3 to);
        abstract public Vector3 SetPosition(Vector3 position);
    }
}
