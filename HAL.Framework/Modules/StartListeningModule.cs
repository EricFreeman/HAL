using System.Speech.Recognition;

namespace HAL.Framework.Modules
{
    public class StartListeningModule : IModule
    {
        public Grammar Grammar { get; set; }

        public void CreateGrammar()
        {
            var b = new GrammarBuilder();
            b.Append(Config.ComputerName);

            Grammar = new Grammar(b);
        }

        public string Match(string message)
        {
            return message.Contains(Config.ComputerName.ToLower()) ? 
                "Yes " + Config.UserName : 
                string.Empty;
        }
    }
}
