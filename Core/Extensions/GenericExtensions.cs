namespace Emberpoint.Core.Extensions
{
    public static class GenericExtensions
    {
        public static bool IsDefault<T>(this T value) where T : struct
        {
            return value.Equals(default);
        }
    }
}
