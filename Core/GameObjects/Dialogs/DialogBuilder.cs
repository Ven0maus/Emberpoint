using System.Collections.Generic;

namespace Emberpoint.Core.GameObjects.Dialogs
{
    public sealed class DialogBuilder
    {
        private readonly Queue<Dialog> _dialogs = new Queue<Dialog>();

        public DialogBuilder Add(string title, params string[] content)
        {
            _dialogs.Enqueue(new Dialog(title, content));
            return this;
        }

        public IEnumerable<Dialog> Build()
        {
            return _dialogs.ToArray();
        }

        public class Dialog
        {
            public string Title;
            public string[] Content;

            public Dialog(string title, string[] content)
            {
                Title = title;
                Content = content;
            }
        }
    }
}
