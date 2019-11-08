using Emberpoint.Core.GameObjects.Blueprints;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class MapWindow : ScrollingConsole, IUserInterface
    {
        public Console Console
        {
            get { return this; }
        }

        public MapWindow(int width, int height) : base(width, height, Global.FontDefault.Master.GetFont(Font.FontSizes.Four), new Rectangle(0, 0, 18, 7))
        {
            Position = new Point(1, 1);

            Global.CurrentScreen.Children.Add(this);
        }

        public void Initialize()
        {
            // Initialize grid and render it on the map
            GridManager.InitializeBlueprint<GroundFloorBlueprint>();
            GridManager.Grid.RenderObject(this);

            // After map is created, we calibrate the light engine
            GridManager.Grid.CalibrateLightEngine();
        }

        public void CenterOnEntity(IEntity entity)
        {
            this.CenterViewPortOnPoint(entity.Position);
        }

        public void Update()
        {
            // Tell's sadconsole to redraw this console
            IsDirty = true;
        }
    }
}
