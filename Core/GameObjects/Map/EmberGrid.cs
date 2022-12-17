using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using SadConsole;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using Console = SadConsole.Console;

namespace Emberpoint.Core.GameObjects.Map
{
    public class EmberGrid : IRenderable<Console>
    {
        protected EmberCell[] Cells { get; }

        private ArrayView<bool> _fieldOfView;
        public ArrayView<bool> FieldOfView
        {
            get
            {
                if (_fieldOfView != null) return _fieldOfView;
                _fieldOfView = new ArrayView<bool>(GridSizeX, GridSizeY);
                for (int x = 0; x < GridSizeX; x++)
                {
                    for (int y = 0; y < GridSizeY; y++)
                    {
                        var cell = GetNonClonedCell(x, y);
                        _fieldOfView[x, y] = !cell.CellProperties.BlocksFov;
                    }
                }
                return _fieldOfView;
            }
        }

        public int GridSizeX { get; }
        public int GridSizeY { get; }

        public CellBlueprint<EmberCell> CellBlueprint { get; }
        public ItemBlueprint<EmberItem> ItemBlueprint { get; }

        private MapWindow _map;
        protected MapWindow Map
        {
            get
            {
                return _map ??= UserInterfaceManager.Get<MapWindow>();
            }
        }

        private LightEngine<EmberCell> _lightEngine;
        public LightEngine<EmberCell> LightEngine
        {
            get
            {
                return _lightEngine ??= new LightEngine<EmberCell>();
            }
        }

        private Console _renderedConsole;

        public EmberGrid(CellBlueprint<EmberCell> cellBlueprint, ItemBlueprint<EmberItem> itemBlueprint)
        {
            GridSizeX = cellBlueprint.GridSizeX;
            GridSizeY = cellBlueprint.GridSizeY;
            CellBlueprint = cellBlueprint;
            ItemBlueprint = itemBlueprint;

            // Initialize cells
            Cells = CellBlueprint.GetCells();

            // Initialize items
            if (itemBlueprint != null)
            {
                foreach (var item in itemBlueprint.GetCells())
                {
                    item.IsVisible = true;
                    SetItem(item);
                }
            }
        }

        public EmberGrid(int gridSizeX, int gridSizeY, EmberCell[] cells, 
            CellBlueprint<EmberCell> cellBlueprint = null, ItemBlueprint<EmberItem> itemBlueprint = null)
        {
            GridSizeX = gridSizeX;
            GridSizeY = gridSizeY;
            CellBlueprint = cellBlueprint;
            ItemBlueprint = itemBlueprint;
            Cells = cells;

            // Initialize items
            if (itemBlueprint != null)
            {
                foreach (var item in itemBlueprint.GetCells())
                    SetItem(item);
            }
        }

        /// <summary>
        /// Call this after the player is loaded once to calibrate the light engine.
        /// </summary>
        public void CalibrateLightEngine()
        {
            LightEngine.Calibrate(Cells);
        }

