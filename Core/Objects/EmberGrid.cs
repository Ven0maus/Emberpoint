using Emberpoint.Core.Objects.Abstracts;
using Emberpoint.Core.Objects.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Console = SadConsole.Console;

namespace Emberpoint.Core.Objects
{
    public class EmberGrid : IRenderable
    {
        private EmberCell[] Cells { get; }

        public int GridSizeX { get; }
        public int GridSizeY { get; }

        public EmberGrid(int gridSizeX, int gridSizeY, Blueprint<EmberCell> blueprint)
        {
            GridSizeX = gridSizeX;
            GridSizeY = gridSizeY;
            Cells = blueprint.GetCells();
        }

        public EmberGrid(int gridSizeX, int gridSizeY, EmberCell[] cells)
        {
            GridSizeX = gridSizeX;
            GridSizeY = gridSizeY;
            Cells = cells;
        }

        public EmberCell GetCell(Point position)
        {
            return Cells[position.Y * GridSizeX + position.X];
        }

        public EmberCell GetCell(int x, int y)
        {
            return Cells[y * GridSizeX + x];
        }

        public void RenderObject(Console console)
        {
            console.SetSurface(Cells, GridSizeX, GridSizeY);
        }

        public void SetCell(EmberCell cell)
        {
            Cells[cell.Position.Y * GridSizeX + cell.Position.X] = cell;
        }

        public EmberCell[] GetNeighbors(EmberCell cell)
        {
            int x = cell.Position.X;
            int y = cell.Position.Y;

            var points = new List<Point>();
            if (!InBounds(x, y)) return Array.Empty<EmberCell>();

            if (InBounds(x + 1, y)) points.Add(new Point(x + 1, y));
            if (InBounds(x - 1, y)) points.Add(new Point(x - 1, y));
            if (InBounds(x, y + 1)) points.Add(new Point(x, y + 1));
            if (InBounds(x, y - 1)) points.Add(new Point(x, y - 1));
            if (InBounds(x + 1, y + 1)) points.Add(new Point(x + 1, y + 1));
            if (InBounds(x - 1, y - 1)) points.Add(new Point(x - 1, y - 1));
            if (InBounds(x + 1, y - 1)) points.Add(new Point(x + 1, y - 1));
            if (InBounds(x - 1, y + 1)) points.Add(new Point(x - 1, y + 1));

            return points.Select(a => GetCell(a)).ToArray();
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < GridSizeX && y < GridSizeY;
        }

        public bool InBounds(Point position)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < GridSizeX && position.Y < GridSizeY;
        }
    }
}
