using Emberpoint.Core.GameObjects.Entities;
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

            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            Moved += OnMove;
        }

        private void OnMove(object sender, EntityMovedEventArgs args)
        {
            if (FieldOfViewRadius > 0)
            {
                // Re-calculate the field of view
                FieldOfView.Calculate(Position, FieldOfViewRadius);

                // Only update visual for player entity
                if (this is Player)
                {
                    GridManager.Grid.DrawFieldOfView(this);
                }
            }
        }

        public bool CanMoveTowards(Point position)
        {
            return GridManager.Grid.InBounds(position) && GridManager.Grid.GetCell(position).Walkable && !EntityManager.EntityExistsAt(position);
        }

        public void RenderObject(Console console)
        {
            console.Children.Add(this);
        }

        public void MoveTowards(Point position, bool checkCanMove = true)
        {
            if (checkCanMove && !CanMoveTowards(position)) return;
            Position = position;
        }
    }
}
