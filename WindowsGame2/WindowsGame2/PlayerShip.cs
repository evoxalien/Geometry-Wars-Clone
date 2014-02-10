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
    class PlayerShip : Entity
    {
        private static PlayerShip instance;

        public static int WeaponLevel = 0;

        int framesUntilRespawn = 0;
        int cooldownFrames = 6;
        int cooldownRemaining = 0;
        public static Random rand = new Random();
        public bool IsDead { get { return framesUntilRespawn > 0; } }

        public static PlayerShip Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerShip();
                return instance;
            }
        }

        private PlayerShip()
        {
            image = Art.Player;
            Position = GameRoot.ScreenSize / 2;
            Radius = 10;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(!IsDead)
                base.Draw(spriteBatch);

          
        }

        public void Kill()
        {
            
            PlayerShip.WeaponLevel = 0;            
            PlayerStatus.RemoveLife();
            framesUntilRespawn = 60;
            framesUntilRespawn = PlayerStatus.isGameOver ? 300 : framesUntilRespawn;

           

            for (int i = 0; i < 1200; i++)
            {
                float speed = 18f * (1f - 1 / rand.NextFloat(1f, 10f));
                Color color = Color.Lerp(Color.White, Color.Yellow, rand.NextFloat(0, 1));
                var state = new ParticleState()
                {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.None,
                    LengthMultiplier = 1
                };

                GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, new Vector2(1.0f,1.0f), state);
            }
            GameRoot.grid.ApplyExplosiveForce(100f, Position, 450);
            Position = GameRoot.ScreenSize / 2;
            EnemySpawner.Reset();
     
        }

        public override void Update()
        {

            GameRoot.grid.ApplyExplosiveForce(Velocity.Length() / 2, Position, 75);
            if (IsDead)
            {
                framesUntilRespawn--;
                return;
            }
            if (PlayerStatus.isGameOver)
                PlayerStatus.Reset();

            const float speed = 8;
            Velocity = speed * Input.GetMovementDirection();
            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            var aim = Input.GetAimDirection();
            if (aim.LengthSquared() > 0 && cooldownRemaining <= 0)
            {
                cooldownRemaining = cooldownFrames;
                float aimAngle = aim.ToAngle();
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

                float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
                Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11F);
                Vector2 offset;

                if (WeaponLevel == 0)
                {
                    cooldownFrames = 4;
                    offset = Vector2.Transform(new Vector2(35, 0), aimQuat);
                    EntityManager.Add(new Bullet(Position + offset , vel + (Velocity / 2)));
                    Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
                }
                else if (WeaponLevel == 1)
                {
                    cooldownFrames = 6;

                    offset = Vector2.Transform(new Vector2(35, -8), aimQuat);
                    EntityManager.Add(new Bullet(Position + offset, vel));

                    offset = Vector2.Transform(new Vector2(35, 8), aimQuat);
                    EntityManager.Add(new Bullet(Position + offset, vel));
                    Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
                }
                else if (WeaponLevel >= 2)
                {
                    cooldownFrames = 3;
                    offset = Vector2.Transform(new Vector2(35, -8), aimQuat);
                    EntityManager.Add(new Bullet(Position + offset, vel));

                    offset = Vector2.Transform(new Vector2(35, 8), aimQuat);
                    EntityManager.Add(new Bullet(Position + offset, vel));
                    Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);

                }
            }

            if(cooldownRemaining > 0)
                cooldownRemaining --;
            MakeExhaustFire();
        }

        private void MakeExhaustFire()
        {
            if (Velocity.LengthSquared() > 0.1f)
            {
                //Setting up Variables
                Orientation = Velocity.ToAngle();
                Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, Orientation);

                double t = GameRoot.GameTime.TotalGameTime.TotalSeconds;
                // The primary velocity of the particles is 3 px per frame in the direction opposite to which the ship is travelling
                Vector2 baseVel = Velocity.ScaleTo(-4);
                //Calc the sideways vel for the two side streams. the dir is perpendicuar to the ship's velocity and the magnitude varies
                Vector2 prepVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
                Color sideColor = new Color(200, 38, 9); // Deep Red
                Color midColor = new Color(255, 187, 30); // orange - yellow
                Vector2 pos = Position + Vector2.Transform(new Vector2(-25, 0), rot); // Position of the ships exaust pipe
                const float alpha = 0.75f;

                //-------------Mid Particle Stream!!---------------
                Vector2 velMid = baseVel + rand.NextVector2(0, 1);
                GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 15f, new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));
                GameRoot.ParticleManager.CreateParticle(Art.Glow, pos, midColor * alpha, 15f, new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));

                //-------------Sid Particle Stream!----------------
                Vector2 vel1 = baseVel + prepVel + rand.NextVector2(0, 0.3f);
                Vector2 vel2 = baseVel - prepVel + rand.NextVector2(0, 0.3f);

                GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 15f, new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                GameRoot.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 15f, new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));



            }
        }
    }
}
