using System.Configuration;
using System.Speech.Recognition;

namespace HAL.Framework.Modules
{
    public class StopListeningModule : IModule
    {
        public Grammar Grammar { get; set; }

        public void CreateGrammar()
        {
            var b = new GrammarBuilder();
            b.Append(Config.StopListening);

            Grammar = new Grammar(b);
        }

        public string Match(string message)
        {
            return message.Contains(Config.StopListening.ToLower()) ?
                "Goodbye " + Config.UserName :
                string.Empty;
        }
    }
}
