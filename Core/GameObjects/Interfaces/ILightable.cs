namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface ILightable
    {
        float Brightness { get; set; }
        int LightRadius { get; set; }
        bool EmitsLight { get; set; }
    }
}
