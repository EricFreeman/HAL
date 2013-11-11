namespace HAL.Framework.Extensions
{
    public static class BoolExtensions
    {
        public static bool IsTrue(this bool b)
        {
            return b;
        }

        public static bool IsFalse(this bool b)
        {
            return !b;
        }
    }
}
