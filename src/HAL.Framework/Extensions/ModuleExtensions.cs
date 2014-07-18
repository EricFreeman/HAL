using System;
using HAL.Framework.Modules;

namespace HAL.Framework.Extensions
{
    public static class ModuleExtensions
    {
        public static string Match(this IModule m, string message, Action onSuccess)
        {
            var s = m.Match(message);

            if (s.IsNotEmpty())
                onSuccess();

            return s;
        }
    }
}
