using Emberpoint.Core.GameObjects.Interfaces;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class ItemManager
    {
        public static int GetUniqueId()
        {
            return ItemDatabase.GetUniqueId();
        }

        public static void Clear()
        {
            ItemDatabase.Reset();
        }

        /// <summary>
        /// This method is automatically called when creating a new EmberItem.
        /// </summary>
        /// <param name="item"></param>
        public static void Add(IItem item)
        {
            if (!ItemDatabase.Items.ContainsKey(item.ObjectId))
            {
                ItemDatabase.Items.Add(item.ObjectId, item);
            }
        }

        public static void Remove(IItem item)
        {
            ItemDatabase.Items.Remove(item.ObjectId);
        }

        public static T[] GetItems<T>(Func<T, bool> criteria = null) where T : IItem
        {
            var collection = ItemDatabase.Items.Values.ToArray().OfType<T>();
            if (criteria != null)
            {
                collection = collection.Where(criteria.Invoke);
            }
            return collection.ToArray();
        }

        public static T GetItemAt<T>(Point position) where T : IItem
        {
            return (T)ItemDatabase.Items.Values.SingleOrDefault(a => a.Position == position);
        }

        public static T GetItemAt<T>(int x, int y) where T : IItem
        {
            return (T)ItemDatabase.Items.Values.SingleOrDefault(a => a.Position.X == x && a.Position.Y == y);
        }

        public static bool ItemExistsAt(int x, int y)
        {
            return GetItemAt<IItem>(x, y) != null;
        }

        public static bool ItemExistsAt(Point position)
        {
            return GetItemAt<IItem>(position) != null;
        }

        private static class ItemDatabase
        {
            public static readonly Dictionary<int, IItem> Items = new Dictionary<int, IItem>();

            private static int _currentId;
            public static int GetUniqueId()
            {
                return _currentId++;
            }

            public static void Reset()
            {
                Items.Clear();
                _currentId = 0;
            }
        }
    }
}
