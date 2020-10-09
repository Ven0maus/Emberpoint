namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IEntityBaseStats
    {
        int Health { get; }
        int MaxHealth { get; set; }

        void TakeDamage(int amount);
        void Heal(int amount);
    }
}
