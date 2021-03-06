﻿using System;
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
    static class Input
    {
        private static KeyboardState keyboardState, lastKeyboardState;
        private static MouseState mouseState, lastMouseState;
        private static GamePadState gamepadState, lastGamepadState;
        public static bool GodMode = false;
        public static bool DevMode = false;
        public static bool DevModeParticles = false;
        public static bool DevModeGrid = false;
        public static bool GamePause = false;

        private static bool isAimingWithMouse = false;

        public static Vector2 MousePosition { get { return new Vector2(mouseState.X, mouseState.Y); } }

        #region WasKeyOrButtonPressed
        public static bool WasKeyPressed(Keys key)
        {
            return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        public static bool WasButtonPressed(Buttons button)
        {
            return lastGamepadState.IsButtonUp(button) && gamepadState.IsButtonDown(button);
        }
        #endregion

        #region Movement
        public static Vector2 GetMovementDirection()
        {
            Vector2 direction = gamepadState.ThumbSticks.Left;
            direction.Y *= -1; //invert the y-axis

            if (keyboardState.IsKeyDown(Keys.A))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D))
                direction.X += 1;
            if (keyboardState.IsKeyDown(Keys.W))
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S))
                direction.Y += 1;

            //Clamp the length of the vector to a maximum of 1.
            if (direction.LengthSquared() > 1)
                direction.Normalize();

            return direction;
        }
        #endregion

        #region Aiming
        public static Vector2 GetAimDirection()
        {
            
            if (isAimingWithMouse)
                if (mouseState.LeftButton == ButtonState.Pressed)
                    return GetMouseAimDirection();
            
            Vector2 direction = gamepadState.ThumbSticks.Right;
            direction.Y *= -1;

            if (keyboardState.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.Right))
                direction.X += 1;
            if (keyboardState.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.Down))
                direction.Y += 1;

            //If there's no aim input, return zero. Otherwise normalize the direction to have a length of 1.

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }

        public static Vector2 GetMouseAimDirection()
        {
            Vector2 direction = MousePosition - PlayerShip.Instance.Position;

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }
        #endregion

        #region Bomb
        public static bool WasBombButtonPressed()
        {
            return WasButtonPressed(Buttons.LeftTrigger) || WasButtonPressed(Buttons.RightTrigger) || WasKeyPressed(Keys.Space);
        }
        public static void GodModeButtonPressed()
        {
            if (WasButtonPressed(Buttons.RightShoulder) || WasKeyPressed(Keys.G))
            {
                GodMode = !GodMode;
                if (GodMode)
                    DevMode = true;
            }
        }
        #endregion

        #region DevModeButtons
        public static void DevModeButtonPressed()
        {
            if (WasButtonPressed(Buttons.X) || WasKeyPressed(Keys.X))
                DevMode = !DevMode;
            if (WasButtonPressed(Buttons.DPadLeft))
                DevModeParticles = !DevModeParticles;
            if (WasButtonPressed(Buttons.DPadRight))
                DevModeGrid = !DevModeGrid;
            if (GodMode)
            {
                if (WasButtonPressed(Buttons.DPadUp))
                    PlayerShip.WeaponLevel++;
                if (WasButtonPressed(Buttons.DPadDown) && PlayerShip.WeaponLevel >= 1)
                    PlayerShip.WeaponLevel--;
            }
        }
        #endregion

        #region GamePause
        public static void GamePausePressed()
        {
            if (WasButtonPressed(Buttons.Start) || WasKeyPressed(Keys.P))
            {
                GamePause = !GamePause;
            }
        }
        #endregion

        #region Update
        public static void Update()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
            lastGamepadState = gamepadState;

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            gamepadState = GamePad.GetState(PlayerIndex.One);

            // If the player pressed one of the arrow keys or is using a gamepad to aim, we want to disable mouse aiming. Otherwise,
            // if the player moves the mouse, enable mouse aiming.

            if (new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(x => keyboardState.IsKeyDown(x)) || gamepadState.ThumbSticks.Right != Vector2.Zero)
                isAimingWithMouse = false;
            else if (MousePosition != new Vector2(lastMouseState.X, lastMouseState.Y))
                isAimingWithMouse = true;

            Input.GodModeButtonPressed();
            Input.DevModeButtonPressed();
            Input.GamePausePressed();
        }
        #endregion
    }
}
