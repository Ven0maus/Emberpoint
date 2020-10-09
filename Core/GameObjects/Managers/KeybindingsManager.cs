using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Managers
{
    public static class KeybindingsManager
    {
        private static readonly Dictionary<Keybindings, Keys> _keybindings = new Dictionary<Keybindings, Keys>();

        public static void InitializeDefaultKeybindings()
        {
            (Keybindings, Keys)[] bindings = new (Keybindings, Keys)[]
            {
                (Keybindings.Movement_Up, Keys.Up),
                (Keybindings.Movement_Down, Keys.Down),
                (Keybindings.Movement_Left, Keys.Left),
                (Keybindings.Movement_Right, Keys.Right),
                (Keybindings.Flashlight, Keys.F)
            };

            foreach (var binding in bindings)
            {
                _keybindings.Add(binding.Item1, binding.Item2);
            }
        }

        public static void EditKeybinding(Keybindings binding, Keys newKey)
        {
            if (_keybindings.ContainsKey(binding))
                _keybindings[binding] = newKey;
        }

        public static Keys GetKeybinding(Keybindings binding)
        {
            if (_keybindings.TryGetValue(binding, out Keys value)) return value;
            throw new System.Exception("No keybinding defined with name: " + binding);
        }

        public static KeyValuePair<Keybindings, Keys>[] GetKeybindings()
        {
            return _keybindings.ToArray();
        }
    }

    public enum Keybindings
    {
        Movement_Up,
        Movement_Down,
        Movement_Left,
        Movement_Right,
        Flashlight
    }
}
