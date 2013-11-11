using System;
using System.Speech.Recognition;
using HAL.Framework.Modules;

namespace HAL.Implementation.Modules
{
    public class DateAndTimeModule : IModule
    {
        public Grammar Grammar { get; set; }

        public void CreateGrammar()
        {
            // 1[what]  2[is today's, is the]  3[time, day, date]  4[is it]

            var one = new Choices();
            one.Add(new SemanticResultValue("what", "what"));
            var sOne = new SemanticResultKey("one", one);

            var two = new Choices();
            two.Add(new SemanticResultValue("is today's", "is today's"));
            two.Add(new SemanticResultValue("is the", "is the"));
            var sTwo = new SemanticResultKey("two", two);

            var three = new Choices();
            three.Add(new SemanticResultValue("time", "time"));
            three.Add(new SemanticResultValue("day", "day"));
            three.Add(new SemanticResultValue("date", "day"));
            var sThree = new SemanticResultKey("three", three);

            var four = new Choices();
            four.Add(new SemanticResultValue("is it", "is it"));
            var sFour = new SemanticResultKey("four", four);

            // what (time, day, date) is it
            var gOne = new GrammarBuilder();
            gOne.Append(sOne);
            gOne.Append(sThree);
            gOne.Append(sFour);

            // what (is today's, is the) (time, day, date)
            var gTwo = new GrammarBuilder();
            gTwo.Append(sOne);
            gTwo.Append(sTwo);
            gTwo.Append(sThree);

            var perm = new Choices();
            perm.Add(gOne);
            perm.Add(gTwo);

            var b = new GrammarBuilder();
            b.Append(perm, 0, 1);

            Grammar = new Grammar(b);
        }

        public string Match(string message)
        {
            if (message.Contains("what"))
            {
                if (message.Contains("date") || message.Contains("day"))
                {
                    return DateTime.Now.ToString("D");
                }

                if (message.Contains("time"))
                {
                    return DateTime.Now.ToString("t");
                }
            }

            return string.Empty;
        }
    }
}