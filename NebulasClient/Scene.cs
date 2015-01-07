using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nebulas
{
    public class Camera : Nebulas.Events.EventListener
    {

        Nebulas.Objects.Prop mTarget;
        Nebulas.Math.Vec3 mTranslation; //z Zoom
        Double mRotation;
        Rectangle mViewport;

    }
    public class Scene : Nebulas.Events.EventListener
    {
        
        protected Camera mCamera;
        protected List<Nebulas.Objects.Prop> mChildren;
        
        public Int32 AddChild(Nebulas.Objects.Prop prop)
        {
            mChildren.Add(prop);
            return prop.GetHandle();
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }
    }

    public class Menu : Nebulas.Events.EventListener
    {
        SpriteFont mFont;
        static Color mBackground = Color.CornflowerBlue;
        String[] mChoices;
        Int32 mSelected;
        Int32 mOffset;
        
        static Color mSelected = Color.Yellow;

        public Menu()
        {
            mFont = ?;
            mChoices = new String[4];
            mOffset = 0;
            this.AddResponse("KEY_UP", "moveMenu.rb");
            this.AddResponse("KEY_DOWN", "moveMenu.rb");
            this.AddResponse("KEY_ENTER", "selectConnection.rb");
            this.AddResponse("KEY_INPUT", "inputConnection.rb");
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
    
}
