using GoRogue;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IEntity : IRenderable, IEntityBaseStats
    {
        int ObjectId { get; }
        FOV FieldOfView { get; }
        int FieldOfViewRadius { get; set; }
        Point Position { get; set; }
        int Glyph { get; }
        void ResetFieldOfView();
        void MoveTowards(Point position, bool checkCanMove = true);
        bool CanMoveTowards(Point position);
    }
}
