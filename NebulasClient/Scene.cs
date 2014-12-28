using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nebulas
{
    public class Camera
    {

        Nebulas.Objects.Prop mTarget;
        Nebulas.Math.Vec3 mTranslation; //z Zoom
        Double mRotation;
    }
    public class Scene
    {
        enum SectorGridPosition
        {
            TopLeft = 0,
            TopMid,
            TopRight,
            MidLeft,
            MidMid,
            MidRight,
            BottomLeft,
            BottomMid,
            BottomRight
        }
        //Sector[] mGrid;
        Camera mCamera;
        Nebulas.Events.EventListener mEventListener;
        void Shift(SectorGridPosition direction)
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }
    }
    
}
