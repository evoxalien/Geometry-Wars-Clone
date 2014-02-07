using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using BloomPostprocess;

namespace GeometryWars
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameRoot : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static GameRoot Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
        public static ParticleManager<ParticleState> ParticleManager { get; private set; }
        public static GameTime GameTime;
        public static Grid grid;
        int frameCounter;
        int FPS;
        float elapsedTime;

        BloomComponent bloom;
        public GameRoot()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Instance = this;

            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1600;
            //this.IsFixedTimeStep = false;
            graphics.ApplyChanges();

            bloom = new BloomComponent(this);
            Components.Add(bloom);
            bloom.Settings = new BloomSettings(null, 0.05f, 2, 2, 1, 1.5f, 1);
            
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            int gridScale = 35;
            Input.DevModeParticles = true;
            Input.DevModeGrid = true;
            Vector2 gridSpace = new Vector2(gridScale, gridScale);
            // TODO: Add your initialization logic here
            GameTime = new GameTime();
            //grid = new Grid(new Rectangle((int)(-gridSpace.X),(int) (-gridSpace.Y), Viewport.Width + (int)(gridSpace.X), Viewport.Height + (int)(gridSpace.Y)), new Vector2(gridSpace.X, gridSpace.Y));
            grid = new Grid(new Rectangle((int)(-gridSpace.X) * (2), (int)(-gridSpace.Y) * (2), Viewport.Width + (int)(gridSpace.X) * 3, Viewport.Height + (int)(gridSpace.Y) * 3), new Vector2(gridSpace.X, gridSpace.Y));

            base.Initialize();
            EntityManager.Add(PlayerShip.Instance);
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Sound.Music);
            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
 
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // TODO: use this.Content to load your game content here
            Art.Load(Content);
            Sound.Load(Content);
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Input.Update();

            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            // TODO: Add your update logic here
            
            
            base.Update(gameTime);
            if (!Input.GamePause)
            {
                PlayerStatus.Update();
                grid.Update();
                EnemySpawner.Update();
                EntityManager.Update();
                ParticleManager.Update();
            }

            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter++;

            if (elapsedTime > 1)
            {
                FPS = frameCounter;
                frameCounter = 0;
                elapsedTime = 0;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {


            bloom.BeginDraw();
            GraphicsDevice.Clear(Color.Black);
            

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            if (Input.DevModeGrid)
                grid.Draw(spriteBatch);
            if (Input.DevModeParticles)
                ParticleManager.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);

            if (PlayerStatus.isGameOver)
            {
                string text = "Game Over\n" +
                    "Your Score: " + PlayerStatus.Score + "\n" +
                    "High Score: " + PlayerStatus.HighScore;
                Vector2 textSize = Art.Font.MeasureString(text);
                spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
            }

            if (Input.GamePause)
            {
                string text = "Game Paused\n";
                Vector2 textSize = Art.Font.MeasureString(text);
                spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
            }

            int print = 0;
            DrawRightAlignedString("Score: " + PlayerStatus.Score, 5 + (30 * print++));
            DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 5 + (30 * print++));
            if (Input.DevMode)
            {
                DrawRightAlignedString("FPS: " + FPS.ToString(), +(30 * print++));
                DrawRightAlignedString("Entities: " + EntityManager.Count, +(30 * print++));
                if(Input.DevModeParticles)
                    DrawRightAlignedString("Particles: " + ParticleManager.ParticleCount, +(30 * print++));
                DrawRightAlignedString("Weapon Level: " + PlayerShip.WeaponLevel, +(30 * print++));
                
                if (Input.GodMode)
                    DrawRightAlignedString("GOD MODE", +(30 * print++));
            }
            
            DrawLeftAlignedString("Lives: " + PlayerStatus.Lives, 5);
            

            //Draw Cursor
            EntityManager.Draw(spriteBatch);
            spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
            spriteBatch.End();

            

            base.Draw(gameTime);
        }

        private void DrawRightAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
        }

        private void DrawLeftAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            spriteBatch.DrawString(Art.Font, text, new Vector2(5, y), Color.White);
        }


        public static void WriteFile(string filename, string stuff)
        {
            File.WriteAllText(filename, stuff);
        }
    }
}
