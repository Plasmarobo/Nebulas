#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;


#endregion

namespace Nebulas
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NebulasClient : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static Dictionary<String, Texture2D> sprites = new Dictionary<string,Texture2D>();
        Scene scene;
        Menu connectionMenu;
        Boolean connected;
        Nebulas.Events.EventStream eventManager;
        public NebulasClient()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            scene = null;
            connectionMenu = new Menu();
            connected = false;
            eventManager = new Nebulas.Events.EventStream();
            //Basic Event manager must handle UP DOWN ENTER and Entering 0-9 and .
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO : Load sounds or something
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // TODO: use this.Content to load your game content here
            //sprites["rocket"] = this.Content.Load<Texture2D>("TerranRocket");

            //Load SectorA
            string dirRoot = @"sectorA";
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirRoot));
            foreach (var dir in dirs)
            {
                List<string> files = new List<string>(Directory.EnumerateFiles(dir));
                foreach (var file in files)
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    string slice = Path.GetDirectoryName(dir);
                    sprites["{dirRoot}{slice}{name}"] = this.Content.Load<Texture2D>(file);
                }
                
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            this.Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //scene.Update(gameTime);
            if(connected.Equals(true))
            {
                eventManager.DispatchEvents();
            }
            else
            {

            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            // TODO: Add your drawing code here
            if (scene != null)
            {
                scene.Draw(spriteBatch, gameTime);
            }
            else
            {
                connectionMenu.Draw(spriteBatch, gameTime);
            }
            //spriteBatch.Draw(rocket, new Vector2(200,200), new Rectangle(0,0,128,128), Color.White, angle, new Vector2(64,64), 1.0f, SpriteEffects.None, 1);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        
    }
}
