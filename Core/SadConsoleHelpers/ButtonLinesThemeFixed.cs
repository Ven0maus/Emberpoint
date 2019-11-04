using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;
using System;

namespace Emberpoint.Core.SadConsoleHelpers
{
    /// <summary>
    /// A 3D theme of the button control using thin lines. Supports the SadConsole extended character set.
    /// </summary>
    public class ButtonLinesThemeFixed : ButtonTheme
    {
        public Cell TopLeftLineColors;

        public Cell BottomRightLineColors;

        public bool UseExtended;

        public ButtonLinesThemeFixed()
        {
            UseExtended = true;
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors themeColors)
        {
            base.RefreshTheme(themeColors);

            TopLeftLineColors = new Cell(themeColors.Gray, Color.Transparent);
            BottomRightLineColors = new Cell(themeColors.GrayDark, Color.Transparent);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;

            if (!button.IsDirty) return;

            Cell appearance;
            var mouseDown = false;
            var mouseOver = false;
            var focused = false;

            if (Helpers.HasFlag(button.State, ControlStates.Disabled))
                appearance = Disabled;
            else
                appearance = Normal;

            if (Helpers.HasFlag(button.State, ControlStates.MouseLeftButtonDown) ||
                Helpers.HasFlag(button.State, ControlStates.MouseRightButtonDown))
                mouseDown = true;

            if (Helpers.HasFlag(button.State, ControlStates.MouseOver))
                mouseOver = true;

            if (Helpers.HasFlag(button.State, ControlStates.Focused))
                focused = true;


            // Middle part of the button for text.
            var middle = button.Surface.Height != 1 ? button.Surface.Height / 2 : 0;
            var topleftcolor = !mouseDown ? TopLeftLineColors.Foreground : BottomRightLineColors.Foreground;
            var bottomrightcolor = !mouseDown ? BottomRightLineColors.Foreground : TopLeftLineColors.Foreground;
            Color textColor = Normal.Foreground;

            if (button.Surface.Height > 1 && button.Surface.Height % 2 == 0)
                middle -= 1;

            if (mouseOver)
                textColor = MouseOver.Foreground;
            else if (focused)
                textColor = Focused.Foreground;

            // Extended font draw
            if (button.Parent.Font.Master.IsSadExtended && UseExtended)
            {
                // Redraw the control
                button.Surface.Fill(appearance.Foreground, appearance.Background,
                                    appearance.Glyph, SpriteEffects.None);

                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width), textColor);

                if (button.Height == 1)
                {
                    button.Surface.SetDecorator(0, button.Surface.Width,
                                                        new FontMaster.GlyphDefinition(CellSurface.ConnectedLineThinExtended[1], SpriteEffects.None).CreateCellDecorator(topleftcolor),
                                                        new FontMaster.GlyphDefinition(CellSurface.ConnectedLineThinExtended[7], SpriteEffects.None).CreateCellDecorator(bottomrightcolor));
                    button.Surface.AddDecorator(0, 1, button.Parent.Font.Master.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(button.Surface.Width - 1, 1, button.Parent.Font.Master.GetDecorator("box-edge-right", bottomrightcolor));
                }
                else if (button.Height == 2)
                {
                    button.Surface.SetDecorator(0, button.Surface.Width,
                                                        new FontMaster.GlyphDefinition(CellSurface.ConnectedLineThinExtended[1], SpriteEffects.None).CreateCellDecorator(topleftcolor));

                    button.Surface.SetDecorator(button.Surface.GetIndexFromPoint(0, button.Surface.Height - 1), button.Surface.Width,
                                                        new FontMaster.GlyphDefinition(CellSurface.ConnectedLineThinExtended[7], SpriteEffects.None).CreateCellDecorator(bottomrightcolor));

                    button.Surface.AddDecorator(0, 1, button.Parent.Font.Master.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(button.Surface.GetIndexFromPoint(0, 1), 1, button.Parent.Font.Master.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(button.Surface.Width - 1, 1, button.Parent.Font.Master.GetDecorator("box-edge-right", bottomrightcolor));
                    button.Surface.AddDecorator(button.Surface.GetIndexFromPoint(button.Surface.Width - 1, 1), 1, button.Parent.Font.Master.GetDecorator("box-edge-right", bottomrightcolor));
                }
                else
                {
                    button.Surface.SetDecorator(0, button.Surface.Width,
                                                        new FontMaster.GlyphDefinition(CellSurface.ConnectedLineThinExtended[1], SpriteEffects.None).CreateCellDecorator(topleftcolor));

                    button.Surface.SetDecorator(button.Surface.GetIndexFromPoint(0, button.Surface.Height - 1), button.Surface.Width,
                                                        new FontMaster.GlyphDefinition(CellSurface.ConnectedLineThinExtended[7], SpriteEffects.None).CreateCellDecorator(bottomrightcolor));

                    button.Surface.AddDecorator(0, 1, button.Parent.Font.Master.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(button.Surface.GetIndexFromPoint(0, button.Surface.Height - 1), 1, button.Parent.Font.Master.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(button.Surface.Width - 1, 1, button.Parent.Font.Master.GetDecorator("box-edge-right", bottomrightcolor));
                    button.Surface.AddDecorator(button.Surface.GetIndexFromPoint(button.Surface.Width - 1, button.Surface.Height - 1), 1, button.Parent.Font.Master.GetDecorator("box-edge-right", bottomrightcolor));

                    for (int y = 0; y < button.Surface.Height - 2; y++)
                    {
                        button.Surface.AddDecorator(button.Surface.GetIndexFromPoint(0, y + 1), 1, button.Parent.Font.Master.GetDecorator("box-edge-left", topleftcolor));
                        button.Surface.AddDecorator(button.Surface.GetIndexFromPoint(button.Surface.Width - 1, y + 1), 1, button.Parent.Font.Master.GetDecorator("box-edge-right", bottomrightcolor));
                    }
                }
            }
            else // Non extended normal draw
            {
                button.Surface.Fill(appearance.Foreground, appearance.Background,
                    appearance.Glyph, SpriteEffects.None);

                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width), textColor);

                button.Surface.DrawBox(new Rectangle(0, 0, button.Width, button.Surface.Height), new Cell(topleftcolor, TopLeftLineColors.Background, 0),
                    connectedLineStyle: button.Parent.Font.Master.IsSadExtended ? CellSurface.ConnectedLineThinExtended : CellSurface.ConnectedLineThin);

                //SadConsole.Algorithms.Line(0, 0, button.Width - 1, 0, (x, y) => { return true; });

                button.Surface.DrawLine(Point.Zero, new Point(button.Width - 1, 0), topleftcolor, appearance.Background);
                button.Surface.DrawLine(Point.Zero, new Point(0, button.Surface.Height - 1), topleftcolor, appearance.Background);
                button.Surface.DrawLine(new Point(button.Width - 1, 0), new Point(button.Width - 1, button.Surface.Height - 1), bottomrightcolor, appearance.Background);
                button.Surface.DrawLine(new Point(1, button.Surface.Height - 1), new Point(button.Width - 1, button.Surface.Height - 1), bottomrightcolor, appearance.Background);
            }

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone()
        {
            return new ButtonLinesThemeFixed()
            {
                Colors = Colors.Clone(),
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
                TopLeftLineColors = TopLeftLineColors.Clone(),
                BottomRightLineColors = BottomRightLineColors.Clone()
            };
        }
    }
}
