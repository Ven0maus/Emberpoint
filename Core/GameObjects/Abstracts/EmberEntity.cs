using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.Resources;
using Emberpoint.Core.UserInterface.Windows.ConsoleWindows;
using GoRogue.FOV;
using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Abstracts
{
    public abstract class EmberEntity : Entity, IEntity
    {
        public int ObjectId { get; }
        public int FieldOfViewRadius { get; set; }

        private IFOV _fieldOfView;
        public IFOV FieldOfView
        {
            get
            {
                return _fieldOfView ??= new RecursiveShadowcastingFOV(GridManager.Grid.FieldOfView);
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

        public int Glyph { get => Appearance.Glyph; set => Appearance.Glyph = value; }

        public Direction Facing  {get; private set; }

        public Renderer RenderConsole { get; private set; }

        public int CurrentBlueprintId { get; private set; }

        /// <summary>
        /// Call this when the grid changes to a new grid object. (Like going into the basement etc)
        /// </summary>
        public void ResetFieldOfView()
        {
            _fieldOfView = null;
        }

        public void MoveToBlueprint<T>(CellBlueprint<T> blueprint) where T : EmberCell, new()
            => MoveToBlueprint(blueprint.ObjectId);

        public virtual void MoveToBlueprint(int blueprintId)
        {
            CurrentBlueprintId = blueprintId;

            // Reset field of view when we move to another blueprint
            ResetFieldOfView();

            if (!(this is Player))
            {
                IsVisible = Game.Player != null && Game.Player.CurrentBlueprintId == CurrentBlueprintId;
            }
        }

        public EmberEntity(Color foreground, Color background, int glyph, CellBlueprint<EmberCell> blueprint = null, int zIndex = 0, bool subscribeMoveEvent = true) : base(foreground, background, glyph, zIndex)
        {
            ObjectId = EntityManager.GetUniqueId();
            CurrentBlueprintId = blueprint != null ? blueprint.ObjectId : (GridManager.ActiveBlueprint != null ? GridManager.ActiveBlueprint.ObjectId : -1);

            // Default stats
            MaxHealth = 100;

            Facing = Direction.Down;

            if (subscribeMoveEvent)
                PositionChanged += OnMove;
        }

        public virtual void OnMove(object sender, ValueChangedEventArgs<Point> args)
        {
            // Re-calculate the field of view
            FieldOfView.Calculate(Position, FieldOfViewRadius);
        }

        public bool GetInteractedCell(out Point cellPosition)
        {
            cellPosition = default;

            var neighbors = Position.Get4Neighbors()
                .Where(a => GridManager.Grid.InBounds(a))
                .Where(a => CanInteract(a.X, a.Y));

            int count = 0;
            foreach (var neighbor in neighbors)
            {
                if (CanInteract(neighbor.X, neighbor.Y))
                {
                    cellPosition = new Point(neighbor.X, neighbor.Y);
                    count++;
                }
            }

            if (count > 1)
            {
                var facingPosition = Position + Facing;
                if (CanInteract(facingPosition.X, facingPosition.Y))
                {
                    cellPosition = new Point(facingPosition.X, facingPosition.Y);
                    return true;
                }
                else
                {
                    cellPosition = default;
                }
            }
            return count == 1;
        }

        public bool CanInteract(int x, int y)
        {
            if (Health == 0) return false;
            if (!GridManager.Grid.InBounds(x, y)) return false;
            var cell = GridManager.Grid.GetCell(x, y);
            return cell.CellProperties.Interactable && !EntityManager.EntityExistsAt(x, y, CurrentBlueprintId) && cell.CellProperties.IsExplored;
        }

        public bool CanMoveTowards(Point position)
        {
            if (Health == 0) return false;
            var cell = GridManager.Grid.GetCell(position);
            return GridManager.Grid.InBounds(position) && cell.CellProperties.Walkable && !EntityManager.EntityExistsAt(position, CurrentBlueprintId) && cell.CellProperties.IsExplored;
        }

        public void RenderObject(Renderer console)
        {
            if (Health == 0) return;
            RenderConsole = console;
            console.Add(this);
        }

        public bool CheckInteraction(InteractionWindow interaction)
        {
            if (GetInteractedCell(out _))
            {
                interaction.PrintMessage(() => string.Format(Strings.InteractMessage, KeybindingsManager.GetKeybinding(Keybindings.Interact)));
                return true;
            }
            return false;
        }

        public void MoveTowards(Point position, bool checkCanMove = true, Direction? direction = null, bool triggerMovementEffects = true)
        {
            if (Health == 0) return;

            // Set correct facing direction regardless if we can move or not
            SetFacingDirection(position, direction);

            if (checkCanMove && !CanMoveTowards(position)) return;

            var oldPos = Position;
            Position = position;

            if (triggerMovementEffects)
            {
                // Check if the cell has movement effects to be executed
                ExecuteMovementEffects(new EntityMovedEventArgs(this, oldPos));
            }
        }

        public void MoveTowards(Direction position, bool checkCanMove = true)
        {
            var pos = Position;
            MoveTowards(pos += position, checkCanMove);
        }

        private void SetFacingDirection(Point newPosition, Direction? direction)
        {
            // Set facing direction
            var prevPos = Position;
            var difference = newPosition - prevPos;
            if (difference.X == 1 && difference.Y == 0)
                Facing = Direction.Right;
            else if (difference.X == -1 && difference.Y == 0)
                Facing = Direction.Left;
            else if (difference.X == 0 && difference.Y == 1)
                Facing = Direction.Down;
            else if (difference.X == 0 && difference.Y == -1)
                Facing = Direction.Up;
            else
                Facing = direction ?? Direction.Down;
        }

        public void UnRenderObject()
        {
            if (RenderConsole != null)
            {
                RenderConsole.Remove(this);
                RenderConsole = null;
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
