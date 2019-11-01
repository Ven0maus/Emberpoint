using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Console = SadConsole.Console;

namespace Emberpoint.Core.GameObjects.Map
{
    public class EmberGrid : IRenderable
    {
        private EmberCell[] _cells;
        private EmberCell[] Cells
        {
            get
            {
                return _cells ?? (_cells = Blueprint.GetCells());
            }
        }

        private ArrayMap<bool> _fieldOfView;
        public ArrayMap<bool> FieldOfView
        {
            get
            {
                if (_fieldOfView != null) return _fieldOfView;
                _fieldOfView = new ArrayMap<bool>(GridSizeX, GridSizeY);
                for (int x = 0; x < GridSizeX; x++)
                {
                    for (int y = 0; y < GridSizeY; y++)
                    {
                        _fieldOfView[x, y] = !GetCell(x, y).BlocksFov;
                    }
                }
                return _fieldOfView;
            }
        }

        public int GridSizeX { get; }
        public int GridSizeY { get; }

        public Blueprint<EmberCell> Blueprint { get; }

        private MapWindow _map;
        private MapWindow Map
        {
            get
            {
                return _map ?? (_map = UserInterfaceManager.Get<MapWindow>());
            }
        }

        private LightEngine<EmberCell> _lightEngine;
        private LightEngine<EmberCell> LightEngine
        {
            get
            {
                return _lightEngine ?? (_lightEngine = new LightEngine<EmberCell>());
            }
        }

        private Console _renderedConsole;

        public EmberGrid(Blueprint<EmberCell> blueprint)
        {
            GridSizeX = blueprint.GridSizeX;
            GridSizeY = blueprint.GridSizeY;
            Blueprint = blueprint;
        }

        public EmberGrid(int gridSizeX, int gridSizeY, EmberCell[] cells)
        {
            GridSizeX = gridSizeX;
            GridSizeY = gridSizeY;
            _cells = cells;

            _fieldOfView = new ArrayMap<bool>(gridSizeX, gridSizeY);
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    _fieldOfView[x, y] = !GetCell(x, y).BlocksFov;
                }
            }
        }

        /// <summary>
        /// Call this after the player is loaded once to calibrate the light engine.
        /// </summary>
        public void CalibrateLightEngine()
        {
            LightEngine.Calibrate(Cells);
        }

        public EmberCell GetCell(Point position)
        {
            return Cells[position.Y * GridSizeX + position.X].Clone();
        }

        public EmberCell GetCell(int x, int y)
        {
            return Cells[y * GridSizeX + x].Clone();
        }

        /// <summary>
        /// Use this when updating multiple cells at a time for performance.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private EmberCell GetNonClonedCell(int x, int y)
        {
            return Cells[y * GridSizeX + x];
        }

        public void SetCell(EmberCell cell, bool calculateEntitiesFov = false)
        {
            var originalCell = Cells[cell.Position.Y * GridSizeX + cell.Position.X];

            // Update the map fov values if the walkable is changed
            bool updateFieldOfView = originalCell.BlocksFov != cell.BlocksFov;

            // Adjust neighboring cell light levels if this object gives of light
            LightEngine.AdjustLightLevels(cell, originalCell);

            // Copy the new cell data
            originalCell.CopyFrom(cell);

            if (updateFieldOfView)
            {
                UpdateFieldOfView(cell.Position.X, cell.Position.Y);

                if (calculateEntitiesFov)
                {
                    // Recalculate the fov of all entities
                    EntityManager.RecalculatFieldOfView();
                }
            }
        }

        public void RenderObject(Console console)
        {
            _renderedConsole = console;
            console.SetSurface(Cells, GridSizeX, GridSizeY);
        }

        /// <summary>
        /// Updates the FieldOfView data for this cell.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void UpdateFieldOfView(int x, int y)
        {
            FieldOfView[x, y] = !GetNonClonedCell(x, y).BlocksFov;
        }

        /// <summary>
        /// Updates the FieldOfView data for the entire grid and all the entities.
        /// </summary>
        public void UpdateFieldOfView()
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    FieldOfView[x, y] = !GetNonClonedCell(x, y).BlocksFov;
                }
            }
        }

        public void DrawFieldOfView(IEntity entity)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    var cell = GetNonClonedCell(x, y);
                    SetCellColors(cell, entity);
                    SetCell(cell);
                }
            }

            // Redraw the map
            Map.Update();
        }

        public void SetCellColors(EmberCell cell, IEntity entity)
        {
            // If the cell is in the field of view
            if (entity == null || entity.FieldOfView.BooleanFOV[cell.Position])
            {
                if (cell.Brightness > 0f && cell.LightSources != null)
                    cell.Foreground = Color.Lerp(cell.GetClosestLightSource().LightColor, Color.White, cell.Brightness);
                else
                    cell.Foreground = cell.NormalForeground;
            }
            else
            {
                if (cell.Brightness > 0f && cell.LightSources != null)
                    cell.Foreground = Color.Lerp(Color.Lerp(cell.GetClosestLightSource().LightColor, Color.Black, .5f), Color.White, cell.Brightness);
                else
                    cell.Foreground = cell.ForegroundFov;
            }
        }

        public void SetCellColors(EmberCell cell, IEntity entity, Color foreground, Color foregroundFov)
        {
            // If the cell is in the field of view
            if (entity == null || entity.FieldOfView.BooleanFOV[cell.Position])
            {
                if (cell.Brightness > 0f)
                    cell.Foreground = Color.Lerp(foreground, Color.White, cell.Brightness);
                else
                    cell.Foreground = cell.NormalForeground;
            }
            else
            {
                if (cell.Brightness > 0f)
                    cell.Foreground = Color.Lerp(foregroundFov, Color.White, cell.Brightness);
                else
                    cell.Foreground = cell.ForegroundFov;
            }
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

        public void UnRenderObject()
        {
            if (_renderedConsole != null)
            {
                _renderedConsole.Clear();
                _renderedConsole = null;
            }
        }
    }
}
