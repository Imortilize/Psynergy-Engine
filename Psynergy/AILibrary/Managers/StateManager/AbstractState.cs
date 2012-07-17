using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

namespace Psynergy.AI
{
    abstract public class AbstractState<T> : GameObject
    {
        abstract public override void Initialise();
        abstract public override void Reset();
        abstract public void OnEnter(T objectRef);
        abstract public void Update(GameTime deltaTime, T objectRef);
        abstract public void Render(GameTime deltaTime, T objectRef);
        abstract public void OnExit(T objectRef);
    }
}
