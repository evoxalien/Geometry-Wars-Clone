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
    static class EntityManager
    {
        private static Random rand = new Random();
        static List<Entity> entities = new List<Entity>();
        static List<Enemy> enemies = new List<Enemy>();
        static List<Bullet> bullets = new List<Bullet>();
        static List<BlackHole> blackHoles = new List<BlackHole>();

        public static IEnumerable<BlackHole> BlackHoles { get { return blackHoles; } }

        static bool isUpdating;
        static List<Entity> addedEntities = new List<Entity>();

        public static int Count { get { return entities.Count; } }
        public static int BlackHoleCount { get { return blackHoles.Count; } }

        private static void AddEntity(Entity entity)
        {
            entities.Add(entity);
            if (entity is Bullet)
                bullets.Add(entity as Bullet);
            else if (entity is Enemy)
                enemies.Add(entity as Enemy);
            else if (entity is BlackHole)
                blackHoles.Add(entity as BlackHole);
        }

        public static void Add(Entity entity)
        {
            if (!isUpdating)
                AddEntity(entity);
            else
                addedEntities.Add(entity);
        }

        public static void Update()
        {
            isUpdating = true;
            
            HandleCollisions();

            foreach (var entity in entities)
                entity.Update();

            isUpdating = false;

            foreach (var entity in addedEntities)
                AddEntity(entity);

            addedEntities.Clear();

            // remove any expired entities
            entities = entities.Where(x => !x.IsExpired).ToList();
            bullets = bullets.Where(x => !x.IsExpired).ToList();
            enemies = enemies.Where(x => !x.IsExpired).ToList();
            blackHoles = blackHoles.Where(x => !x.IsExpired).ToList();

        }

        // Used with Blackholes!

        public static System.Collections.IEnumerable GetNearbyEntities(Vector2 position, float radius)
        {
            return entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius);
        }

        //Collision!

        private static bool IsColliding(Entity a, Entity b)
        {
            float radius = a.Radius + b.Radius;
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }

        static void HandleCollisions()
        {
            //handle collisions between enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = i + 1; j < enemies.Count; j++)
                {
                    if (IsColliding(enemies[i], enemies[j]))
                    {
                        enemies[i].HandleCollision(enemies[j]);
                        enemies[j].HandleCollision(enemies[i]);
                    }
                }

            //handle HandleCollisions between bullets and enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = 0; j < bullets.Count; j++)
                {
                    if(IsColliding(enemies[i], bullets[j]))
                    {
                        enemies[i].WasShot();
                        bullets[j].IsExpired = true;
                        for (int p = 0; p < 30; p++)
                            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, bullets[j].Position, Color.LightBlue, 50, new Vector2(0.5f, 0.5f), new ParticleState() { Velocity = rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
                    }
                }

            //HANDLE COLLISIONS BETWEEN PLAYERS AND ENEMIES

            if (Input.GodMode == false)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
                    {
                        PlayerShip.Instance.Kill();
                        enemies.ForEach(x => x.WasShot());
                        blackHoles.ForEach(x => x.Kill());
                        break;

                    }
                }
            }

            // handle collisions with black holes
            for (int i = 0; i < blackHoles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                    if (enemies[j].IsActive && IsColliding(blackHoles[i], enemies[j]))
                    {
                        enemies[j].Effect();
                        enemies[j].IsExpired = true;

                    }
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (IsColliding(blackHoles[i], bullets[j]))
                    {
                        bullets[j].IsExpired = true;
                        blackHoles[i].WasShot();
                        for (int p = 0; p < 30; p++)
                            GameRoot.ParticleManager.CreateParticle(Art.LineParticle, bullets[j].Position, Color.LightBlue, 50, new Vector2(0.5f, 0.5f), new ParticleState() { Velocity = rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
                    
                    }
                }
                if (Input.GodMode == false)
                {
                    if (IsColliding(PlayerShip.Instance, blackHoles[i]))
                    {
                        PlayerShip.Instance.Kill();
                        enemies.ForEach(x => x.IsExpired = true);
                        blackHoles.ForEach(x => x.Kill());
                        break;
                    }
                }
            }
        }


        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
                entity.Draw(spriteBatch);
        }
    }
}
