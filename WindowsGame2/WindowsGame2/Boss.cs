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
    class Boss : Entity
    {
        public static Random rand = new Random();
        private int timeUntilStart = 60;
        public bool IsActive { get { return timeUntilStart <= 0; } }
        private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        private int PointValue { get; set; }
        private int EnemyType = 0;
        private int Health = 20;
        
        public Boss(Texture2D image, Vector2 position)
        {
            this.image = image;
            Position = position;
            Radius = image.Width / 2f;
            color = Color.Transparent;
        }
        
        public void Effect()
        {
            for (int i = 0; i < 20; i++)
            {
                float speed = 6f * (1f - 1 / rand.NextFloat(1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };
                GameRoot.grid.ApplyExplosiveForce(2.5f, Position, 75);
                if (EnemyType == 1)
                    GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.FromNonPremultiplied(255, 255, 128, 155), 190, new Vector2(1.0f), state);
                else if (EnemyType == 2)
                    GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.FromNonPremultiplied(255, 128, 128, 155), 190, new Vector2(1.0f), state);
                else
                    GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightGreen, 190, new Vector2(1.0f), state);
            }
        }

        public void WasShot()
        {
            Health--;
            PlayerStatus.AddPoints(PointValue);
            for(int i = 0; i < 5; i++)
                PlayerStatus.IncreaseCombo();
            //Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);
            if (Health <= 0)
            {
                IsExpired = true;
                EnemySpawner.FlappyKingSpawned = false;
                EnemySpawner.SpawnFlappyKing = false;
                EntityManager.enemies.ForEach(x => x.WasShot());
                EnemySpawner.bosseskilled++;
            }
            Effect();

        }


        IEnumerable<int> FlappyBirdMotion()
        {

            float direction = rand.NextFloat(0, MathHelper.TwoPi);
            int framesBetweenDips = rand.Next(200,500);

            while (true)
            {
                float LastOrientation = Orientation;
                
                framesBetweenDips = rand.Next(200, 500);
                for (int i = 0; i < framesBetweenDips / 3 + rand.Next(0, 100); i++)
                {
                    Velocity += (-Vector2.UnitY + Vector2.UnitX) / 2;
                    Orientation = ((Vector2.UnitY + Vector2.UnitX) / 2).ToAngle();
                    yield return 0;
                }
                framesBetweenDips = rand.Next(200, 500);
                for (int i = 0; i < framesBetweenDips / 2 + rand.Next(0, 10); i++)
                {
                    Velocity += (Vector2.UnitY + Vector2.UnitX) / 2;
                    Orientation = ((-Vector2.UnitY + Vector2.UnitX) / 2).ToAngle();
                    yield return 0;
                }

                if (Velocity != Vector2.Zero)
                {
                    Orientation = Velocity.ToAngle();
                }
                Velocity *= .75f;
            }

        }

        public static Boss FlappyKing()
        {

            Vector2 position = new Vector2(GameRoot.Viewport.Width - 200, GameRoot.Viewport.Height/2);
  
            var boss = new Boss(Art.FlappyKing, position);
            boss.AddBehaviour(boss.FlappyBirdMotion());
            boss.Radius = 150;
            boss.PointValue = 3;
            boss.EnemyType = 1;
            boss.Health = 500;
            EntityManager.enemies.ForEach(x => x.WasShot());
            return boss;
        }

        
        public void HandleCollision(Enemy other)
        {
            //var d = Position - other.Position;
            //Velocity += 10 * d / (d.LengthSquared() + 1);
        }
        
        private void AddBehaviour(IEnumerable<int> behavior)
        {
            behaviours.Add(behavior.GetEnumerator());
        }

        private void ApplyBehaviors()
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                if (!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);
            }
        }

        public override void Update()
        {
            if (timeUntilStart <= 0)
            {
                //Enemy Behavior logic goes here
                if (timeUntilStart <= 0)
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
    }
}
