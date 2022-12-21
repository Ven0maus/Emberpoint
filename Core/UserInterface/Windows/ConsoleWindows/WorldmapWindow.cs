using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.Resources;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

namespace Emberpoint.Core.UserInterface.Windows.ConsoleWindows
{
    public class WorldmapWindow : BorderedWindow
    {
        public WorldmapWindow(int width, int height) : base(width + 2, height + 2, 0, 0)
        {
            Title = Strings.Map;
            Position = new Point(4, 2);
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            // Hide map again, since player focus is on this console and he cannot move around.
            if (info.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Map)))
            {
                Hide();
                return true;
            }
            return false;
        }

        public override void BeforeCreate()
        {
            IsVisible = false;
        }

        public override void Refresh()
        {
            //Surface.Print(Width / 2 - Strings.Map.Length / 2, 0, Strings.Map, Color.Orange);

            if (GridManager.Grid != null)
                DrawMap();
        }

        private bool IsBorderCell(EmberCell cell)
        {
            return cell.Position.X == 0 || cell.Position.Y == 0 ||
                cell.Position.X == GridManager.Grid.GridSizeX - 1 ||
                cell.Position.Y == GridManager.Grid.GridSizeY - 1;
        }

        private void DrawMap()
        {
            // Make cells dark
            var darkGray = new Color(10, 10, 10, 255);
            for (int x = 0; x < Content.Width; x++)
            {
                for (int y = 0; y < Content.Height; y++)
                {
                    Content.SetGlyph(x, y, 0, darkGray, darkGray);
                }
            }

            // Show only explored cells and border cells
            var exploredCells = GridManager.Grid.GetCells(a => a.CellProperties.IsExplored || IsBorderCell(a));
            foreach (var cell in exploredCells)
            {
                Content.SetGlyph(cell.Position.X, cell.Position.Y, cell.Glyph,
                    cell.CellProperties.NormalForeground, cell.CellProperties.NormalBackground);
            }

            // Draw player
            if (Game.Player != null)
            {
                Content.SetGlyph(Game.Player.Position.X, Game.Player.Position.Y, Game.Player.Glyph,
                    Game.Player.Appearance.Foreground, Game.Player.Appearance.Background);
            }
        }

        public void Show()
        {
            Game.Player.MapWindow.IsVisible = false;
            Refresh();
            IsFocused = true;
            Game.Player.IsFocused = false;
            IsVisible = true;
        }

        public void Hide()
        {
            Game.Player.MapWindow.IsVisible = true;
            IsFocused = false;
            Game.Player.IsFocused = true;
            IsVisible = false;
        }
    }
}
