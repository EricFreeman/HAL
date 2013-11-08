using System.Speech.Recognition;

namespace Hal.Extensions
{
    public static class GrammerExtensions
    {
        public static void Append(this GrammarBuilder gb, params string[] str)
        {
            foreach (var s in str)
            {
                gb.Append(s);
            }
        }
    }
}