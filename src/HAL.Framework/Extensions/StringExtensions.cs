using System.Linq;
using System.Speech.Synthesis;

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
            return param.Any(s.Contains);
        }

        public static void Talk(this string s)
        {
            var ss = new SpeechSynthesizer();
            ss.Speak(s);
        }
    }
}
