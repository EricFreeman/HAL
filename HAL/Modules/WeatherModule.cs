using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Xml.Linq;
using HAL.Framework;
using HAL.Framework.Modules;

namespace HAL.Implementation.Modules
{
    public class WeatherModule : IModule
    {
        public Grammar Grammar { get; set; }

        public void CreateGrammar()
        {
            // 1[what's, what is, how's, how is] 2[the weather] 3[today, tomorrow, on (day of week) right now, currently, outside]

            var one = new Choices();
            one.Add(new SemanticResultValue("what's", "what"));
            one.Add(new SemanticResultValue("what is", "what"));
            one.Add(new SemanticResultValue("how's", "what"));
            one.Add(new SemanticResultValue("how is", "what"));
            var sOne = new SemanticResultKey("one", one);

            var two = new Choices();
            two.Add(new SemanticResultValue("the weather", "the weather"));
            var sTwo = new SemanticResultKey("two", two);

            var three = new Choices();
            three.Add(new SemanticResultValue("today", "today"));
            three.Add(new SemanticResultValue("tomorrow", "tomorrow"));
            three.Add(new SemanticResultValue("right now", "right now"));
            three.Add(new SemanticResultValue("currently", "currently"));
            three.Add(new SemanticResultValue("outside", "outside"));
            var sThree = new SemanticResultKey("three", three);

            var four = new Choices();
            four.Add(new SemanticResultValue("on", "on"));
            four.Add(new SemanticResultValue("for", "for"));
            var sFour = new SemanticResultKey("four", four);

            var five = new Choices();
            five.Add(new SemanticResultValue("sunday", "sunday"));
            five.Add(new SemanticResultValue("monday", "monday"));
            five.Add(new SemanticResultValue("tuesday", "tuesday"));
            five.Add(new SemanticResultValue("wednesday", "wednesday"));
            five.Add(new SemanticResultValue("thursday", "thursday"));
            five.Add(new SemanticResultValue("friday", "friday"));
            five.Add(new SemanticResultValue("saturday", "saturday"));
            var sFive = new SemanticResultKey("five", five);

            // (what's, how's) the weather (today, tomorrow)
            var gOne = new GrammarBuilder();
            gOne.Append(sOne);
            gOne.Append(sTwo);
            gOne.Append(sThree);

            // (what's, how's) the weather
            var gTwo = new GrammarBuilder();
            gTwo.Append(sOne);
            gTwo.Append(sTwo);

            // (what's, how's) the weather (for, on) (sunday, monday, tuesday, ...)
            var gThree = new GrammarBuilder();
            gThree.Append(sOne);
            gThree.Append(sTwo);
            gThree.Append(sFour);
            gThree.Append(sFive);

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
            if (message.Contains("tomorrow"))
                return GetWeather(DateTime.Now.AddDays(1));

            if (message.Contains("today"))
                return GetWeather(DateTime.Now);

            if (message.Contains("on"))
                return GetDateFromMessage(message);

            else
                return GetCurrentWeather(); //return specifics about right now if no date is specified
        }

        private string GetDateFromMessage(string message)
        {
            var stringToDayOfWeek = new Dictionary<string, int>
            {
                {"sunday", 0},
                {"monday", 1},
                {"tuesday", 2},
                {"wednesday", 3},
                {"thursday", 4},
                {"friday", 5},
                {"saturday", 6}
            };

            var split = message.Split(new [] {"on", "for"}, StringSplitOptions.RemoveEmptyEntries);
            var dayName = split.Last().Trim();
            int day = stringToDayOfWeek[dayName];
            int curDay = (int)System.Globalization.CultureInfo
                .InvariantCulture.Calendar.GetDayOfWeek(DateTime.Now);

            int daysInFuture = day - curDay;

            if (daysInFuture > 0)
                return GetWeather(DateTime.Now.AddDays(daysInFuture));
            else
                return "Sorry, that day is outside of my scope.";
        }

        private string GetCurrentWeather()
        {
            var results = GetWeatherData();
            XNamespace ns = "http://xml.weather.yahoo.com/ns/rss/1.0";
            var wind = results.Descendants(ns + "wind").FirstOrDefault();
            var atmosphere = results.Descendants(ns + "atmosphere").FirstOrDefault();
            var astronomy = results.Descendants(ns + "astronomy").FirstOrDefault();
            var condition = results.Descendants(ns + "condition").FirstOrDefault();

            return string.Format(@"It is {0} outside with a current temperature of {1} degrees, " +
                                "The whind is {2} miles per hour making it feel {3} degrees outside, " +
                                  "Humidity is at {4} percent with a visibility of {5} miles, " +
                                  "The sun {6} at {7} and {8} at {9}.",
                                   condition.Attribute("text").Value, condition.Attribute("temp").Value,
                                   wind.Attribute("speed").Value, wind.Attribute("chill").Value,
                                   atmosphere.Attribute("humidity").Value, atmosphere.Attribute("visibility").Value,
                                   DateTime.Now.TimeOfDay < DateTime.Parse(astronomy.Attribute("sunrise").Value).TimeOfDay ? "will rise" : "rose",
                                   astronomy.Attribute("sunrise").Value,
                                   DateTime.Now.TimeOfDay < DateTime.Parse(astronomy.Attribute("sunset").Value).TimeOfDay ? "will set" : "set",
                                   astronomy.Attribute("sunset").Value);
        }

        private string GetWeather(DateTime date)
        {
            var results = GetWeatherData();
            XNamespace ns = "http://xml.weather.yahoo.com/ns/rss/1.0";
            var forecast = (from i in results.Descendants(ns + "forecast") select i);

            var day = forecast.FirstOrDefault(x =>
            {
                int dateOfMonth = DateTime.Parse(x.Attribute("date").Value).Day;
                return dateOfMonth == date.Day;
            });
            
            if (day != null)
            {
                return string.Format("it is {0} with a low of {1} and a high of {2}", day.Attribute("text").Value,
                    day.Attribute("low").Value, day.Attribute("high").Value);
            }

            return string.Empty;
        }

        private XDocument GetWeatherData()
        {
            var query = String.Format("http://weather.yahooapis.com/forecastrss?w={0}", GetWoeid(Config.Location));
            return XDocument.Load(query);
        }

        private string GetWoeid(string Zipcode)
        {
            var query = String.Format("http://where.yahooapis.com/v1/places.q('{0}')?appid={1}", Zipcode, Config.AppId);
            var thisDoc = XDocument.Load(query);
            XNamespace ns = "http://where.yahooapis.com/v1/schema.rng";
         
            return (from i in thisDoc.Descendants(ns + "place") select i.Element(ns + "woeid").Value).First();
        }
    }
}
