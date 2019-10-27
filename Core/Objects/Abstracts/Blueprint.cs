using SadConsole;
using System;

namespace Emberpoint.Core.Objects.Abstracts
{
    public abstract class Blueprint<T> where T : Cell
    {
        public virtual T[] GetCells()
        {
            return Array.Empty<T>();
        }
    }
}
