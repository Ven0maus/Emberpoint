﻿using Emberpoint.Core.Extensions;
using Emberpoint.Core.Objects.Abstracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;

namespace Emberpoint.Core.Objects
{
    public class Player : EmberEntity
    {
        public Player() : base(Constants.Player.Foreground, Color.Transparent, Constants.Player.Character, 1, 1)
        {
            FieldOfViewRadius = Constants.Player.FieldOfViewRadius;
        }

        public void CheckForMovement()
        {
            if (Global.KeyboardState.IsKeyPressed(Keys.Z) || Global.KeyboardState.IsKeyPressed(Keys.D8)) 
            {
                MoveTowards(Position.Translate(0, -1)); // Move up
            }
            else if (Global.KeyboardState.IsKeyPressed(Keys.S) || Global.KeyboardState.IsKeyPressed(Keys.D2))
            {
                MoveTowards(Position.Translate(0, 1)); // Move down
            }
            else if (Global.KeyboardState.IsKeyPressed(Keys.Q) || Global.KeyboardState.IsKeyPressed(Keys.D4))
            {
                MoveTowards(Position.Translate(-1, 0)); // Move left
            }
            else if (Global.KeyboardState.IsKeyPressed(Keys.D) || Global.KeyboardState.IsKeyPressed(Keys.D6))
            {
                MoveTowards(Position.Translate(1, 0)); // Move right
            }
        }
    }
}
