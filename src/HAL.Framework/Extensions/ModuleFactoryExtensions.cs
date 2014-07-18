using System.Collections.Generic;
using HAL.Framework.Modules;

namespace HAL.Framework.Extensions
{
    public static class ModuleFactoryExtensions
    {
        public static void Add<T>(this List<IModule> list) where T : IModule, new()
        {
            IModule m = ModuleFactory.CreateModule<T>();
            list.Add(m);
        }
    }
}
