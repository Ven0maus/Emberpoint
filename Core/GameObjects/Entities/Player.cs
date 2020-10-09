using System;
using System.Collections.Generic;
using System.Linq;
using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Items;
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
        public InventoryWindow Inventory => _inventory ??= UserInterfaceManager.Get<InventoryWindow>();

        private MapWindow _mapWindow;
        public MapWindow MapWindow => _mapWindow ??= UserInterfaceManager.Get<MapWindow>();

        private readonly FovWindow _fovObjectsWindow;

        public Player() : base(Constants.Player.Foreground, Color.Transparent, Constants.Player.Character, 1, 1)
        {
            FieldOfViewRadius = 0;
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

        public override void Update(TimeSpan timeElapsed)
        {
            // Call base to update correctly
            base.Update(timeElapsed);

            // Check movement keys for the player
            CheckForMovementKeys();

            // Check any interaction keys for the player
            CheckForInteractionKeys();
        }

        public void Initialize()
        {
            // Draw player on the map
            RenderObject(MapWindow);

            // Center viewport on player
            MapWindow.CenterOnEntity(this);

            _fovObjectsWindow.Update(this);
        }

        public void CheckForInteractionKeys()
        {
            //If this will grow in the future, we may want to add a Dictionary<Keybindings, EmberItem>
            // to efficiently retrieve and activate items.
            if (Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Flashlight)))
            {
                var flashLight = Game.Player.Inventory.GetItemOfType<Flashlight>();
                flashLight?.Switch();
            }
        }

        public void CheckForMovementKeys()
        {
            foreach (var movementKey in _playerMovements.Keys
                .Where(key => Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(key))))
            {
                var (x, y) = _playerMovements[movementKey];
                MoveTowards(Position.Translate(x, y));
            }
        }

        private readonly Dictionary<Keybindings, (int x, int y)> _playerMovements = 
        new Dictionary<Keybindings, (int x, int y)>
        {
            {Keybindings.Movement_Up, (0, -1)},
            {Keybindings.Movement_Down, (0, 1)},
            {Keybindings.Movement_Left, (-1, 0)},
            {Keybindings.Movement_Right, (1, 0)}
        };
    }
}
