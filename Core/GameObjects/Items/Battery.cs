using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.GameObjects.Items
{
    public class Battery : EmberItem
    {
        public int Power { get; private set; }

        public override string DisplayName { get { return string.Format(" {0} : {1} : Power [{2}] \r\n", Name, Amount, Power); } }

        public Battery() : base('B', Color.YellowGreen, 1, 1)
        {
            Power = Constants.Items.BatteryMaxPower;
        }

        public bool Drain()
        {
            if (Power > 0)
                Power--;

            Game.Player.Inventory.UpdateInventoryText();

            if (Power == 0)
            {
                var dialogWindow = UserInterfaceManager.Get<DialogWindow>();
                // Check if we have more than one battery
                if (Amount > 1)
                {
                    Game.Player.Inventory.RemoveInventoryItem<Battery>(1);
                    Power = Constants.Items.BatteryMaxPower;

                    dialogWindow.AddDialog("Battery depleted.", new[] { "A battery has been depleted!", "Press enter to hide this message." });
                    dialogWindow.ShowNext();
                    return true;
                }

                dialogWindow.AddDialog("Battery depleted.", new[] { "You ran out of batteries!", "Press enter to hide this message." });
                dialogWindow.ShowNext();
                return false;
            }
            return true;
        }

        public override void PickUp()
        {
            var inventory = Game.Player == null ? UserInterfaceManager.Get<InventoryWindow>() : Game.Player.Inventory;
            var flashlight = inventory.GetItemOfType<Flashlight>();
            if (flashlight != null && flashlight.Battery == null)
            {
                flashlight.Battery = this;
            }
            base.PickUp();
        }
    }
}
