using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GeometryWars
{
    public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

    public struct ParticleState
    {
        public Vector2 Velocity;
        public ParticleType Type;
        public float LengthMultiplier;

        public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle)
        {
            var vel = particle.State.Velocity;
            float speed = vel.Length();
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;
            particle.Position += vel;
            particle.Orientation = vel.ToAngle();

            if (particle.State.Type != ParticleType.IgnoreGravity)
            {
                foreach (var blackHole in EntityManager.BlackHoles)
                {
                    var dPos = blackHole.Position - particle.Position;
                    float distance = dPos.Length();
                    var n = dPos / distance;
                    vel += 10000 * n / (distance * distance + 10000);

                    // add tagentia acceleration for neary particles
                    if (distance < 400)
                        vel += 45 * new Vector2(n.Y, -n.X) / (distance + 100);

                }
            }
            
            

            particle.Tint.A = (byte)(255 * alpha);

            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);
            //denormalized floats cause significant performance issues
            if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f)
                vel = Vector2.Zero;

            vel *= 0.97f;
            particle.State.Velocity = vel;
        }
    }
    
}
