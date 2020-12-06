using System;
using System.Collections.Generic;
using System.Text;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.GameObjects.Managers
{
    public class InteractionManager
    {
        private InteractionWindow interactionWindow;
        //For possible use in the future, for example when interacting with an object, the player's hp is reduced.
        private Player player;
        public InteractionManager(InteractionWindow interactionWindow, Player player)
        {
            this.interactionWindow = interactionWindow;
            this.player = player;
        }
        public void HandleInteraction(Point position) 
        {
            var cell = GridManager.Grid.GetCell(position);
            string cellName = cell.CellProperties.Name;
            switch (cellName)
            {
                case Constants.Door:
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
            }
            else
            {
                interactionWindow.PrintMessage(Constants.OpenDoor);
                cell.CellProperties.Walkable = true;
            }
            GridManager.Grid.SetCell(cell);
        }

        private void HandleDefaultInteraction(EmberCell cell)
        {
            interactionWindow.PrintMessage("Interacting with " + cell.CellProperties.Name);
        }
    }
}