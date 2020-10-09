using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Items;
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
                return _fieldOfView ??= new FOV(GridManager.Grid.FieldOfView);
            }
        }

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

        public int Glyph { get => Animation.CurrentFrame[0].Glyph; }

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

            // Default stats
            MaxHealth = 100;

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

            // Check if the cell has movement effects to be executed
            ExecuteMovementEffects(args);
        }

        public bool CanMoveTowards(Point position)
        {
            if (Health == 0) return false;
            var cell = GridManager.Grid.GetCell(position);
            return GridManager.Grid.InBounds(position) && cell.CellProperties.Walkable && !EntityManager.EntityExistsAt(position) && cell.CellProperties.IsExplored;
        }

        public void RenderObject(Console console)
        {
            if (Health == 0) return;
            _renderedConsole = console;
            console.Children.Add(this);
        }

        public void MoveTowards(Point position, bool checkCanMove = true)
        {
            if (Health == 0) return;
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

        private void ExecuteMovementEffects(EntityMovedEventArgs args)
        {
            // Check if we moved
            if (args.FromPosition != Position)
            {
                var cell = GridManager.Grid.GetCell(Position);
                if (cell.EffectProperties.EntityMovementEffects != null)
                {
                    foreach (var effect in cell.EffectProperties.EntityMovementEffects)
                    {
                        effect(this);
                    }
                }
            }
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                Health = 0;

                // Handle entity death
                UnRenderObject();
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
    }
}
