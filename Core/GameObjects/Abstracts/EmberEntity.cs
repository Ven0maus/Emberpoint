using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Entities.Items;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.GameObjects.Abstracts
{
    public abstract class EmberEntity : SadConsole.Entities.Entity, IEntity
    {
        public int ObjectId { get; }
        public int FieldOfViewRadius { get; set; }

        private FOV _fieldOfView;
        public FOV FieldOfView
        {
            get
            {
                return _fieldOfView ?? (_fieldOfView = new FOV(GridManager.Grid.FieldOfView));
            }
        }

        private Console _renderedConsole;

        /// <summary>
        /// Call this when the grid changes to a new grid object. (Like going into the basement etc)
        /// </summary>
        public void ResetFieldOfView()
        {
            _fieldOfView = null;
        }

        public EmberEntity(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(width, height)
        {
            ObjectId = EntityManager.GetUniqueId();

            Font = Global.FontDefault.Master.GetFont(Constants.Map.Size);
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            Moved += OnMove;
        }

        public virtual void OnMove(object sender, EntityMovedEventArgs args)
        {
            if (this is IItem) return;

            // Re-calculate the field of view
            FieldOfView.Calculate(Position, FieldOfViewRadius);

            // Only update visual for player entity
            if (this is Player player)
            {
                // Center viewpoint on player
                player.MapWindow.CenterOnEntity(player);

                // Draw unexplored tiles when flashlight is on
                var flashLight = player.Inventory.GetItemOfType<Flashlight>();
                bool discoverUnexploredTiles = flashLight != null && flashLight.LightOn;
                GridManager.Grid.DrawFieldOfView(this, discoverUnexploredTiles);
            }
        }

        public bool CanMoveTowards(Point position)
        {
            var cell = GridManager.Grid.GetCell(position);
            return GridManager.Grid.InBounds(position) && cell.CellProperties.Walkable && !EntityManager.EntityExistsAt(position) && cell.CellProperties.IsExplored;
        }

        public void RenderObject(Console console)
        {
            _renderedConsole = console;
            console.Children.Add(this);
        }

        public void MoveTowards(Point position, bool checkCanMove = true)
        {
            if (checkCanMove && !CanMoveTowards(position)) return;
            Position = position;
        }

        public void UnRenderObject()
        {
            if (_renderedConsole != null)
            {
                _renderedConsole.Children.Remove(this);
                _renderedConsole = null;
            }
        }
    }
}
