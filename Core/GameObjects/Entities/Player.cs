using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Entities.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;

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

        public Player() : base(Constants.Player.Foreground, Color.Transparent, Constants.Player.Character, 1, 1)
        {
            FieldOfViewRadius = 0; // TODO: needs to be 0 but map should stay dark
        }

        public void Initialize()
        {
            // Draw player on the map
            RenderObject(UserInterfaceManager.Get<MapWindow>());
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
