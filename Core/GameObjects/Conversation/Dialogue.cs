using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emberpoint.Core.GameObjects.Conversation
{
    public record Dialogue(int ID, DialogueSection[] Sections)
    {
        /// <summary>
        /// Dialogue sections in order as chosen by the player during conversation
        /// </summary>
        public List<DialogueSection> Path = new();
    }

    public record DialogueLine(int ID, int NextID, string Text)
    {
        int _maxLineLength = 0;
        protected const int DefaultMaxLineLength = 200;
        string[] _textLines;

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

        public virtual void SetMaxLineLength(int maxLineLength)
        {
            if (_maxLineLength != maxLineLength)
            {
                _maxLineLength = maxLineLength;
                _textLines = BreakString(Text, maxLineLength);
            }
        }

        public virtual int Height
        {
            get
            {
                if (_maxLineLength == 0)
                    SetMaxLineLength(DefaultMaxLineLength);

                return _textLines.Length;
            }
        }

        // breaks a long string down into an array of shorter ones
        public static string[] BreakString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
                return Array.Empty<string>();

            string[] words = input.Split(' ');
            List<string> lines = new();
            StringBuilder sb = new();

            string[] articles = { "a", "an", "is", "are", "were", "was", "I" };
            string lastWord = "";

            for (int i = 0; i < words.Length; i++)
            {
                if (sb.Length + words[i].Length > maxLength)
                {
                    // check if the last word is in the articles
                    if (articles.Contains(lastWord.ToLower()))
                    {
                        sb.Remove(sb.Length - lastWord.Length, lastWord.Length);
                        lines.Add(sb.ToString().TrimEnd().TrimStart());
                        sb.Clear();
                        sb.Append(lastWord + " ");

                    }
                    else
                    {
                        lines.Add(sb.ToString().TrimEnd().TrimStart());
                        sb.Clear();
                    }
                }

                // add word to the line
                sb.Append(words[i] + " ");
                lastWord = words[i];
            }

            lines.Add(sb.ToString().TrimEnd().TrimStart());
            return lines.ToArray();
        }
    }

    public record DialogueSection(int ID, int ActorID, int NextID, string Text, string Description,
        DialogueLine[] Choices) : DialogueLine(ID, NextID, Text)
    {
        string[] _descriptionLines;

        public bool HasText() => !string.IsNullOrEmpty(Text);

        public bool HasDescription() => !string.IsNullOrEmpty(Description);

        public bool HasChoices() => Choices is not null && Choices.Length > 0;

        public string[] DescriptionLines
        {
            get
            {
                if (_descriptionLines is null)
                    SetMaxLineLength(DefaultMaxLineLength);
                return _descriptionLines;
            }
        }

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
