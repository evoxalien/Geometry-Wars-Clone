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
    static class EnemySpawner
    {
        static Random rand = new Random();
        static float inverseSpawnChance = 45;
        static float inverseBlackHoleChance = 600;
        private static int MaxEnemyCount = 500;
        private static bool RandomSpawns = true;
        public static int SpawnCounter = 0;
        private static int NumTillCornerSpawn = 200;
        //static Boss FlappyKing = new Boss(Art.FlappyKing, new Vector2(GameRoot.Viewport.Width - 200, GameRoot.Viewport.Height));
        public static bool FlappyKingSpawned = false;
        public static bool SpawnFlappyKing = false;

        #region AdditionalFunctions
        private static Vector2 GetSpawnPosition()
        {
            Vector2 pos;
            do
            {
                pos = new Vector2(rand.Next((int)GameRoot.ScreenSize.X), rand.Next((int)GameRoot.ScreenSize.Y));
            } while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

            return pos;
        }

        public static void Reset()
        {
            inverseSpawnChance = 60;
        }
        #endregion

        #region BlackHoleSwarm
        public static void Swarm(Vector2 pos)
        {
            int cooldownFrames = 2;
            int cooldownRemaining = 0;

            for(int Swarm = 0; Swarm < 35 && EntityManager.Count + Swarm <= MaxEnemyCount; )
            {
                if(cooldownRemaining <= 0)
                {
                    cooldownRemaining = cooldownFrames;
                    pos += new Vector2(rand.Next(5) / 2, rand.Next(5) / 2);
                    EntityManager.Add(Enemy.CreateWanderer(pos));
                    Swarm++;
                    SpawnCounter++;
                }
                if (cooldownRemaining > 0)
                    cooldownRemaining--;
            }
        }
        #endregion

        #region UpdateAndSpawning
        public static void Update()
        {

            if (SpawnFlappyKing == true)
            {
                if (FlappyKingSpawned == false)
                {
                    EntityManager.Add(Boss.FlappyKing());
                    FlappyKingSpawned = true;
                }
                if (rand.Next((int)inverseSpawnChance) == 0)
                {
                    for (int j = 0; j < (int)(PlayerStatus.Multiplier / 100 + 1) || j < (PlayerShip.WeaponLevel * 2); j++)
                    {
                        EntityManager.Add(Enemy.CreateFlappyMinion(new Vector2(rand.Next(0,200), rand.Next((int)GameRoot.ScreenSize.Y))));
                        //Sound.Spawn.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
                    }

                    //EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                }
            }
            if (!PlayerShip.Instance.IsDead && EntityManager.Count < MaxEnemyCount && FlappyKingSpawned == false)//Remove this to spawn regular again
            {
                //if (rand.Next((int)inverseSpawnChance) == 0)
                    //EntityManager.Add(Enemy.CreateSquareDance(GetSpawnPosition()));

                if (RandomSpawns)
                {
                    //Random Spawns
                    if (rand.Next((int)inverseSpawnChance) == 0)
                    {
                        //for (int j = 0; j < (int)(PlayerStatus.Combo / 100 + 1) % 3 || j < (PlayerShip.WeaponLevel * 2) || j < PlayerStatus.Multiplier / 2; j++)
                        for (int j = 0; j < (PlayerShip.WeaponLevel * 2) || j < PlayerStatus.Multiplier * 2; j++)
                        {
                            EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                            //Sound.Spawn.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0); 
                            SpawnCounter++;
                        }
                        //EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                    }
                    if (rand.Next((int)inverseSpawnChance) == 0)
                    {
                        if (PlayerStatus.Combo != 500)
                        {
                            //for (int j = 0; j < (int)(PlayerStatus.Combo / 50) % 4 || j < (PlayerShip.WeaponLevel * 3 + 1) % 3 || j < PlayerStatus.Multiplier; j++)
                            for (int j = 0; j < (PlayerShip.WeaponLevel * 3 + 1) % 3 || j < PlayerStatus.Multiplier; j++)
                            {
                                EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
                                //Sound.Spawn.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
                                SpawnCounter++;
                            }
                            //EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));

                        }
                    }

                    if (EntityManager.BlackHoleCount < PlayerStatus.Multiplier % 5 && rand.Next((int)inverseBlackHoleChance) == 0)
                        EntityManager.Add(new BlackHole(GetSpawnPosition()));
                    if (SpawnCounter >= NumTillCornerSpawn)
                        RandomSpawns = false;
                    /*
                   if (rand.Next((int)inverseSpawnChance) == 0)
                   {
                       for (int j = 0; j < (int)(PlayerStatus.Multiplier / 100 + 1) % 3 || j < (PlayerShip.WeaponLevel * 2); j++)
                       {
                           EntityManager.Add(Enemy.CreateFlappyMinion(GetSpawnPosition()));
                           //Sound.Spawn.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
                       }

                       //EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                   }
                     */
                }
                if (!RandomSpawns)
                {
                    if (rand.Next((int)inverseSpawnChance) == 0)
                        for (int i = 0; i < 5; i++)
                        {
                            EntityManager.Add(Enemy.CreateSeeker(new Vector2(20 + rand.Next(0, 10), 20 + rand.Next(0, 10))));
                            SpawnCounter++;
                        }
                    if (rand.Next((int)inverseSpawnChance) == 0)
                        for (int i = 0; i < 5; i++)
                        {
                            EntityManager.Add(Enemy.CreateSeeker(new Vector2(20 + rand.Next(0, 10), GameRoot.Viewport.Height - rand.Next(0, 10))));
                            SpawnCounter++;
                        }
                    if (rand.Next((int)inverseSpawnChance) == 0)
                        for (int i = 0; i < 5; i++)
                        {
                            EntityManager.Add(Enemy.CreateSeeker(new Vector2(GameRoot.Viewport.Width - rand.Next(0, 10), 20 + rand.Next(0, 10))));
                            SpawnCounter++;
                        }
                    if (rand.Next((int)inverseSpawnChance) == 0)
                        for (int i = 0; i < 5; i++)
                        {
                            EntityManager.Add(Enemy.CreateSeeker(new Vector2(GameRoot.Viewport.Width - rand.Next(0, 10),GameRoot.Viewport.Height - rand.Next(0, 10))));
                            SpawnCounter++;
                        }

                    //Corner Spawning
                    if (SpawnCounter >= NumTillCornerSpawn + (10 * PlayerStatus.Multiplier))
                    {
                        RandomSpawns = true;
                        SpawnCounter = 0;
                    }
                }
            }

            // slowly increase the spawn rate as time progresses
            if (inverseSpawnChance > 10)
                inverseSpawnChance -= 0.0025f;

        }
        #endregion
    }
}
