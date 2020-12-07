using Emberpoint.Core.GameObjects.Entities;
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
                    HandleDoorInteraction(cell);
                    break;
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
            }
            else
            {
                interactionWindow.PrintMessage(Constants.OpenDoor);
                cell.CellProperties.Walkable = true;
                cell.Glyph = '=';
            }
            GridManager.Grid.SetCell(cell);
            player.MapWindow.Update();
            _fovObjectsWindow.Update(player);
        }

        private void HandleDefaultInteraction(EmberCell cell)
        {
            interactionWindow.PrintMessage("Interacting with " + cell.CellProperties.Name);
        }
    }
}