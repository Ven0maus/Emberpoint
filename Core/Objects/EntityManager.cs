﻿using Emberpoint.Core.Objects.Abstracts;
using Emberpoint.Core.Objects.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.Objects
{
    public static class EntityManager
    {
        /// <summary>
        /// Returns a new entity or null, if the position is already taken.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T Create<T>(Point position) where T : IEntity, new()
        {
            if (EntityExistsAt(position))
            {
                return default;
            }

            var entity = new T()
            {
                Position = position
            };
            EntityDatabase.Entities.Add(entity.ObjectId, entity);
            return entity;
        }

        public static int GetUniqueId()
        {
            return EntityDatabase.GetUniqueId();
        }

        public static void Remove(EmberEntity entity)
        {
            EntityDatabase.Entities.Remove(entity.ObjectId);
        }

        public static void Clear()
        {
            EntityDatabase.Reset();
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
            return GetEntityAt<EmberEntity>(x, y) != null;
        }

        public static bool EntityExistsAt(Point position)
        {
            return GetEntityAt<EmberEntity>(position) != null;
        }

        public static void RecalculatFieldOfView()
        {
            // Recalculate the fov of all entities
            var entities = GetEntities<IEntity>();
            foreach (var entity in entities.Where(a => a.FieldOfViewRadius > 0))
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
        }
    }
}
