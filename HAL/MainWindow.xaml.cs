using System;
using System.Diagnostics;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;

namespace HAL
{
    public partial class MainWindow : Window
    {
        private readonly SpeechRecognitionEngine _recognizer;

        private readonly Grammar _startGrammar;
        private readonly Grammar _endGrammar;

        private readonly Grammar _dateAndTimeGrammar;
        private readonly Grammar _birthdayGrammar;

        public MainWindow()
        {
            InitializeComponent();

            _startGrammar = CreateStartGrammar();
            _endGrammar = CreateEndGrammar();
            _dateAndTimeGrammar = CreateDateAndTimeGrammar();

            _recognizer = new SpeechRecognitionEngine();
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammar(_startGrammar);
            _recognizer.LoadGrammar(_endGrammar);

            _recognizer.SpeechRecognized += recognizer_SpeechRecognized;
            _recognizer.SpeechHypothesized += recognizer_SpeechHypothesized;
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private Grammar CreateStartGrammar()
        {
            var b = new GrammarBuilder();
            b.Append("Hal");

            return new Grammar(b);
        }

        private Grammar CreateEndGrammar()
        {
            var b = new GrammarBuilder();
            b.Append("stop listening");

            return new Grammar(b);
        }

        private Grammar CreateDateAndTimeGrammar()
        {
            // 1[what]  2[is today's]  3[time, day, date]  4[is it]

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

            return new Grammar(b);
        }

        private void recognizer_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Debug.Write(e.Result.Text);
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var m = e.Result.Text.ToLower();

            if (m.Contains("hal"))
            {
                if (!_recognizer.Grammars.Contains(_dateAndTimeGrammar))
                {
                    _recognizer.LoadGrammar(_dateAndTimeGrammar);
                    _recognizer.LoadGrammar(_birthdayGrammar);
                    Talk("Yes Eric?");
                    return;
                }
            }

            if (m.Contains("stop listening"))
            {
                if (_recognizer.Grammars.Contains(_dateAndTimeGrammar))
                {
                    _recognizer.UnloadGrammar(_dateAndTimeGrammar);
                    Talk("Goodbye");
                    return;
                }
            }

            if (m.Contains("what"))
            {
                if (m.Contains("date") || m.Contains("day"))
                {
                    Talk(DateTime.Now.ToString("D"));
                }
                else if (m.Contains("time"))
                {
                    Talk(DateTime.Now.ToString("t"));
                }
            }
        }

        private void Talk(string message)
        {
            var ss = new SpeechSynthesizer();
            ss.Speak(message);
        }
    }

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
