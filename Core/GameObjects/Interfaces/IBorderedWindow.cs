using SadConsole;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    internal interface IBorderedWindow
    {
        /// <summary>
        /// Horizontal padding between the content console and the <see cref="IBorderedWindow"/> border.
        /// </summary>
        int HorizontalPadding { get; set; }

        /// <summary>
        /// Vertical padding between the content console and the <see cref="IBorderedWindow"/> border.
        /// </summary>
        int VerticalPadding { get; set; }

        /// <summary>
        /// Text printed in the top left corner of the <see cref="IBorderedWindow"/>.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Text printed in the bottom right corner of the <see cref="IBorderedWindow"/>.
        /// </summary>
        string Prompt { get; set; }
        
        /// <summary>
        /// Internal <see cref="Console"/> that is used to print contents of the <see cref="IBorderedWindow"/>.
        /// </summary>
        Console Content { get; }

        /// <summary>
        /// Resizes the <see cref="IBorderedWindow"/> and its content to the given dimensions.
        /// </summary>
        /// <param name="width">New <see cref="IBorderedWindow"/> width.</param>
        /// <param name="height">New <see cref="IBorderedWindow"/> height.</param>
        void Resize(int width, int height);

        /// <summary>
        /// Resizes the width of the <see cref="IBorderedWindow"/>.
        /// </summary>
        /// <param name="width">New <see cref="IBorderedWindow"/> width.</param>
        void ResizeWidth(int width);

        /// <summary>
        /// Resizes the height of the <see cref="IBorderedWindow"/>.
        /// </summary>
        /// <param name="height">New <see cref="IBorderedWindow"/> height.</param>
        void ResizeHeight(int height);

        /// <summary>
        /// Resizes the Content console and, implicitly, the parent <see cref="IBorderedWindow"/>.
        /// </summary>
        /// <param name="width">New Content width.</param>
        /// <param name="height">New Content height.</param>
        void ResizeContent(int width, int height);

        /// <summary>
        /// Resizes the width of the Content console and, implicitly, the parent <see cref="IBorderedWindow"/>.
        /// </summary>
        /// <param name="width">New Content width.</param>
        void ResizeContentWidth(int width);

        /// <summary>
        /// Resizes the height of the Content console and, implicitly, the parent <see cref="IBorderedWindow"/>.
        /// </summary>
        /// <param name="height">New Content height.</param>
        void ResizeContentHeight(int height);
    }
}
