using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole;
using Emberpoint.Core.UserInterface.Windows;
using Emberpoint.Core.GameObjects.Map;

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

        public Direction Facing  {get; private set; }

        public Console RenderConsole { get; private set; }

        public int CurrentBlueprintId { get; private set; }

        /// <summary>
        /// Call this when the grid changes to a new grid object. (Like going into the basement etc)
        /// </summary>
        public void ResetFieldOfView()
        {
            _fieldOfView = null;
        }

        public void MoveToBlueprint<T>(Blueprint<T> blueprint) where T : EmberCell, new()
        {
            CurrentBlueprintId = blueprint.ObjectId;

            // Reset field of view when we move to another blueprint
            ResetFieldOfView();

            if (!(this is Player))
            {
                IsVisible = Game.Player != null && Game.Player.CurrentBlueprintId == CurrentBlueprintId;
            }
        }

        public EmberEntity(Color foreground, Color background, int glyph, Blueprint<EmberCell> blueprint = null, int width = 1, int height = 1) : base(width, height)
        {
            ObjectId = EntityManager.GetUniqueId();
            CurrentBlueprintId = blueprint != null ? blueprint.ObjectId : GridManager.ActiveBlueprint.ObjectId;

            Font = Global.FontDefault.Master.GetFont(Constants.Map.Size);
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            // Default stats
            MaxHealth = 100;

            Facing = Direction.DOWN;

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

        public bool GetInteractedCell(out Point cellPosition)
        {
            cellPosition = default;
            var facingPosition = Position + Facing;
            if (CanInteract(facingPosition.X, facingPosition.Y))
            {
                cellPosition = new Point(facingPosition.X, facingPosition.Y);
                return true;
            }
            return false;
        }
        public bool CanInteract(int x, int y)
        {
            if (Health == 0) return false;
            if (!GridManager.Grid.InBounds(x, y)) return false;
            var cell = GridManager.Grid.GetCell(x, y);
            return cell.CellProperties.Interactable && !EntityManager.EntityExistsAt(x, y) && cell.CellProperties.IsExplored;
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
            RenderConsole = console;
            console.Children.Add(this);
        }

        public bool CheckInteraction(InteractionWindow interaction)
        {
            if (GetInteractedCell(out _))
            {
                interaction.PrintMessage("Press " + KeybindingsManager.GetKeybinding(Keybindings.Interact) +
                                         " to interact with object.");
                return true;
            }
            return false;
        }

        public void MoveTowards(Point position, bool checkCanMove = true, Direction direction = null, bool triggerMovementEffects = true)
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

        private void SetFacingDirection(Point newPosition, Direction direction)
        {
            // Set facing direction
            var prevPos = Position;
            var difference = newPosition - prevPos;
            if (difference.X == 1 && difference.Y == 0)
                Facing = Direction.RIGHT;
            else if (difference.X == -1 && difference.Y == 0)
                Facing = Direction.LEFT;
            else if (difference.X == 0 && difference.Y == 1)
                Facing = Direction.DOWN;
            else if (difference.X == 0 && difference.Y == -1)
                Facing = Direction.UP;
            else
                Facing = direction ?? Direction.DOWN;
        }

        public void UnRenderObject()
        {
            if (RenderConsole != null)
            {
                RenderConsole.Children.Remove(this);
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
