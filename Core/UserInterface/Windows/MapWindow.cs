using Emberpoint.Core.GameObjects.Blueprints;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class MapWindow : Console, IUserInterface
    {
        public readonly Renderer EntityRenderer;

        public Console Content
        {
            get { return this; }
        }

        public MapWindow(int width, int height) : base(width, height)
        {
            View = new Rectangle(0, 0, 23, 10);
            Position = new Point(1, 1);
            Font = Constants.Fonts.Default;
            FontSize = Font.GetFontSize(Constants.Map.Size);
            GameHost.Instance.Screen.Children.Add(this);

            EntityRenderer = new Renderer
            {
                DoEntityUpdate = true
            };
            SadComponents.Add(EntityRenderer);
        }

        public void AfterCreate()
        {
            // Initialize grid and render it on the map
            GridManager.InitializeBlueprint<GroundFloorCellsBlueprint>(true);
            GridManager.Grid.RenderObject(this);
        }

        public void CenterOnEntity(IEntity entity)
        {
            View = View.WithCenter(entity.Position);
        }

        /// <summary>
        /// Call this method when you change the cell colors on the cell objects.
        /// </summary>
        public void Refresh()
        {
            IsDirty = true;
        }
    }
}
