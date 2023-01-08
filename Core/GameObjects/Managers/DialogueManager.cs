using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Emberpoint.Core.UserInterface.Windows.ConsoleWindows;
using Newtonsoft.Json.Linq;
using Emberpoint.Core.GameObjects.Conversation;

namespace Emberpoint.Core.GameObjects.Managers
{
    static class DialogueManager
    {
        static Dialogue s_dialogue;
        static readonly List<Dialogue> s_resolvedDialogues = new();

        public static DialogueSection GetNextDialogueSection() => GetDialogueSection(s_dialogue.Path.Last().NextID);

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

        public static void Load(string fileName)
        {
            string jsonString = File.ReadAllText("Resources/Dialogues/en-GB/" + fileName + ".json");
            s_dialogue = JsonSerializer.Deserialize<Dialogue>(jsonString)!;
            var firstSection = Array.Find(s_dialogue.Sections, l => l.ID == 1);
            if (firstSection is null) throw new JsonException("Cannot find the initial dialogue line (with the ID: 1).");
            s_dialogue.Path.Add(firstSection);
            UserInterfaceManager.Get<DialogueWindow>().Show(firstSection);
        }
    }
}
