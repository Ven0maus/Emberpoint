using System.Collections.Generic;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole.Components;
using SadConsole.Input;

namespace Emberpoint.Core.GameObjects.Entities
{
    public class Player : EmberEntity
    {
        private InventoryWindow _inventory;
        public InventoryWindow Inventory => _inventory ??= UserInterfaceManager.Get<InventoryWindow>();

        private MapWindow _mapWindow;
        public MapWindow MapWindow => _mapWindow ??= UserInterfaceManager.Get<MapWindow>();

        private readonly InteractionManager interactionManager;
        private readonly FovWindow _fovObjectsWindow;
        private readonly InteractionWindow _interaction;
        private bool InteractionStatus;

        public Player() : base(Constants.Player.Foreground, Color.Transparent, Constants.Player.Character)
        {
            FieldOfViewRadius = 0;
            InteractionStatus = false;
            _interaction = UserInterfaceManager.Get<InteractionWindow>();
            interactionManager = new InteractionManager(_interaction, this);
            _fovObjectsWindow = UserInterfaceManager.Get<FovWindow>();
            Components.Add(new EntityViewSyncComponent());
        }

        public override void OnMove(object sender, EntityMovedEventArgs args)
        {
            // Handles flashlight lights
            GridManager.Grid.LightEngine.HandleFlashlight(this);

            // OnMove will redraw fov, and flashlight needs to be handled before that
            base.OnMove(sender, args);

            // Update visible objects in fov window
            _fovObjectsWindow.Update(this);
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            bool keyHandled = false;

            // Simplified way to check if any key we care about is pressed and set movement direction.
            foreach (var key in _playerMovements.Keys)
            {
                var binding = KeybindingsManager.GetKeybinding(key);
                if (info.IsKeyPressed(binding))
                {
                    var moveDirection = _playerMovements[key];
                    _interaction.PrintMessage(Constants.EmptyMessage);
                    MoveTowards(moveDirection);
                    InteractionStatus = CheckInteraction( _interaction);
                    keyHandled = true;
                    break;
                }
            }

            // Handle cell interactions
            if (info.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Interact)) 
                && InteractionStatus && GetInteractedCell(out Point position))
            {
                interactionManager.HandleInteraction(position);
                keyHandled = true;
            }

            //If this will grow in the future, we may want to add a Dictionary<Keybindings, EmberItem>
            // to efficiently retrieve and activate items.
            if (info.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Flashlight)))
            {
                var flashLight = Game.Player.Inventory.GetItemOfType<Flashlight>();
                flashLight?.Switch();
                keyHandled = true;
            }

            // Handle dialog window
            if (info.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                var devConsole = UserInterfaceManager.Get<DeveloperWindow>();
                if (!devConsole.IsVisible)
                {
                    var dialogWindow = UserInterfaceManager.Get<DialogWindow>();
                    if (dialogWindow.IsVisible)
                    {
                        dialogWindow.ShowNext();
                        keyHandled = true;
                    }
                }
            }

            // Handle dev console showing
            if (info.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.DeveloperConsole)))
            {
                var devConsole = UserInterfaceManager.Get<DeveloperWindow>();
                if (devConsole.IsVisible)
                {
                    devConsole.Hide();
                }
                else
                {
                    devConsole.Show();
                }
                keyHandled = true;
            }

            if (keyHandled)
                return true;
            else
                return base.ProcessKeyboard(info);
        }

        public void Initialize(bool addRenderObject = true)
        {
            if (addRenderObject)
            {
                // Draw player on the map
                RenderObject(MapWindow);
            }

            // Make sure we check for input
            IsFocused = true;

            // Center viewport on player
            MapWindow.CenterOnEntity(this);

            // Show the nearby objects in the fov window
            _fovObjectsWindow.Update(this);

            // Render map
            MapWindow.IsDirty = true;
        }

        private readonly Dictionary<Keybindings, Direction> _playerMovements =
        new Dictionary<Keybindings, Direction>
        {
            {Keybindings.Movement_Up, Direction.UP},
            {Keybindings.Movement_Down, Direction.DOWN},
            {Keybindings.Movement_Left, Direction.LEFT},
            {Keybindings.Movement_Right, Direction.RIGHT}
        };
    }
}
