using System;
using System.Speech.Recognition;
using HAL.Framework.Extensions;
using HAL.Framework.Modules;

namespace HAL.Implementation.Modules
{
    public class DateAndTimeModule : IModule
    {
        public Grammar Grammar { get; set; }

        public void CreateGrammar()
        {
            // 1[what]  2[is today's, is tomorrow's, is the] 3[is today, is tomorrow]  4[time, day, date]  5[is it]

            var one = new Choices();
            one.Add(new SemanticResultValue("what", "what"));
            var sOne = new SemanticResultKey("one", one);

            var two = new Choices();
            two.Add(new SemanticResultValue("is today's", "is today"));
            two.Add(new SemanticResultValue("is tomorrow's", "is tomorrow"));
            two.Add(new SemanticResultValue("is the", "is the"));
            var sTwo = new SemanticResultKey("two", two);

            var three = new Choices();
            three.Add(new SemanticResultValue("is today", "is today"));
            three.Add(new SemanticResultValue("is tomorrow", "is tomorrow"));
            three.Add(new SemanticResultValue("was yesterday", "was yesterday"));
            var sThree = new SemanticResultKey("three", three);

            var four = new Choices();
            four.Add(new SemanticResultValue("time", "time"));
            four.Add(new SemanticResultValue("day", "day"));
            four.Add(new SemanticResultValue("date", "day"));
            var sFour = new SemanticResultKey("three", four);

            var five = new Choices();
            five.Add(new SemanticResultValue("is it", "is it"));
            var sFive = new SemanticResultKey("four", five);

            // what (time, day, date) is it
            var gOne = new GrammarBuilder();
            gOne.Append(sOne);
            gOne.Append(sFour);
            gOne.Append(sFive);

            // what (is today's, is the) (time, day, date)
            var gTwo = new GrammarBuilder();
            gTwo.Append(sOne);
            gTwo.Append(sTwo);
            gTwo.Append(sFour);

            // what (is today, is tomorrow)
            var gThree = new GrammarBuilder();
            gThree.Append(sOne);
            gThree.Append(sThree);

            var perm = new Choices();
            perm.Add(gOne);
            perm.Add(gTwo);
            perm.Add(gThree);

            var b = new GrammarBuilder();
            b.Append(perm, 0, 1);

            Grammar = new Grammar(b);
        }

        public string Match(string message)
        {
            if (message.Contains("time"))
            {
                return DateTime.Now.ToString("t");
            }

            if(message.ContainsOneOf("today", "tomorrow", "date", "day"))
            {
                if(message.Contains("today"))
                    return DateTime.Now.ToString("D");
                else if (message.Contains("tomorrow"))
                    return DateTime.Now.AddDays(1).ToString("D");
                else if (message.Contains("yesterday"))
                    return DateTime.Now.AddDays(-1).ToString("D");
            }

            return string.Empty;
        }
    }
}