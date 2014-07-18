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

        public static bool ContainsOneOf(this string s, params string[] param)
        {
            foreach (var p in param)
            {
                if (s.Contains(p))
                    return true;
            }

            return false;
        }
    }
}
