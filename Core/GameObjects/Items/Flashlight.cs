using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using System.Timers;

namespace Emberpoint.Core.GameObjects.Items
{
    public class Flashlight : EmberItem
    {
        public bool LightOn { get; private set; } = false;
        public Battery Battery { get; set; }

        private readonly Timer _drainTimer;

        public Flashlight() : base('F', Color.LightSkyBlue, 1, 1)
        {
            _drainTimer = new Timer(1000);
            _drainTimer.Elapsed += DrainTimer_Elapsed;
        }

        private void DrainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!UserInterfaceManager.IsInitialized)
            {
                _drainTimer.Stop();
                return;
            }

            if (UserInterfaceManager.IsPaused) return;

            if (!Battery.Drain())
            {
                // Remove battery
                Game.Player.Inventory.RemoveInventoryItem(Battery);

                // Switch light off
                Switch();

                // Set battery null
                Battery = null;
            }
        }

        public void Switch()
        {
            if (Battery == null) return;

            LightOn = !LightOn;

            // Handles flashlight lights
            GridManager.Grid.LightEngine.HandleFlashlight(Game.Player);

            if (!LightOn)
            {
                Game.Player.FieldOfViewRadius = 0;
                EntityManager.RecalculatFieldOfView(Game.Player);

                // Stop drain timer
                _drainTimer.Stop();
            }
            else
            {
                Game.Player.FieldOfViewRadius = Constants.Items.FlashlightRadius;
                EntityManager.RecalculatFieldOfView(Game.Player, false);

                // Discover new tiles when turning on the flashlight
                var flashLight = Game.Player.Inventory.GetItemOfType<Flashlight>();
                bool discoverUnexploredTiles = flashLight != null && flashLight.LightOn;
                GridManager.Grid.DrawFieldOfView(Game.Player, discoverUnexploredTiles);

                // Start drain timer
                _drainTimer.Start();
            }
        }
    }
}
