using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emberpoint.Core.GameObjects.Conversation
{
    public record Dialogue(int ID, DialogueSection[] Sections)
    {
        // list of all dialogues as a pair {id, file_name}
        static readonly Dictionary<int, string> s_dialogues = ((Dialogues[])Enum.GetValues(typeof(Dialogues)))
            .ToDictionary(a => (int)a, a => ((int)a).ToString("D3") + "_" + a.ToString());

        /// <summary>
        /// Retrieves dialogue file name as per given id.
        /// </summary>
        /// <param name="id">ID of the dialogue.</param>
        public static string GetFileName(int id) => s_dialogues[id];

        /// <summary>
        /// Dialogue sections in order as chosen by the player during conversation
        /// </summary>
        public List<DialogueSection> Path = new();

        /// <summary>
        /// Retrieves <see cref="DialogueSection"/> with the id 1.
        /// </summary>
        public DialogueSection GetFirstSection()
        {
            var firstSection = Array.Find(Sections, s => s.ID == 1);
            if (firstSection is null) 
                throw new IndexOutOfRangeException("Cannot find the initial dialogue line (with the ID: 1).");
            return firstSection;
        }
    }

    public record DialogueLine(int ID, int NextID, string Text)
    {
        int _maxLineLength = 0;
        protected const int DefaultMaxLineLength = 200;
        string[] _textLines;

        /// <summary>
        /// Dialogue text split into shorter strings according to the MaxLineLength
        /// </summary>
        public string[] TextLines
        {
            get
            {
                if (_textLines is null)
                    SetMaxLineLength(DefaultMaxLineLength);
                return _textLines;
            }
        }

        public int GetMaxLineLength() => _maxLineLength;

        /// <summary>
        /// Sets the maximum line length and breaks the dialogue text down into shorter strings per that value.
        /// </summary>
        /// <param name="maxLineLength">Maxim number of characters per line.</param>
        public virtual void SetMaxLineLength(int maxLineLength)
        {
            if (_maxLineLength != maxLineLength)
            {
                _maxLineLength = maxLineLength;
                _textLines = BreakString(Text, maxLineLength);
            }
        }

        /// <summary>
        /// Number of lines this dialogue line occupies.
        /// </summary>
        public virtual int Height
        {
            get
            {
                if (_maxLineLength == 0)
                    SetMaxLineLength(DefaultMaxLineLength);

                return _textLines.Length;
            }
        }

        /// <summary>
        /// Breaks a long string down into an array of shorter ones.
        /// </summary>
        /// <param name="input">String to be broken down.</param>
        /// <param name="maxLength">Maxim number of characters per line.</param>
        /// <returns></returns>
        public static string[] BreakString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
                return Array.Empty<string>();

            string[] words = input.Split(' ');
            List<string> lines = new();
            StringBuilder sb = new();

            string[] articles = { "a", "an", "and", "is", "are", "were", "was", "i" };
            string lastWord = "";

            for (int i = 0; i < words.Length; i++)
            {
                if (sb.Length + words[i].Length > maxLength)
                {
                    // if the language is English, move the short article words at the end of a line to the next line
                    if (Constants.Language == "en-US" && articles.Contains(lastWord.ToLower()))
                    {
                        sb.Remove(sb.Length - lastWord.Length - 1, lastWord.Length);
                        lines.Add(sb.ToString().TrimEnd());
                        sb.Clear();
                        sb.Append(lastWord + " ");

                    }
                    else
                    {
                        lines.Add(sb.ToString().TrimEnd());
                        sb.Clear();
                    }
                }

                // add word to the line
                sb.Append(words[i] + " ");
                lastWord = words[i];
            }

            lines.Add(sb.ToString().TrimEnd());
            return lines.ToArray();
        }
    }

    public record DialogueSection(int ID, int ActorID, int NextID, string Text, string Description,
        DialogueLine[] Choices) : DialogueLine(ID, NextID, Text)
    {
        string[] _descriptionLines;

        /// <summary>
        /// Checks if this <see cref="DialogueSection"/> has a text line.
        /// </summary>
        public bool HasText() => !string.IsNullOrEmpty(Text);

        /// <summary>
        /// Checks if this <see cref="DialogueSection"/> has a description.
        /// </summary>
        /// <returns></returns>
        public bool HasDescription() => !string.IsNullOrEmpty(Description);

        /// <summary>
        /// Checks if this <see cref="DialogueSection"/> has choices.
        /// </summary>
        public bool HasChoices() => Choices is not null && Choices.Length > 0;

        /// <summary>
        /// Description text broken down into an array of shorter strings as per MaxLineLength.
        /// </summary>
        public string[] DescriptionLines
        {
            get
            {
                if (_descriptionLines is null)
                    SetMaxLineLength(DefaultMaxLineLength);
                return _descriptionLines;
            }
        }

        /// <inheritdoc/>
        public override void SetMaxLineLength(int maxLineLength)
        {
            if (GetMaxLineLength() != maxLineLength)
            {
                base.SetMaxLineLength(maxLineLength);
                if (Choices != null)
                {
                    foreach (DialogueLine line in Choices)
                    {
                        // substract 3 to allow for adding the choice number at the beginning "1: " + ...
                        line.SetMaxLineLength(maxLineLength - 3);
                    }
                }
                _descriptionLines = BreakString(Description, maxLineLength);
            }
        }

        /// <summary>
        /// Total number of lines this dialogue section has including description and choices.
        /// </summary>
        public override int Height
        {
            get
            {
                if (GetMaxLineLength() == 0)
                    SetMaxLineLength(DefaultMaxLineLength);

                int choicesHeight = 0;
                if (Choices != null)
                {
                    foreach (DialogueLine line in Choices)
                    {
                        choicesHeight += line.Height;
                    }
                }

                return base.Height + _descriptionLines.Length + choicesHeight;
            }
        }
    }
}
