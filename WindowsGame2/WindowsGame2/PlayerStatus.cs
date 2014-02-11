﻿using System;
using System.IO;
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
    static class PlayerStatus
    {
        //amount of time it takes, in seconds, for a multiplier to expire
        private const float multiplierExpiryTime = 1.8f;
        private const int maxMultiplier = 500;

        public static int Lives { get; private set; }
        public static int Score { get; private set; }
        public static int Multiplier { get; private set; }
        public static int HighScore;

        private static float multiplierTimeLeft; // time until the current multiplier expries
        private static int scoreForExtraLife; // score required to gain an extra life

        public static bool isGameOver { get { return Lives == 0; } }

        private static int LoadHighScore()
        {
            // return the saved high score if possible and return 0 otherwise
            return GameRoot.GetHighscore();
        }

        private static void SaveHighScore(int score)
        {
            GameRoot.WriteFile(GameRoot.highScoreFilename, score.ToString());
            //File.WriteAllText(highScoreFilename, score.ToString());
        }

        //Static constructor
        static PlayerStatus()
        {
            HighScore = LoadHighScore();
            Reset();
        }
        
        public static void Reset()
        {
            if (Score > HighScore)
                SaveHighScore(HighScore = Score);
            Score = 0;
            Multiplier = 1;
            scoreForExtraLife = 2000;
            multiplierTimeLeft = 0;
            Lives = 4;
        }

        public static void ResetMultiplier()
        {
            Multiplier = 1;
        }

        public static void AddPoints(int basePoints)
        {
            if (PlayerShip.Instance.IsDead)
                return;
            Score += basePoints * Multiplier;
            while (Score >= scoreForExtraLife)
            {
                scoreForExtraLife *= 2;
                Lives++;
            }
        }

        public static void RemoveLife()
        {
            Lives--;
        }

        #region Multiplier
        public static void IncreaseMultiplier()
        {
            if (PlayerShip.Instance.IsDead)
                return;

            multiplierTimeLeft = multiplierExpiryTime;
            if (Multiplier < maxMultiplier)
                Multiplier++;

            if (PlayerShip.WeaponLevel < Multiplier / 100 + 1)
                PlayerShip.WeaponLevel = Multiplier / 100 ;
            
        }
        #endregion

        #region Update
        public static void Update()
        {

            if (Multiplier > 1)
            {//update teh multiplier timer
                if ((multiplierTimeLeft -= (float)GameRoot.GameTime.ElapsedGameTime.TotalSeconds) <= 0)
                {
                    multiplierTimeLeft = multiplierExpiryTime;
                    ResetMultiplier();
                }
            }
        }
        #endregion

    }
}
