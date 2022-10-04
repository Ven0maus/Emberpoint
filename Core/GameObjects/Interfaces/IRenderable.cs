namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IRenderable<T>
    {
        void RenderObject(T console);
        void UnRenderObject();
    }
}
