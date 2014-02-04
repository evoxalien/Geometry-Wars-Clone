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
    class BlackHole : Entity
    {
        private static Random rand = new Random();

        private int hitpoints = 10;

        public BlackHole(Vector2 position)
        {
            image = Art.BlackHole;
            Position = position;
            Radius = image.Width / 2f;
        }

        public override void Update()
        {
            var entities = EntityManager.GetNearbyEntities(Position, 250);

            foreach (var entity in entities)
            {
                Entity entity1 = (Entity)entity;
                if (entity1 is Enemy && !(entity1 as Enemy).IsActive)
                    continue;

                // bullets are repelled by black holes and everything else is attracted
                if (entity1 is Bullet)
                    entity1.Velocity += (entity1.Position - Position).ScaleTo(0.3f);
                else
                {
                    var dPos = Position - entity1.Position;
                    var length = dPos.Length();

                    entity1.Velocity += dPos.ScaleTo(MathHelper.Lerp(2, 0, length / 250f));
                }
            }
        }

        public void WasShot()
        {
            hitpoints--;
            if (hitpoints < 0)
                IsExpired = true;
        }

        public void Kill()
        {
            hitpoints = 0;
            WasShot();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = 1 + 0.1f * (float)Math.Sin(10 * GameRoot.GameTime.TotalGameTime.TotalSeconds);
        spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, scale, 0, 0);
        }

        
    }
}
