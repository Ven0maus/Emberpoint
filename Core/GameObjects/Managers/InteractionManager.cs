using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.GameObjects.Managers
{
    public class InteractionManager
    {
        private readonly InteractionWindow interactionWindow;
        //For possible use in the future, for example when interacting with an object, the player's hp is reduced.
        private readonly Player player;
        private readonly FovWindow _fovObjectsWindow;
        public InteractionManager(InteractionWindow interactionWindow, Player player)
        {
            this.interactionWindow = interactionWindow;
            _fovObjectsWindow = UserInterfaceManager.Get<FovWindow>();
            this.player = player;
        }
        public void HandleInteraction(Point position) 
        {
            var cell = GridManager.Grid.GetCell(position);
            string cellName = cell.CellProperties.Name;
            switch (cellName)
            {
                case Constants.DoorClosed:
                case Constants.DoorOpen:
                    HandleDoorInteraction(cell);
                    break;
                default:
                    HandleDefaultInteraction(cell);
                    break;
            }
        }
        private void HandleDoorInteraction(EmberCell cell) 
        {
            if (cell.CellProperties.Walkable)
            {
                interactionWindow.PrintMessage(Constants.CloseDoor);
                cell.CellProperties.Walkable = false;
                cell.Glyph = '+';
                cell.CellProperties.BlocksFov = true;
            }
            else
            {
                interactionWindow.PrintMessage(Constants.OpenDoor);
                cell.CellProperties.Walkable = true;
                cell.Glyph = '=';
                cell.CellProperties.BlocksFov = false;
            }
            GridManager.Grid.SetCell(cell);

            // Incase some effects need to be updated/shown after an interaction
            UpdateMapAfterInteraction();
        }

        private void UpdateMapAfterInteraction()
        {
            // Changing if cell blocks fov, needs to handle a few extra things regarding player:
            GridManager.Grid.LightEngine.HandleFlashlight(player);
            player.FieldOfView.Calculate(player.Position, player.FieldOfViewRadius);

            // Draw unexplored tiles when flashlight is on
            var flashLight = player.Inventory.GetItemOfType<Flashlight>();
            bool discoverUnexploredTiles = flashLight != null && flashLight.LightOn;
            GridManager.Grid.DrawFieldOfView(player, discoverUnexploredTiles);

            player.MapWindow.Update();
            _fovObjectsWindow.Update(player);
        }

        private void HandleDefaultInteraction(EmberCell cell)
        {
            interactionWindow.PrintMessage("Interacting with " + cell.CellProperties.Name);
        }
    }
}