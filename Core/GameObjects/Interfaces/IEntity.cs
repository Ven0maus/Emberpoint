using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;
using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IEntity : IRenderable, IEntityBaseStats
    {
        Console RenderConsole { get; }
        int ObjectId { get; }
        int CurrentBlueprintId { get; }
        FOV FieldOfView { get; }
        int FieldOfViewRadius { get; set; }
        Point Position { get; set; }
        int Glyph { get; }
        void ResetFieldOfView();
        void MoveTowards(Point position, bool checkCanMove = true, Direction direction = null, bool triggerMovementEffects = true);
        void MoveToBlueprint(int blueprintId);
        void MoveToBlueprint<T>(Blueprint<T> blueprint) where T : EmberCell, new();
        bool CanMoveTowards(Point position);
    }
}
