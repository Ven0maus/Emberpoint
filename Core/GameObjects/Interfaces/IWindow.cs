using SadConsole;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    internal interface IWindow
    {
        /// <summary>
        /// Horizontal padding between the content console and the window border.
        /// </summary>
        int HorizontalPadding { get; set; }

        /// <summary>
        /// Vertical padding between the content console and the window border.
        /// </summary>
        int VerticalPadding { get; set; }

        /// <summary>
        /// Text printed in the top left corner of the window.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Text printed in the bottom right corner of the window.
        /// </summary>
        string Prompt { get; set; }
        
        /// <summary>
        /// Internal <see cref="Console"/> that is used to print window's content.
        /// </summary>
        Console Content { get; }

        /// <summary>
        /// Resizes the <see cref="IWindow"/> and its content to the given dimensions.
        /// </summary>
        /// <param name="width">New <see cref="IWindow"/> width.</param>
        /// <param name="height">New <see cref="IWindow"/> height.</param>
        void Resize(int width, int height);

        /// <summary>
        /// Resizes the width of the <see cref="IWindow"/>.
        /// </summary>
        /// <param name="width">New <see cref="IWindow"/> width.</param>
        void ResizeWidth(int width);

        /// <summary>
        /// Resizes the height of the <see cref="IWindow"/>.
        /// </summary>
        /// <param name="height">New <see cref="IWindow"/> height.</param>
        void ResizeHeight(int height);

        /// <summary>
        /// Resizes the Content console and, implicitly, the parent <see cref="IWindow"/>.
        /// </summary>
        /// <param name="width">New Content width.</param>
        /// <param name="height">New Content height.</param>
        void ResizeContent(int width, int height);

        /// <summary>
        /// Resizes the width of the Content console and, implicitly, the parent <see cref="IWindow"/>.
        /// </summary>
        /// <param name="width">New Content width.</param>
        void ResizeContentWidth(int width);

        /// <summary>
        /// Resizes the height of the Content console and, implicitly, the parent <see cref="IWindow"/>.
        /// </summary>
        /// <param name="height">New Content height.</param>
        void ResizeContentHeight(int height);
    }
}
