using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using Emberpoint.Core.Resources;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class WorldmapWindow : Console, IUserInterface
    {
        public Console Console
        {
            get { return this; }
        }

        public WorldmapWindow(int width, int height) : base(width+2, height+2)
        {
            Position = new Point(4, 2);
            Global.CurrentScreen.Children.Add(this);
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

        public void BeforeCreate()
        {
            IsVisible = false;
        }

        public void Update()
        {
            Clear();
            this.ColorFill(Color.Black);
            this.DrawBorders(Width, Height, "O", "|", "-", Color.Gray);
            Print((Width / 2) - (Strings.Map.Length / 2), 0, Strings.Map, Color.Orange);

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
            for (int x=1; x < Width-1; x++)
            {
                for (int y=1; y < Height-1; y++)
                {
                    Print(x, y, new ColoredGlyph(0, darkGray, darkGray));
                }
            }

            // Show only explored cells and border cells
            var exploredCells = GridManager.Grid.GetCells(a => a.CellProperties.IsExplored || IsBorderCell(a));
            foreach (var cell in exploredCells)
            {
                Print(cell.Position.X + 1, cell.Position.Y + 1,
                    new ColoredGlyph(cell.Glyph, cell.CellProperties.NormalForeground,
                    cell.CellProperties.NormalBackground));
            }

            // Draw player
            if (Game.Player != null)
            {
                Print(Game.Player.Position.X + 1, Game.Player.Position.Y + 1, new ColoredGlyph(Game.Player.Glyph, Game.Player.Animation.CurrentFrame[0].Foreground,
                        Game.Player.Animation.CurrentFrame[0].Background));
            }
        }

        public void Show()
        {
            Game.Player.MapWindow.IsVisible = false;
            Update();
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
