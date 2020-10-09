using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using GoRogue;
using Microsoft.Xna.Framework;
using System;
using Tests.TestObjects.Grids;

namespace Tests.TestObjects.Entities
{
    public class BaseEntity : IEntity
    {
        private Point _position;
        public Point Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value.X != _position.X || value.Y != _position.Y)
                {
                    Moved?.Invoke(this, new SadConsole.Entities.Entity.EntityMovedEventArgs(null, _position));
                }
                _position = value;
            }
        }

        public EventHandler<SadConsole.Entities.Entity.EntityMovedEventArgs> Moved;

        public int FieldOfViewRadius { get; set; } = 0;

        private FOV _fieldOfView;
        public FOV FieldOfView
        {
            get => _fieldOfView ??= new FOV(_grid.FieldOfView);
        }

        public int ObjectId { get; }

        private readonly EmberGrid _grid;

        public BaseEntity()
        {
            // Not linked to a grid
            ObjectId = EntityManager.GetUniqueId();
            Moved += OnMove;
        }

        public BaseEntity(BaseGrid grid)
        {
            _grid = grid;
            ObjectId = EntityManager.GetUniqueId();
            Moved += OnMove;
        }

        private void OnMove(object sender, SadConsole.Entities.Entity.EntityMovedEventArgs args)
        {
            if (_grid != null && FieldOfViewRadius > -1)
            {
                // Re-calculate the field of view
                FieldOfView.Calculate(Position, FieldOfViewRadius);
            }
        }

        public bool CanMoveTowards(Point position)
        {
            return _grid.InBounds(position) && _grid.GetCell(position).CellProperties.Walkable && !EntityManager.EntityExistsAt(position);
        }

        public void MoveTowards(Point position, bool checkCanMove = true)
        {
            if (checkCanMove && !CanMoveTowards(position)) return;
            var prevPos = Position;
            Position = position;
            Moved.Invoke(this, new SadConsole.Entities.Entity.EntityMovedEventArgs(null, prevPos));
        }

        public void ResetFieldOfView()
        {
            _fieldOfView = null;
        }

        public void RenderObject(SadConsole.Console console)
        {
            throw new NotImplementedException();
        }

        public void UnRenderObject()
        {
            throw new NotImplementedException();
        }
    }
}
