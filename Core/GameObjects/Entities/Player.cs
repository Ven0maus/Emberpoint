using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System.Collections.Generic;

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
        private readonly WorldmapWindow _worldMap;
        private readonly DeveloperWindow _devWindow;
        private bool InteractionStatus;

        public Player() : base(Constants.Player.Foreground, Color.Transparent, Constants.Player.Character)
        {
            FieldOfViewRadius = 0;
            InteractionStatus = false;
            _interaction = UserInterfaceManager.Get<InteractionWindow>();
            interactionManager = new InteractionManager(_interaction, this);
            _fovObjectsWindow = UserInterfaceManager.Get<FovWindow>();
            _worldMap = UserInterfaceManager.Get<WorldmapWindow>();
            _devWindow = UserInterfaceManager.Get<DeveloperWindow>();
        }

        public override void OnMove(object sender, ValueChangedEventArgs<Point> args)
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
                    _interaction.ClearMessage();
                    MoveTowards(moveDirection);
                    InteractionStatus = CheckInteraction( _interaction);
                    keyHandled = true;
                    break;
                }
            }

            if (info.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Map)))
            {
                if (!_worldMap.IsVisible)
                {
                    _worldMap.Show();
                }
                keyHandled = true;
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
            if (info.IsKeyPressed(Keys.Enter))
            {
                if (!_devWindow.IsVisible)
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
                if (_devWindow.IsVisible)
                {
                    _devWindow.Hide();
                }
                else
                {
                    _devWindow.Show();
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
                RenderObject(MapWindow.EntityRenderer);
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
            {Keybindings.Movement_Up, Direction.Up},
            {Keybindings.Movement_Down, Direction.Down},
            {Keybindings.Movement_Left, Direction.Left},
            {Keybindings.Movement_Right, Direction.Right}
        };
    }
}
