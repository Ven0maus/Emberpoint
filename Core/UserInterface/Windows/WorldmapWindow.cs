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
            Update();
            IsFocused = true;
            Game.Player.IsFocused = false;
            IsVisible = true;
        }

        public void Hide()
        {
            IsFocused = false;
            Game.Player.IsFocused = true;
            IsVisible = false;
        }
    }
}
