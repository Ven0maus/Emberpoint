using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using SadConsole;
using System;
using Console = SadConsole.Console;

namespace Emberpoint.Core.UserInterface.Windows
{
    public abstract class BorderedWindow : Console, IWindow, IUserInterface
    {
        string _title = string.Empty;
        string _prompt = string.Empty;
        int _horizontalPadding;
        int _verticalPadding;

        public BorderedWindow(int width, int height, int hPadding = 1, int vPadding = 1) : base(width, height)
        {
            Content = new Console(1, 1) { Parent = this };
            _horizontalPadding = hPadding;
            _verticalPadding = vPadding;
            ResizeContent();
            Draw();
        }

        public void Draw()
        {
            this.DrawBorders(Width, Height, Constants.BorderStyle.Corner, Constants.BorderStyle.VerticalSide, 
                Constants.BorderStyle.HorizontalSide, Constants.Colors.WindowBorder);
            PrintTitle();
            PrintPrompt();
        }

        /// <inheritdoc/>
        public int HorizontalPadding
        {
            get => _horizontalPadding;
            set
            {
                _horizontalPadding = value;
                ResizeContent();
            }
        }

        /// <inheritdoc/>
        public int VerticalPadding
        {
            get => _verticalPadding;
            set
            {
                _verticalPadding = value;
                ResizeContent();
            }
        }

        /// <inheritdoc/>
        public string Title 
        {
            get => _title;
            set
            {
                _title = value;
                PrintTitle();
            }
        }

        /// <inheritdoc/>
        public string Prompt 
        {
            get => _prompt;
            set
            {
                _prompt = value;
                PrintPrompt();
            }
        }

        void PrintTitle()
        {
            int maxLength = Width - 6;
            if (maxLength > 0 && _title.Length > 0)
            {
                // truncate title
                string title = _title.Length > maxLength ? _title[..maxLength] : _title;

                // make sure the previous title is erased
                Surface.Print(1, 0, new string(Constants.BorderStyle.Top, Width - 2), Constants.Colors.WindowBorder);

                // print new title
                Surface.Print(3, 0, title, Constants.Colors.WindowTitle);
            }
        }

        void PrintPrompt()
        {
            int maxLength = Width - 6;
            if (maxLength > 0 && _prompt.Length > 0)
            {
                string prompt = _prompt.Length > maxLength ? _prompt[..maxLength] : _prompt;
                Surface.Print(Width - prompt.Length - 3, Height - 1, _prompt, Constants.Colors.WindowTitle);
            }
        }

        /// <inheritdoc/>
        public Console Content { get; }

        /// <inheritdoc/>
        public void Resize(int width, int height)
        {
            (Surface as CellSurface).Resize(width, height, true);
            ResizeContent();
            Draw();
        }

        /// <inheritdoc/>
        public void ResizeWidth(int width)
        {
            Resize(width, Height);
        }

        /// <inheritdoc/>
        public void ResizeHeight(int height)
        {
            Resize(Width, height);
        }

        /// <inheritdoc/>
        public void ResizeContent(int width, int height)
        {
            Resize(width + 2 + _horizontalPadding * 2, height + 2 + _verticalPadding * 2);
        }

        /// <inheritdoc/>
        public void ResizeContentWidth(int width)
        {
            Resize(width + 2 + _horizontalPadding * 2, Height);
        }

        /// <inheritdoc/>
        public void ResizeContentHeight(int height)
        {
            Resize(Width, height + 2 + _verticalPadding * 2);
        }

        void ResizeContent()
        {
            // calculate new content width and height taking into account border and padding
            int width = Width - 2 - _horizontalPadding * 2;
            int height = Height - 2 - _verticalPadding * 2;

            if (width < 1 || height < 1)
                throw new InvalidOperationException("Content console width or height cannot be 0.");

            // resize content console
            (Content.Surface as CellSurface).Resize(width, height, false);
            Content.Position = (1 + HorizontalPadding, 1 + VerticalPadding);
        }

        public virtual void Refresh() { }
        public virtual void BeforeCreate() { }
        public virtual void AfterCreate() { }
    }
}
