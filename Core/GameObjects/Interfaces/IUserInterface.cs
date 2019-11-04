using Microsoft.Xna.Framework;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IUserInterface
    {
        Point Position { get; set; }
        bool IsVisible { get; set; }
        bool IsDirty { get; set; }
        SadConsole.Console Console { get; }

        void Update();
    }
}
