using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy
{
    public interface IFactoryProduct
    {
        object GetFactoryKey();
    }

    public interface IObject
    {
        void Initialise();
        void Load();
        void UnLoad();
        void Update(GameTime deltaTime);
        void Render(GameTime deltaTime);
    }

    public interface IFocusable
    {
        Transform transform { get; }
        bool Focused { get; set; }
    }

    public interface IFocusable2D : IFocusable
    {
        float Height { get; }
        float Width { get; }
        float Rotation { get; set; }
    }

    public interface IFocusable3D : IFocusable
    {
    }

    public interface IMenuAction
    {
        void RunAction();
    }

    public interface IRegister<T>
    {
    }
}
