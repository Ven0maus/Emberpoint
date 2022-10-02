using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;

namespace Emberpoint.Core.GameObjects.Dialogs
{
    public class TutorialDialog
    {
        public static void Start()
        {
            var dialogWindow = UserInterfaceManager.Get<DialogWindow>();
            var builder = new DialogBuilder();
            // TODO: Move text to files?
            // TODO: Add localization for dialogs
            builder.Add("Game introduction", new[]
            {
                "Welcome to Emberpoint.",
                "This is a small introduction to the game.",
                "Press 'Enter' to continue."
            }).Add("The flashlight", new[]
            {
                "The flashlight can be used to discover new places.",
                "It can be turned on using the following keybinding: '" + KeybindingsManager.GetKeybinding(Keybindings.Flashlight) + "'",
                "Press 'Enter' to continue."
            }).Add("Batteries", new[]
            {
                "The flashlight uses batteries to function.",
                "In your inventory you can see the amount and power of your batteries.",
                "Batteries can be found within the mansion, by searching for them.",
                "Press 'Enter' to continue."
            }).Add("Objects window", new[]
            {
                "In the bottom right corner you have a panel that shows nearby objects.",
                "This can be used to navigate to certain interactable objects.",
                "You can interact with some objects by standing near them and pressing: '" + KeybindingsManager.GetKeybinding(Keybindings.Interact) + "'",
                "Press 'Enter' to continue."
            });
            dialogWindow.AddDialogs(builder);
        }
    }
}
