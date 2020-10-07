using System;
using System.Linq;
using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
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
                return _inventory ??= UserInterfaceManager.Get<InventoryWindow>();
            }
        }

        private MapWindow _mapWindow;
        public MapWindow MapWindow
        {
            get
            {
                return _mapWindow ??= UserInterfaceManager.Get<MapWindow>();
            }
        }

        private FovWindow _fovObjectsWindow;

        public Player() : base(Constants.Player.Foreground, Color.Transparent, Constants.Player.Character, 1, 1)
        {
            FieldOfViewRadius = 0;
            Components.Add(new EntityViewSyncComponent());
        }

        public override void OnMove(object sender, EntityMovedEventArgs args)
        {
            // Handles flashlight lights
            GridManager.Grid.LightEngine.HandleFlashlight(this);

            // OnMove will redraw fov, and flashlight needs to be handled before that
            base.OnMove(sender, args);

            // Update visible objects in fov window
            UpdateFovWindow();
        }

        private void UpdateFovWindow()
        {
            var fovObjectsWindow = _fovObjectsWindow ??= UserInterfaceManager.Get<FovWindow>();

            var prevFov = FieldOfViewRadius;
            // Get all unique characters visible in the fov radius and display them in the fovObjectsWindow
            // The fov is bigger for certain cells with brightness
            FieldOfViewRadius = Constants.Player.FieldOfViewRadius + 3;
            EntityManager.RecalculatFieldOfView(this, false);

            // Get first the further bright cells
            var farBrightCells = GridManager.Grid.GetCellsInFov(this)
                .Where(a => a.CellProperties.IsExplored)
                .Where(a => a.LightProperties.Brightness > 0f)
                .Select(a => (char)a.Glyph)
                .ToList();

            FieldOfViewRadius = Constants.Player.FieldOfViewRadius;
            EntityManager.RecalculatFieldOfView(this, false);

            // Get normal visible cells
            var normalCells = GridManager.Grid.GetCellsInFov(this)
                .Where(a => a.CellProperties.IsExplored)
                .Select(a => (char)a.Glyph)
                .ToList();
            normalCells.AddRange(farBrightCells);

            // Actual cells we see
            var cells = normalCells.Distinct().ToList();

            // Reset player fov
            if (FieldOfViewRadius != prevFov)
            {
                FieldOfViewRadius = prevFov;
                EntityManager.RecalculatFieldOfView(this, false);
            }

            // Add to the fov window
            foreach (var cell in cells)
            {
                fovObjectsWindow.Add(cell, false);
            }
            fovObjectsWindow.RemoveAllExcept(cells);
            fovObjectsWindow.UpdateText();
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

            UpdateFovWindow();
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
