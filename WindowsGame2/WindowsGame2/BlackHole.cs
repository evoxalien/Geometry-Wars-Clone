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
            Velocity = new Vector2(0.0f, 0.0f);
        }

        public override void Update()
        {
            Console.WriteLine(Velocity.X);
            var entities = EntityManager.GetNearbyEntities(Position, 250);

            foreach (Entity entity in entities)
            {
                if (entity is Enemy && !(entity as Enemy).IsActive)
                    continue;

                // bullets are repelled by black holes and everything else is attracted
                if (entity is Bullet)
                {
                    entity.Velocity += (entity.Position - Position).ScaleTo(0.3f);
                    //var d = Position - entity1.Position;
                    //Velocity += 10 * d / (d.LengthSquared() + 1);

                    Velocity += (Position - entity.Position).ScaleTo(0.0001f);

                }
                else if (!(entity is BlackHole))
                {
                    var dPos = Position - entity.Position;
                    var length = dPos.Length();

                    entity.Velocity += dPos.ScaleTo(MathHelper.Lerp(2, 0, length / 250f));
                }
            }
            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);
            //Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

            //Velocity *= 0.75f;
            
        }

        public void WasShot()
        {
            hitpoints--;
            if (hitpoints < 0)
                IsExpired = true;

            for (int i = 0; i < 120; i++)
            {
                float speed = 10f * (1f - 1 / rand.NextFloat(1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.FromNonPremultiplied(128, 255, 255, 155), 190, new Vector2(1.0f), state);
            }
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
