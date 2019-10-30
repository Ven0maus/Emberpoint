using GoRogue;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IEntity : IRenderable
    {
        int ObjectId { get; }
        FOV FieldOfView { get; }
        int FieldOfViewRadius { get; set; }
        Point Position { get; set; }
        void ResetFieldOfView();
        void MoveTowards(Point position, bool checkCanMove = true);
        bool CanMoveTowards(Point position);
    }
}
