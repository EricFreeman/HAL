namespace HAL.Framework.Modules
{
    public static class ModuleFactory
    {
        public static IModule CreateModule<T>() where T : IModule, new()
        {
            var t = new T();
            t.CreateGrammar();

            return t;
        }
    }
}