        public IEnumerable<EmberCell> GetCells(Func<EmberCell, bool> criteria)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    if (criteria.Invoke(GetNonClonedCell(x, y)))
                    {
                        yield return GetCell(x, y);
                    }
                }
            }
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
        /// Retrieves the first cell that matches the given criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public EmberCell GetCell(Func<EmberCell, bool> criteria)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    if (criteria.Invoke(GetNonClonedCell(x, y)))
                    {
                        return GetCell(x, y);
                    }
                }
            }
            return null;
        }

        public bool ContainsEntity(Point position, int blueprintId)
        {
            return EntityManager.EntityExistsAt(position.X, position.Y, blueprintId);
        }

        public bool ContainsEntity(int x, int y, int blueprintId)
        {
            return EntityManager.EntityExistsAt(x, y, blueprintId);
        }

        /// <summary>
        /// Use this when updating multiple cells at a time for performance.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected EmberCell GetNonClonedCell(int x, int y)
        {
            return Cells[y * GridSizeX + x];
        }

        public void SetCell(EmberCell cell, bool calculateEntitiesFov = false, bool adjustLights = true)
        {
            var originalCell = Cells[cell.Position.Y * GridSizeX + cell.Position.X];

            // Update the map fov values if the walkable is changed
            bool updateFieldOfView = originalCell.CellProperties.BlocksFov != cell.CellProperties.BlocksFov;
            
            // Adjust the lights of the tiles
            if (adjustLights)
            {
                LightEngine.AdjustLightSource(cell, originalCell);
            }

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

        public void SetItem(EmberItem item)
        {
            var cell = GetNonClonedCell(item.Position.X, item.Position.Y);
            if (!cell.CellProperties.Walkable)
                throw new Exception($"This cell is invalid for item placement. ({item.Position})");
            if (cell.EmberItem != null)
                throw new Exception($"An item is already placed on position: {item.Position}");

            cell.EmberItem = item;
            cell.IsVisible = cell.CellProperties.IsExplored;
            item.IsVisible = cell.IsVisible;
            item.RenderObject(Map.EntityRenderer);
        }

        public void RemoveItem(Point position) => RemoveItem(position.X, position.Y);
        public void RemoveItem(int x, int y)
        {
            var cell = GetNonClonedCell(x, y);
            if (cell.EmberItem == null) return;

            cell.EmberItem.UnRenderObject();
            cell.EmberItem = null;
            cell.IsVisible = true;
        }

        public void RenderObject(Console console)
        {
            _renderedConsole = console;
            console.Surface = new CellSurface(console.ViewWidth, console.ViewHeight, GridSizeX, GridSizeY, Cells);
            console.IsDirty = true;
        }

        /// <summary>
        /// Updates the FieldOfView data for this cell.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void UpdateFieldOfView(int x, int y)
        {
            var cell = GetNonClonedCell(x, y);
            FieldOfView[x, y] = !cell.CellProperties.BlocksFov;
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
                    var cell = GetNonClonedCell(x, y);
                    FieldOfView[x, y] = !cell.CellProperties.BlocksFov;
                }
            }
        }

        public IEnumerable<EmberCell> GetCellsInFov(IEntity entity)
        {
            var cells = new List<EmberCell>();
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    var cell = GetNonClonedCell(x, y);
                    if (entity.FieldOfView.BooleanResultView[x, y])
                        cells.Add(cell);
                }
            }
            return cells;
        }

        public IEnumerable<EmberCell> GetCellsInFov(IEntity entity, int fovRadius)
        {
            var originalFov = entity.FieldOfViewRadius;

            entity.FieldOfViewRadius = fovRadius;
            EntityManager.RecalculatFieldOfView(entity, false);

            var cells = GetCellsInFov(entity).ToList();

            entity.FieldOfViewRadius = originalFov;
            EntityManager.RecalculatFieldOfView(entity, false);

            return cells;
        }

        public IEnumerable<EmberCell> GetExploredCellsInFov(IEntity entity)
        {
            return GetCellsInFov(entity).Where(cell => cell.CellProperties.IsExplored);
        }

        public IEnumerable<EmberCell> GetExploredCellsInFov(IEntity entity, int fovRadius)
        {
            return GetCellsInFov(entity, fovRadius).Where(cell => cell.CellProperties.IsExplored);
        }

        public void DrawFieldOfView(IEntity entity, bool discoverUnexploredTiles = false)
        {
            // Check if there is a lightsource nearby, then explore all cells enlighted by it automatically
            var prevFov = entity.FieldOfViewRadius;

            entity.FieldOfViewRadius = Constants.Player.DiscoverLightsRadius;
            EntityManager.RecalculatFieldOfView(entity, false);

            // Get cells that emit light
            var cellsThatEmitLight = GridManager.Grid.GetCellsInFov(entity)
                .Where(a => a.LightProperties.EmitsLight && !a.CellProperties.IsExplored)
                .ToList();

            // Actual cells we see
            foreach (var lightCell in cellsThatEmitLight)
            {
                var cell = GetNonClonedCell(lightCell.Position.X, lightCell.Position.Y);
                cell.CellProperties.IsExplored = true;
                cell.IsVisible = true;
                if (cell.EmberItem != null)
                    cell.EmberItem.IsVisible = cell.IsVisible;
            }

            // Reset entity fov
            if (prevFov != entity.FieldOfViewRadius)
            {
                entity.FieldOfViewRadius = prevFov;
                EntityManager.RecalculatFieldOfView(entity, false);
            }

            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    var cell = GetNonClonedCell(x, y);

                    if (discoverUnexploredTiles && !cell.CellProperties.IsExplored)
                    {
                        if (entity.FieldOfView.BooleanResultView[x, y])
                        {
                            cell.CellProperties.IsExplored = true;
                        }
                    }

                    // Cells near light sources are automatically visible
                    if (cell.LightProperties.Brightness > 0f && !cell.LightProperties.EmitsLight && !cell.CellProperties.IsExplored && 
                        cell.LightProperties.LightSources.Any(a => a.CellProperties.IsExplored))
                    {
                        cell.CellProperties.IsExplored = true;
                    }  

                    cell.IsVisible = cell.CellProperties.IsExplored;
                    if (cell.EmberItem != null)
                        cell.EmberItem.IsVisible = cell.IsVisible;

                    SetCellColors(cell);
                    SetCell(cell);
                }
            }

            // Redraw the map
            if (Map != null)
            {
                Map.Refresh();
            }
        }

        public void SetCellColors(EmberCell cell)
        {
            if (cell.LightProperties.Brightness > 0f)
            {
                var closestLightSource = cell.GetClosestLightSource();
                Color lightSourceColor = closestLightSource?.LightProperties.LightColor ?? Color.White;
                if (cell.CellProperties.NormalForeground != Color.Transparent)
                    cell.Foreground = Color.Lerp(cell.CellProperties.NormalForeground, lightSourceColor, cell.LightProperties.Brightness);
                if (cell.CellProperties.NormalBackground != Color.Transparent)
                cell.Background = Color.Lerp(cell.CellProperties.NormalBackground, lightSourceColor, cell.LightProperties.Brightness / 3f);
            }
            else
            {
                cell.Foreground = cell.CellProperties.ForegroundFov;
                cell.Background = cell.CellProperties.BackgroundFov;
            }
        }

        public IEnumerable<EmberCell> GetNeighbors(EmberCell cell)
        {
            int x = cell.Position.X;
            int y = cell.Position.Y;
            var points = new List<Point>();
            if (!InBounds(x, y)) return Enumerable.Empty<EmberCell>();
            if (InBounds(x + 1, y)) points.Add(new Point(x + 1, y));
            if (InBounds(x - 1, y)) points.Add(new Point(x - 1, y));
            if (InBounds(x, y + 1)) points.Add(new Point(x, y + 1));
            if (InBounds(x, y - 1)) points.Add(new Point(x, y - 1));
            if (InBounds(x + 1, y + 1)) points.Add(new Point(x + 1, y + 1));
            if (InBounds(x - 1, y - 1)) points.Add(new Point(x - 1, y - 1));
            if (InBounds(x + 1, y - 1)) points.Add(new Point(x + 1, y - 1));
            if (InBounds(x - 1, y + 1)) points.Add(new Point(x - 1, y + 1));
            return points.Select(a => GetCell(a));
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
