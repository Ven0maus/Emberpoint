using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using Emberpoint.Core.UserInterface.Windows;
using SadRogue.Primitives;

namespace Emberpoint.Core.GameObjects.Items
{
    public class Battery : EmberItem
    {
        public int Power { get; private set; }

        public override string DisplayName { get { return string.Format(" {0} : {1} : " + Strings.BatteryPower + " [{2}] \r\n", Name, Amount, Power); } }

        public Battery() : base('B', Color.YellowGreen, name: () => Strings.Battery)
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
                    return true;
                }

                // TODO: Add localization, move to file?
                dialogWindow.AddDialog("Batteries depleted.", new[] { "You ran out of batteries!", "Press enter to hide this message." });
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
