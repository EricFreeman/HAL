namespace HAL.Framework.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }
    }
}
