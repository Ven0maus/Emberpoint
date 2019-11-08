using System;
using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Entities.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;

namespace Emberpoint.Core.GameObjects.Entities
{
    public class Player : EmberEntity
    {
        private InventoryWindow _inventory;
        public InventoryWindow Inventory
        {
            get
            {
                return _inventory ?? (_inventory = UserInterfaceManager.Get<InventoryWindow>());
            }
        }

        private MapWindow _mapWindow;
        public MapWindow MapWindow
        {
            get
            {
                return _mapWindow ?? (_mapWindow = UserInterfaceManager.Get<MapWindow>());
            }
        }

        public Player() : base(Constants.Player.Foreground, Color.Black, Constants.Player.Character, 1, 1)
        {
            FieldOfViewRadius = 0; // TODO: needs to be 0 but map should stay dark
   
            Components.Add(new EntityViewSyncComponent());
        }

        public override void Update(TimeSpan timeElapsed)
        {
            // Check movement keys for the player
            CheckForMovementKeys();

            // Check any interaction keys for the player
            CheckForInteractionKeys();

            // Call base to update correctly
            base.Update(timeElapsed);
        }

        public void Initialize()
        {
            // Draw player on the map
            RenderObject(MapWindow);

            // Center viewport on player
            MapWindow.CenterOnEntity(this);
        }

        public void CheckForInteractionKeys()
        {
            if (Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Flashlight)))
            {
                var flashLight = Game.Player.Inventory.GetItemOfType<Flashlight>();
                if (flashLight != null)
                {
                    flashLight.Switch();
                }
            }
        }

        public void CheckForMovementKeys()
        {
            if (Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Movement_Up))) 
            {
                MoveTowards(Position.Translate(0, -1)); // Move up
            }
            else if (Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Movement_Down)))
            {
                MoveTowards(Position.Translate(0, 1)); // Move down
            }
            else if (Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Movement_Left)))
            {
                MoveTowards(Position.Translate(-1, 0)); // Move left
            }
            else if (Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Movement_Right)))
            {
                MoveTowards(Position.Translate(1, 0)); // Move right
            }
        }
    }
}
