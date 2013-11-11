using System;
using System.Speech.Recognition;

namespace HAL.Framework.Modules
{
    public interface IModule
    {
        Grammar Grammar { get; set; }

        void CreateGrammar();
        string Match(string message);
    }
}
