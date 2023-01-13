using Emberpoint.Core.GameObjects.Conversation;
using Emberpoint.Core.UserInterface.Windows.ConsoleWindows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Emberpoint.Core.GameObjects.Managers
{
    static class DialogueManager
    {
        static Dialogue s_dialogue;

        // TODO: Check if we can save only the int ID to save memory in long-run
        static readonly List<Dialogue> s_resolvedDialogues = new();

        /// <summary>
        /// Checks if a dialogue with the given id has already been resolved.
        /// </summary>
        /// <param name="id">Id of the dialogue.</param>
        public static bool CheckResolved(Dialogues dialogue) => s_resolvedDialogues.Exists(d => d.ID == (int)dialogue);

        /// <summary>
        /// Retrieves a <see cref="DialogueSection"/> with the id equal to the NextID of the last element in <see cref="Path"/>.
        /// </summary>
        /// <returns>Either a <see cref="DialogueSection"/> or null if the id was 0.</returns>
        public static DialogueSection GetNextDialogueSection() => GetDialogueSection(s_dialogue.Path.Last().NextID);

        /// <summary>
        /// Retrieves <see cref="DialogueSection"/> with the given id.
        /// </summary>
        /// <param name="id">Id of the dialogue.</param>
        /// <returns>Either a <see cref="DialogueSection"/> or null if the id was 0.</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static DialogueSection GetDialogueSection(int id)
        {
            if (s_dialogue == null)
                throw new ApplicationException("Trying to access dialogue lines from a null dialogue.");

            if (id == 0)
            {
                s_resolvedDialogues.Add(s_dialogue);
                s_dialogue = null;
                return null;
            }
            else
            {
                var nextSection = Array.Find(s_dialogue.Sections, l => l.ID == id);
                if (nextSection is not null)
                {
                    s_dialogue.Path.Add(nextSection);
                    return nextSection;
                }
                else 
                    throw new ArgumentException("There is no dialogue line with the id: " + id);
            }
        }

        /// <summary>
        /// Loads a localized version of a dialogue with the given id and shows it in the DialogueWindow.
        /// </summary>
        /// <param name="id">Id of the dialogue.</param>
        /// <exception cref="FileLoadException">Neither a localized nor English version of the dialogue file exists.</exception>
        public static void Load(Dialogues dialogue)
        {
            // check if the dialogue has already been loaded before
            var d = s_resolvedDialogues.Find(d => d.ID == (int)dialogue);
            if (d != null)
            {
                s_dialogue = d;
                ResetPath();
                return;
            }

            // form a path pointing to the localized version of the dialogue
            string fileName = Dialogue.GetFileName((int)dialogue);
            string path = $"Resources/Dialogues/{Constants.Language}/{fileName}.json";
            string jsonString;

            // check if the localized version exists
            if (File.Exists(path))
                jsonString = File.ReadAllText(path);

            // if not, revert to the English version
            else
            {
                path = $"Resources/Dialogues/en-US/{fileName}.json";
                if (File.Exists(path))
                    jsonString = File.ReadAllText(path);
                else
                    throw new FileLoadException($"Cannot find the dialogue '{dialogue}' in atleast the en-US culture.");
            }

            // deserialize
            s_dialogue = JsonSerializer.Deserialize<Dialogue>(jsonString)!;
            if (s_dialogue.ID != (int)dialogue) throw new ArgumentException("Dialogue.ID doesn't match argument id.");

            // set path to the first section
            ResetPath();

            // show window
            UserInterfaceManager.Get<DialogueWindow>().Show(s_dialogue.Path[0]);
        }

        static void ResetPath()
        {
            s_dialogue.Path.Clear();
            var fs = s_dialogue.GetFirstSection();
            s_dialogue.Path.Add(fs);
        }
    }
}
