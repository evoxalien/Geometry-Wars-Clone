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
    abstract class Entity
    {
        protected Texture2D image;
        // The tint of the image.
        // This will also allow us to change the transparency.
        protected Color color = Color.White;

        public Vector2 Position, Velocity;
        public float Orientation;
        public float Radius = 20; // used for circular collision detection
        public bool IsExpired;    // true if the enitity was destroyed and should be deleted.

        public Vector2 Size
        {
            get
            {
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }

        public abstract void Update();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, 0, 0);
        }

    }
}
