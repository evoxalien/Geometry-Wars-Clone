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

        public static void Update()
        {
            
            if (!PlayerShip.Instance.IsDead && EntityManager.Count < 400)
            {
                //if (rand.Next((int)inverseSpawnChance) == 0)
                    //EntityManager.Add(Enemy.CreateSquareDance(GetSpawnPosition()));
                if (rand.Next((int)inverseSpawnChance) == 0)
                {
                    for (int j = 0; j < (int)PlayerStatus.Multiplier / 100 + 1; j++)
                        EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                    
                    EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                }
                if (rand.Next((int)inverseSpawnChance) == 0)
                    {
                        if (PlayerStatus.Multiplier != 500)
                            EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
                        EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
                    }
            }

            // slowly increase the spawn rate as time progresses
            if (inverseSpawnChance > 10)
                inverseSpawnChance -= 0.0005f;

        }

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
    }
}
