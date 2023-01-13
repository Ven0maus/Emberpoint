namespace Emberpoint.Core.GameObjects.Dialogs
{
    // TODO: redo as a json dialogue

    /*
    public class TutorialDialog
    {
        public static void Start()
        {
            var dialogWindow = UserInterfaceManager.Get<DialogueWindow>();
            var builder = new DialogBuilder();
            // TODO: Move text to files?
            // TODO: Add localization for dialogs
            builder.Add("Game Introduction", new[]
            {
                "Welcome to Emberpoint.",
                "This is a small introduction to the game."
            }).Add("Flashlight", new[]
            {
                "The flashlight can be used to discover new places.",
                "It can be turned on using the following keybinding: '" + KeybindingsManager.GetKeybinding(Keybindings.Flashlight) + "'"
            }).Add("Batteries", new[]
            {
                "The flashlight uses batteries to function.",
                "In inventory you can see the amount and power of your batteries.",
                "Batteries can be found within the mansion, by searching for them."
            }).Add("Objects Window", new[]
            {
                "Bottom right corner panel shows nearby objects.",
                "It can be used to navigate to certain interactable entities.",
                "Interaction happens by standing near them and pressing: '" + KeybindingsManager.GetKeybinding(Keybindings.Interact) + "'"
            });
            dialogWindow.AddDialogs(builder);
        }
    }
    */
}
