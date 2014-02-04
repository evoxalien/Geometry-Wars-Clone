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
    class Enemy : Entity
    {

        public static Random rand = new Random();
        private int timeUntilStart = 60;
        public bool IsActive { get { return timeUntilStart <= 0; } }
        private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        public static int PointValue { get; set; }

        public Enemy(Texture2D image, Vector2 position)
        {
            this.image = image;
            Position = position;
            Radius = image.Width / 2f;
            color = Color.Transparent;
        }

        public override void Update()
        {
            if (timeUntilStart <= 0)
            {
                //Enemy Behavior logic goes here
                if(timeUntilStart <= 0)
                    ApplyBehaviors();
            }
            else
            {
                timeUntilStart--;
                color = Color.White * (1 - timeUntilStart / 60f);
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

            Velocity *= 0.75f;
        
        }

        IEnumerable<int> FollowPlayer(float acceleration = 1f)
        {
            /*while (true)
            {
                Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();
                yield return 0;
            }*/
            while (true)
            {
                Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
                if (Velocity != Vector2.Zero)
                {
                    Orientation = Velocity.ToAngle();
                }
                yield return 0;
            }
        }
/*
        IEnumerable<int> DodgeBullets(float acceleration = 1f)
        {
            while (true)
            {
                for (int i = 0; i < EntityManager.bullets.Count; i++)
                Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();
                yield return 0;
            }
        }
        */
        IEnumerable<int> MoveInASquare()
        {
            const int framesPerSide = 30;
            while (true)
            {
                //Move Right for 30 frames
                for (int i = 0; i < framesPerSide; i++)
                {
                    Velocity = Vector2.UnitX;
                    yield return 0;
                }
                //Move Down
                for (int i = 0; i < framesPerSide; i++)
                {
                    Velocity = Vector2.UnitY;
                    yield return 0;
                }
                //Move Left
                for (int i = 0; i < framesPerSide; i++)
                {
                    Velocity = -Vector2.UnitX;
                    yield return 0;
                }
                //Move up
                for (int i = 0; i < framesPerSide; i++)
                {
                    Velocity = -Vector2.UnitY;
                    yield return 0;
                }
            }
        }

        IEnumerable<int> MoveRandomly()
        {

            float direction = rand.NextFloat(0, MathHelper.TwoPi);

            while (true)
            {
                direction += rand.NextFloat(-0.1f, 0.1f);
                direction = MathHelper.WrapAngle(direction);

                for (int i = 0; i < 6; i++)
                {
                    Velocity += MathUtil.FromPolar(direction, 0.4f);
                    Orientation -= 0.05f;

                    var bounds = GameRoot.Viewport.Bounds;
                    bounds.Inflate(-image.Width, -image.Height);

                    //if the enemy is outside the bounds, make it move away from the edge
                    if (!bounds.Contains(Position.ToPoint()))
                        direction = (GameRoot.ScreenSize / 2 - Position).ToAngle() + rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

                    yield return 0;
                }
            }

        }

        private void AddBehaviour(IEnumerable<int> behavior)
        {
            behaviours.Add(behavior.GetEnumerator());
        }

        private void ApplyBehaviors()
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                if(!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);
            }
        }

        public void WasShot()
        {
            PlayerStatus.AddPoints(PointValue);
            PlayerStatus.IncreaseMultiplier();
            //Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);
            IsExpired = true;
        }

        public void HandleCollision(Enemy other)
        {
            var d = Position - other.Position;
            Velocity += 10 * d / (d.LengthSquared() + 1);
        }

        //Defined Types of Enemies!

        public static Enemy CreateSeeker(Vector2 position)
        {
            var enemy = new Enemy(Art.Seeker, position);
            enemy.AddBehaviour(enemy.FollowPlayer());
            PointValue = 2;

            return enemy;
        }

        public static Enemy CreateWanderer(Vector2 position)
        {
            var enemy = new Enemy(Art.Wanderer, position);
            enemy.AddBehaviour(enemy.MoveRandomly());
            PointValue = 1;

            return enemy;
        }

        public static Enemy CreateSquareDance(Vector2 position)
        {
            var enemy = new Enemy(Art.Wanderer, position);
            enemy.AddBehaviour(enemy.MoveInASquare());
            //enemy.AddBehaviour(enemy.FollowPlayer());
            PointValue = 1;
            return enemy;
        }
    }
}
