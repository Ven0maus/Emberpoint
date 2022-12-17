using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Map;
using GoRogue.FOV;
using SadConsole.Entities;
using SadRogue.Primitives;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IEntity : IRenderable<Renderer>, IEntityBaseStats
    {
        Renderer RenderConsole { get; }
        int ObjectId { get; }
        bool IsVisible { get; set; }
        int CurrentBlueprintId { get; }
        IFOV FieldOfView { get; }
        int FieldOfViewRadius { get; set; }
        Point Position { get; set; }
        int Glyph { get; }
        void ResetFieldOfView();
        void MoveTowards(Point position, bool checkCanMove = true, Direction? direction = null, bool triggerMovementEffects = true);
        void MoveToBlueprint(int blueprintId);
        void MoveToBlueprint<T>(CellBlueprint<T> blueprint) where T : EmberCell, new();
        bool CanMoveTowards(Point position);
    }
}
