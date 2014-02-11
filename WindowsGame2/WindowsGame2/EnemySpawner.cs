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
                }
                if (cooldownRemaining > 0)
                    cooldownRemaining--;
            }
        }
        #endregion

        #region Update
        public static void Update()
        {

            if (!PlayerShip.Instance.IsDead && EntityManager.Count < MaxEnemyCount)
            {
                //if (rand.Next((int)inverseSpawnChance) == 0)
                    //EntityManager.Add(Enemy.CreateSquareDance(GetSpawnPosition()));
                if (rand.Next((int)inverseSpawnChance) == 0)
                {
                    for (int j = 0; j < (int)(PlayerStatus.Multiplier / 100 + 1) % 3 || j < (PlayerShip.WeaponLevel * 2); j++)
                        EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                    
                    //EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                }
                if (rand.Next((int)inverseSpawnChance) == 0)
                {
                    if (PlayerStatus.Multiplier != 500)
                    {
                        for (int j = 0; j < (int)(PlayerStatus.Multiplier / 50) % 4 || j < (PlayerShip.WeaponLevel * 3 + 1) % 3; j++)
                            EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
                        //EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
                    }
                }
                if (EntityManager.BlackHoleCount < 2 && rand.Next((int)inverseBlackHoleChance) == 0)
                    EntityManager.Add(new BlackHole(GetSpawnPosition()));
            }

            // slowly increase the spawn rate as time progresses
            if (inverseSpawnChance > 10)
                inverseSpawnChance -= 0.0005f;

        }
        #endregion
    }
}
