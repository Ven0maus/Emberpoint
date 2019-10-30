using Emberpoint.Core.GameObjects.Blueprints;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class MapWindow : Console, IUserInterface
    {
        public MapWindow(int width, int height) : base(width, height)
        {
            Position = new Point(25, 3);
            Global.CurrentScreen.Children.Add(this);
        }

        public void Initialize()
        {
            // Initialize grid and render it on the map
            GridManager.InitializeBlueprint<GroundFloorBlueprint>();
            GridManager.Grid.RenderObject(this);
        }

        public void Update()
        {
            // Tell's sadconsole to redraw this console
            IsDirty = true;
        }
    }
}
