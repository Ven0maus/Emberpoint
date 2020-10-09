using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using GoRogue;
using Microsoft.Xna.Framework;
using System;
using Tests.TestObjects.Grids;
using static SadConsole.Entities.Entity;

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

        public EventHandler<EntityMovedEventArgs> Moved;

        public int FieldOfViewRadius { get; set; } = 0;

        private FOV _fieldOfView;
        public FOV FieldOfView
        {
            get => _fieldOfView ??= new FOV(_grid.FieldOfView);
        }

        public int ObjectId { get; }
        public int Health { get; private set; }

        private int _maxHealth;
        public int MaxHealth
        {
            get { return _maxHealth; }
            set
            {
                _maxHealth = value;
                Health = _maxHealth;
            }
        }

        public int Glyph => throw new NotImplementedException();

        private readonly EmberGrid _grid;

        public BaseEntity()
        {
            // Not linked to a grid
            ObjectId = EntityManager.GetUniqueId();
            Moved += OnMove;
            MaxHealth = 100; // Default stats
        }

        public BaseEntity(BaseGrid grid)
        {
            _grid = grid;
            ObjectId = EntityManager.GetUniqueId();
            Moved += OnMove;
            MaxHealth = 100; // Default stats
        }

        private void OnMove(object sender, EntityMovedEventArgs args)
        {
            if (_grid != null)
            {
                if (FieldOfViewRadius > -1)
                {
                    // Re-calculate the field of view
                    FieldOfView.Calculate(Position, FieldOfViewRadius);
                }

                // Check if the cell has movement effects to be executed
                ExecuteMovementEffects(args);
            }
        }

        private void ExecuteMovementEffects(EntityMovedEventArgs args)
        {
            // Check if we moved
            if (args.FromPosition != Position)
            {
                var cell = _grid.GetCell(Position);
                if (cell.EffectProperties.EntityMovementEffects != null)
                {
                    foreach (var effect in cell.EffectProperties.EntityMovementEffects)
                    {
                        effect(this);
                    }
                }
            }
        }

        public bool CanMoveTowards(Point position)
        {
            if (Health == 0) return false;
            return _grid.InBounds(position) && _grid.GetCell(position).CellProperties.Walkable && !EntityManager.EntityExistsAt(position);
        }

        public void MoveTowards(Point position, bool checkCanMove = true)
        {
            if (Health == 0) return;
            if (checkCanMove && !CanMoveTowards(position)) return;
            var prevPos = Position;
            Position = position;
            Moved.Invoke(this, new EntityMovedEventArgs(null, prevPos));
        }

        public void ResetFieldOfView()
        {
            _fieldOfView = null;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                Health = 0;

                // Handle entity death
                // UnRenderObject();
                EntityManager.Remove(this);
            }
        }

        public void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
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
