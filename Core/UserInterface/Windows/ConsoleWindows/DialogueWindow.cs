using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.Resources;
using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadRogue.Primitives;
using Emberpoint.Core.GameObjects.Conversation;

namespace Emberpoint.Core.UserInterface.Windows.ConsoleWindows
{
    public class DialogueWindow : BorderedWindow
    {
        DialogueSection _dialogueSection;
        readonly Color _playerTextColor = Color.Khaki;
        readonly Color _otherActorTextColor = Color.White;
        readonly Color _descriptionTextColor = Color.YellowGreen;

        public DialogueWindow(int width, int height) : base(width, height)
        {
            Position = (5, Constants.GameWindowHeight - Height - 1);
        }

        public override void Refresh()
        {
            if (_dialogueSection is not null)
            {
                // calculate the total height of the window including gaps between text, description and choices
                int lineCount = _dialogueSection.Height 
                    + (_dialogueSection.HasText() && _dialogueSection.HasDescription() ? 1 : 0) 
                    + (_dialogueSection.HasChoices() ? 1 : 0);

                // resize the window according the height needed
                if (Content.Height != lineCount)
                {
                    ResizeContentHeight(lineCount);
                    Position = (5, Constants.GameWindowHeight - Height - 1);
                }

                // remove old content
                Content.Clear();

                // display the name of the currently speaking actor
                Title = Actors.GetActor(_dialogueSection.ActorID).Name;

                // display main dialogue text
                int y = 0, count;
                if (_dialogueSection.HasText())
                {
                    count = _dialogueSection.TextLines.Length;
                    for (int i = 0; i < count; i++)
                    {
                        Color c = _dialogueSection.ActorID == 0 ? _playerTextColor : _otherActorTextColor;
                        ColoredString s = new(_dialogueSection.TextLines[i], c, Color.Black);
                        Content.Print(0, y++, s);
                    }
                    y++;
                }
                
                // display description
                if (_dialogueSection.HasDescription())
                {
                    count = _dialogueSection.DescriptionLines.Length;
                    for (int i = 0; i < count; i++)
                        Content.Print(0, y++, _dialogueSection.DescriptionLines[i], _descriptionTextColor);
                    y++;
                }

                // display choices
                if (_dialogueSection.HasChoices())
                {
                    int choiceNumber = 1;
                    foreach (var choice in _dialogueSection.Choices)
                    {
                        // print the first line of the choice text
                        string number = choiceNumber++ + ": ";
                        Content.Print(0, y++, new ColoredString(number, Color.Orange, Color.Black) 
                            + new ColoredString(choice.TextLines[0], _playerTextColor, Color.Black));

                        // if there is more, print the reminder
                        if (choice.Height > 1)
                        {
                            for (int i = 1; i < choice.TextLines.Length; i++)
                            {
                                Content.Print(3, y++, 
                                    new ColoredString(choice.TextLines[i], _playerTextColor, Color.Black));
                            }
                        }
                    }

                    // prompt the player to pick a choice
                    Prompt = Strings.PickChoice + ": 1 - " + _dialogueSection.Choices.Length;
                }
                else
                {
                    // prompt the player to press Confirm
                    var keyName = KeybindingsManager.GetKeybinding(Keybindings.Confirm);
                    Prompt = string.Format(Strings.PressConfirmToContinue, keyName);
                }
            }

            // conversation is over
            else
            {
                // transfer keyboard focus to the player and hide the window
                IsFocused = false;
                Game.Player.IsFocused = true;
                IsVisible = false;
            }
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (_dialogueSection is null) 
                throw new FieldAccessException("Dialogue Window cannot be focused without a dialogue line saved.");

            // handle a more complex dialogue section with answers to pick from
            if (_dialogueSection.HasChoices())
            {
                for (int x = 0, count = _dialogueSection.Choices.Length; x < count; x++)
                {
                    if (keyboard.IsKeyPressed((Keys) 49 + x))
                    {
                        var id = _dialogueSection.Choices[x].NextID;
                        SetDialogueSection(DialogueManager.GetDialogueSection(id));
                        break;
                    }
                }
            }

            // handle a simple dialogue text without any choices
            else
            {
                if (keyboard.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Confirm)))
                {
                    SetDialogueSection(DialogueManager.GetNextDialogueSection());
                }
            }

            return true;
        }

        void SetDialogueSection(DialogueSection dialogueSection)
        {
            dialogueSection?.SetMaxLineLength(Content.Width);
            _dialogueSection = dialogueSection;
            Refresh();
        }

        /// <summary>
        /// Shows the dialogue window with the first line of the dialogue. Transfers keyboard focus to the window.
        /// </summary>
        /// <param name="dialogueSection">The first section of the dialogue.</param>
        public void Show(DialogueSection dialogueSection)
        {
            SetDialogueSection(dialogueSection);
            Game.Player.IsFocused = false;
            IsFocused = true;
        }
    }
}