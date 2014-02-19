using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars
{
    static class Art
    {
        public static Texture2D Player { get; private set; }
        public static Texture2D Seeker { get; private set; }
        public static Texture2D Wanderer { get; private set; }
        public static Texture2D Bullet { get; private set; }
        public static Texture2D Pointer { get; private set; }
        public static Texture2D BlackHole { get; private set; }
        public static Texture2D LineParticle { get; private set; }
        public static Texture2D Pixel { get; private set; }
        public static Texture2D Glow { get; private set; }

        public static Texture2D FlappyBirdMinion { get; private set; }
        public static Texture2D FlappyKing { get; private set; }

        public static SpriteFont Font { get; private set; }

        public static void Load(ContentManager content)
        {
            Player = content.Load<Texture2D>("Art/Player");
            Bullet = content.Load<Texture2D>("Art/Bullet");
            Seeker = content.Load<Texture2D>("Art/Seeker");
            Wanderer = content.Load<Texture2D>("Art/Wanderer");
            Pointer = content.Load<Texture2D>("Art/Pointer");
            BlackHole = content.Load<Texture2D>("Art/BlackHole");
            LineParticle = content.Load<Texture2D>("Art/LineParticle");
            Glow = content.Load<Texture2D>("Art/Glow");

            FlappyBirdMinion = content.Load<Texture2D>("Art/FlappyBird");
            FlappyKing = content.Load<Texture2D>("Art/FlappyBirdBoss");
            
            //This is for the Grid Background and PlayerShip Exhaust
            Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            Font = content.Load<SpriteFont>("Font");
        }

    }
}
