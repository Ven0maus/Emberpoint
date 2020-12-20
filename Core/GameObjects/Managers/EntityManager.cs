﻿using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Map;
using Microsoft.Xna.Framework;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class EntityManager
    {
        /// <summary>
        /// Returns a new entity or null, if the position is already taken.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="grid">Custom grid, for tests</param>
        /// <returns></returns>
        public static T Create<T>(Point position, EmberGrid grid = null) where T : IEntity
        {
            if (EntityExistsAt(position))
            {
                return default;
            }

            T entity;
            if (grid != null)
            {
                entity = (T)Activator.CreateInstance(typeof(T), grid);
                entity.Position = position;
            }
            else
            {
                entity = Activator.CreateInstance<T>();
                entity.Position = position;
            }

            EntityDatabase.Entities.Add(entity.ObjectId, entity);
            return entity;
        }

        public static int GetUniqueId()
        {
            return EntityDatabase.GetUniqueId();
        }

        public static void Remove(IEntity entity)
        {
            EntityDatabase.Entities.Remove(entity.ObjectId);
        }

        public static void Clear(bool unRenderEntities, Func<IEntity, bool> criteria = null)
        {
            if (unRenderEntities)
            {
                foreach (var entity in EntityDatabase.Entities)
                {
                    if (criteria == null || criteria.Invoke(entity.Value))
                    {
                        entity.Value.UnRenderObject();
                    }
                }
            }
            EntityDatabase.Reset();
        }

        public static void MovePlayerToBlueprint<T>(Blueprint<T> blueprint) where T : EmberCell, new()
        {
            // Set player's blueprint layer
            Game.Player.MoveToBlueprint(blueprint);

            foreach (var entity in EntityDatabase.Entities)
            {
                var castedEntity = (Entity)entity.Value;
                if (entity.Value.CurrentBlueprintId == blueprint.ObjectId)
                {
                    // Make all entities on current blueprint visible
                    castedEntity.IsVisible = true;
                }
                else
                {
                    // Make all entities on another blueprint invisible
                    castedEntity.IsVisible = false;
                }
            }
        }

        public static T[] GetEntities<T>(Func<T, bool> criteria = null) where T : IEntity
        {
            var collection = EntityDatabase.Entities.Values.ToArray().OfType<T>();
            if (criteria != null)
            {
                collection = collection.Where(criteria.Invoke);
            }
            return collection.ToArray();
        }

        public static T GetEntityAt<T>(Point position) where T : IEntity
        {
            return (T)EntityDatabase.Entities.Values.SingleOrDefault(a => a.Position == position);
        }

        public static T GetEntityAt<T>(int x, int y) where T : IEntity
        {
            return (T)EntityDatabase.Entities.Values.SingleOrDefault(a => a.Position.X == x && a.Position.Y == y);
        }

        public static bool EntityExistsAt(int x, int y)
        {
            return GetEntityAt<IEntity>(x, y) != null;
        }

        public static bool EntityExistsAt(Point position)
        {
            return GetEntityAt<IEntity>(position) != null;
        }

        public static void RecalculatFieldOfView(IEntity entity, bool redrawFov = true)
        {
            entity.FieldOfView.Calculate(entity.Position, entity.FieldOfViewRadius);
            if (entity is Player && redrawFov)
                GridManager.Grid.DrawFieldOfView(entity);
        }

        public static void RecalculatFieldOfView()
        {
            // Recalculate the fov of all entities
            var entities = GetEntities<IEntity>();
            foreach (var entity in entities)
            {
                entity.FieldOfView.Calculate(entity.Position, entity.FieldOfViewRadius);
                if (entity is Player)
                    GridManager.Grid.DrawFieldOfView(entity);
            }
        }

        private static class EntityDatabase
        {
            public static readonly Dictionary<int, IEntity> Entities = new Dictionary<int, IEntity>();

            private static int _currentId;
            public static int GetUniqueId()
            {
                return _currentId++;
            }

            public static void Reset()
            {
                Entities.Clear();
                _currentId = 0;
            }

            public static void ResetExcept(params int[] ids)
            {
                var toRemove = new List<int>();
                foreach (var entity in Entities)
                {
                    toRemove.Add(entity.Key);
                }

                toRemove = toRemove.Except(ids).ToList();
                foreach (var id in toRemove)
                    Entities.Remove(id);

                _currentId = Entities.Count == 0 ? 0 : (Entities.Max(a => a.Key) + 1);
            }
        }
    }
}
